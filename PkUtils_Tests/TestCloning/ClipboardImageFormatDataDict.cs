using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PK.TestCloning;

/// <summary>Owns clipboard images and round-trips them as format GUIDs and PNG bytes.</summary>
/// <remarks>Images are copied before the source stream is disposed. The dictionary owns its images.</remarks>
[JsonConverter(typeof(ClipboardImageFormatDataDictJsonConverter))]
public class ClipboardImageFormatDataDict : Dictionary<ImageFormat, Image>, IDisposable
{
    /// <summary>Releases the owned images.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases dictionary contents when disposing.</summary>
    /// <param name="disposing">Whether managed resources should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (KeyValuePair<ImageFormat, Image> pair in this)
            {
                pair.Value?.Dispose();
            }
            Clear();
        }
    }
}

/// <summary>Converts an image dictionary without activating types from serialized data.</summary>
public sealed class ClipboardImageFormatDataDictJsonConverter : JsonConverter<ClipboardImageFormatDataDict>
{
    /// <inheritdoc />
    public override ClipboardImageFormatDataDict Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        ClipboardImageFormatDataDict result = new();
        try
        {
            foreach (JsonElement entry in document.RootElement.EnumerateArray())
            {
                ImageFormat format = new(entry.GetProperty("Format").GetGuid());
                byte[] bytes = entry.GetProperty("Png").GetBytesFromBase64();
                using MemoryStream stream = new(bytes);
                using Image decoded = Image.FromStream(stream);
                Image copy = new Bitmap(decoded);
                try
                {
                    result.Add(format, copy);
                }
                catch
                {
                    copy.Dispose();
                    throw;
                }
            }
        }
        catch
        {
            result.Dispose();
            throw;
        }
        return result;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ClipboardImageFormatDataDict value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (KeyValuePair<ImageFormat, Image> pair in value)
        {
            using MemoryStream stream = new();
            pair.Value.Save(stream, ImageFormat.Png);
            writer.WriteStartObject();
            writer.WriteString("Format", pair.Key.Guid);
            writer.WriteBase64String("Png", stream.ToArray());
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
