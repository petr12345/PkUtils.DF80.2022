using System.Drawing;
using System.IO.IsolatedStorage;
using System.Text;
using System.Text.Json;
using PK.PkUtils.Cloning.Json;
using PK.PkUtils.IO;
using PK.PkUtils.NativeMemory;
using PK.PkUtils.Serialization;

namespace PK.PkUtils.NUnitTests.IOTests;

/// <summary>Regression tests for JSON contracts, type registration and protocol rejection.</summary>
[TestFixture]
public sealed class JsonMigrationTests
{
    /// <summary>A mutable graph used to verify cycles and shared references.</summary>
    public sealed class Node
    {
        /// <summary>Gets or sets the node's name.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Gets or sets a reference to another node.</summary>
        public Node? Next { get; set; }
        /// <summary>Gets or sets a repeated graph reference.</summary>
        public Node? Other { get; set; }
    }

    /// <summary>A registered custom application value.</summary>
    public sealed class CustomValue
    {
        /// <summary>Gets or sets the Unicode payload.</summary>
        public string Text { get; set; } = string.Empty;
    }

    private sealed class ObjectStore : ApplicationObjStorage
    {
        public ObjectStore(string suffix) : base(DefaultStorageScope, true, suffix) { }
        public string FileName => SettingsFileName;
        public void WriteRaw(string text)
        {
            using IsolatedStorageFile store = GetStorageFile();
            using IsolatedStorageFileStream stream = new(FileName, FileMode.Create, store);
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes);
        }
        public void DeleteFile()
        {
            using IsolatedStorageFile store = GetStorageFile();
            if (store.FileExists(FileName)) store.DeleteFile(FileName);
        }
    }

    private sealed class JsonSegmentProbe : Segment
    {
        private JsonSegmentProbe() : base("unused", false) { }
        public static MemoryStream Encode(object value) => ObjectToStream(value);
        public static object Decode(Stream stream) => ObjectFromStream(stream);
    }

    /// <summary>Verifies shared-memory protocol serialization independently of Win32 transport.</summary>
    [Test]
    public void Segment_JsonProtocolRoundTrip()
    {
        Segment.RegisterType<CustomValue>("tests-custom-value-v1");
        using MemoryStream stream = JsonSegmentProbe.Encode(new CustomValue { Text = "Liška podšitá" });
        CustomValue copy = (CustomValue)JsonSegmentProbe.Decode(stream);
        Assert.That(copy.Text, Is.EqualTo("Liška podšitá"));
    }

    /// <summary>Verifies invalid protocol data is rejected without invoking native transport.</summary>
    [TestCase("not-json")]
    [TestCase("{\"$format\":\"PK.PkUtils.SharedMemory.Json\",\"$version\":99,\"$data\":{\"$type\":\"tests-custom-value-v1\",\"$value\":{}}}")]
    [TestCase("{\"$format\":\"wrong\",\"$version\":1,\"$data\":{\"$type\":\"tests-custom-value-v1\",\"$value\":{}}}")]
    [TestCase("{\"$format\":\"PK.PkUtils.SharedMemory.Json\",\"$version\":1,\"$data\":{\"$type\":\"unknown\",\"$value\":{}}}")]
    public void Segment_JsonProtocolRejectsInvalidPayload(string json)
    {
        Segment.RegisterType<CustomValue>("tests-custom-value-v1");
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(json));
        SharedMemoryException? exception = Assert.Throws<SharedMemoryException>((Action)(() => JsonSegmentProbe.Decode(stream)));
        Assert.That(exception!.InnerException, Is.TypeOf<JsonException>());
    }

    /// <summary>Verifies cycles and repeated references survive cloning.</summary>
    [Test]
    public void Clone_PreservesCyclesAndSharedReferences()
    {
        Node child = new() { Name = "Liška podšitá" };
        Node source = new() { Name = "root", Next = child, Other = child };
        child.Next = source;
        Node copy = CloneHelperJson.DeepClone(source);
        Assert.That(copy, Is.Not.SameAs(source));
        Assert.That(copy.Next, Is.Not.SameAs(child));
        Assert.That(copy.Next, Is.SameAs(copy.Other));
        Assert.That(copy.Next!.Next, Is.SameAs(copy));
        Assert.That(copy.Next.Name, Is.EqualTo(child.Name));
    }

    /// <summary>Verifies object-valued geometry, null and custom types survive persistence.</summary>
    [Test]
    public void ObjectStore_RoundTripsRegisteredRuntimeTypes()
    {
        ApplicationObjStorage.RegisterType<CustomValue>("tests-custom-value-v1");
        string suffix = Guid.NewGuid().ToString("N");
        ObjectStore source = new(suffix);
        try
        {
            source["int"] = 42;
            source["point"] = new Point(12, 34);
            source["size"] = new Size(56, 78);
            source["rectangle"] = new Rectangle(1, 2, 3, 4);
            source["null"] = null!;
            source["custom"] = new CustomValue { Text = "Liška podšitá" };
            source.Save();
            ObjectStore copy = new(suffix);
            Assert.That(copy.FileName, Does.EndWith(".json"));
            Assert.That(copy["int"], Is.TypeOf<int>().And.EqualTo(42));
            Assert.That(copy["point"], Is.TypeOf<Point>().And.EqualTo(source["point"]));
            Assert.That(copy["size"], Is.TypeOf<Size>().And.EqualTo(source["size"]));
            Assert.That(copy["rectangle"], Is.TypeOf<Rectangle>().And.EqualTo(source["rectangle"]));
            Assert.That(copy["null"], Is.Null);
            Assert.That(((CustomValue)copy["custom"]).Text, Is.EqualTo("Liška podšitá"));
        }
        finally { source.DeleteFile(); }
    }

    /// <summary>Verifies unsupported types are rejected instead of implicitly registered.</summary>
    [Test]
    public void ObjectStore_RejectsUnregisteredRuntimeType()
    {
        ObjectStore store = new(Guid.NewGuid().ToString("N"));
        try
        {
            store["unknown"] = new Node();
            Assert.Throws<NotSupportedException>((Action)(() => store.Save()));
        }
        finally { store.DeleteFile(); }
    }

    /// <summary>Verifies serializer failures do not truncate a previously valid store.</summary>
    [Test]
    public void ObjectStore_FailedSavePreservesPreviousData()
    {
        string suffix = Guid.NewGuid().ToString("N");
        ObjectStore source = new(suffix);
        try
        {
            source["value"] = 42;
            source.Save();
            source["unsupported"] = new Node();
            Assert.Throws<NotSupportedException>((Action)(() => source.Save()));
            ObjectStore copy = new(suffix);
            Assert.That(copy["value"], Is.EqualTo(42));
            Assert.That(copy.ContainsKey("unsupported"), Is.False);
        }
        finally { source.DeleteFile(); }
    }

    /// <summary>Verifies malformed JSON and unknown discriminators report safe-load failure.</summary>
    [TestCase("{broken")]
    [TestCase("{\"item\":{\"$type\":\"not-registered\",\"$value\":{}}}")]
    public void ObjectStore_ReportsInvalidData(string json)
    {
        string suffix = Guid.NewGuid().ToString("N");
        ObjectStore source = new(suffix);
        try
        {
            source.WriteRaw(json);
            ObjectStore copy = new(suffix);
            Assert.That(copy.LastSafeLoadResult.Success, Is.False);
            Assert.That(copy.LastLoadFoundAnyStorage, Is.True);
            Assert.That(copy, Is.Empty);
        }
        finally { source.DeleteFile(); }
    }

    /// <summary>Verifies conflicting registry mappings are rejected.</summary>
    [Test]
    public void Registry_RejectsConflictingMappings()
    {
        JsonObjectTypeRegistry registry = new();
        registry.Register<int>("integer");
        registry.Register<int>("integer");
        Assert.Throws<ArgumentException>((Action)(() => registry.Register<string>("integer")));
        Assert.Throws<ArgumentException>((Action)(() => registry.Register<int>("other")));
        Assert.Throws<ArgumentException>((Action)(() => registry.Register<object>("object")));
    }

    /// <summary>Verifies invalid shared-memory protocol payloads fail explicitly.</summary>
    [TestCase("not-json")]
    [TestCase("{\"$format\":\"PK.PkUtils.SharedMemory.Json\",\"$version\":99,\"$data\":{\"$type\":\"tests-custom-value-v1\",\"$value\":{}}}")]
    [TestCase("{\"$format\":\"wrong\",\"$version\":1,\"$data\":{\"$type\":\"tests-custom-value-v1\",\"$value\":{}}}")]
    [TestCase("{\"$format\":\"PK.PkUtils.SharedMemory.Json\",\"$version\":1,\"$data\":{\"$type\":\"unknown\",\"$value\":{}}}")]
    public void Segment_RejectsInvalidPayload(string json)
    {
        Segment.RegisterType<CustomValue>("tests-custom-value-v1");
        using Segment segment = new("json-tests-" + Guid.NewGuid().ToString("N"), Encoding.UTF8.GetBytes(json));
        SharedMemoryException? exception = Assert.Throws<SharedMemoryException>((Action)(() => segment.GetData()));
        Assert.That(exception!.InnerException, Is.TypeOf<JsonException>());
    }

    /// <summary>Verifies an object larger than the native buffer is rejected.</summary>
    [Test]
    public void Segment_RejectsInsufficientCapacity()
    {
        Segment.RegisterType<CustomValue>("tests-custom-value-v1");
        using Segment segment = new("json-tests-" + Guid.NewGuid().ToString("N"), SharedMemoryCreationFlag.Create, 20);
        Assert.Throws<SharedMemoryException>((Action)(() => segment.SetData(new CustomValue { Text = "too large" })));
    }

    /// <summary>Verifies unregistered object transfer fails before native memory is allocated.</summary>
    [Test]
    public void Segment_RejectsUnregisteredType()
    {
        Assert.Throws<SharedMemoryException>((Action)(() => new Segment("json-tests-" + Guid.NewGuid().ToString("N"), new Node())));
    }
}
