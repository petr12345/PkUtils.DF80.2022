using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PK.SubstEditLib.Subst;

namespace PK.TestTgSchema.TextBoxCtrls;

/// <summary>Stores logical text and field positions in a versioned JSON contract.</summary>
/// <typeparam name="TFIELDID">The explicitly supported field identifier type.</typeparam>
/// <remarks>The substitution map is supplied by the receiving control, not persisted as CLR objects.</remarks>
internal static class TaggingSchemaJson<TFIELDID>
{
    /// <summary>Reads a logical schema and rejects unsupported formats or versions.</summary>
    internal static SubstLogData<TFIELDID> Read(Stream stream)
    {
        SchemaData data = JsonSerializer.Deserialize<SchemaData>(stream)
            ?? throw new JsonException("The schema payload was null.");
        if (data.Format != "PK.TaggingSchema.Json" || data.Version != 1 || data.Text == null || data.Fields == null)
        {
            throw new JsonException("Unsupported or incomplete tagging-schema JSON.");
        }
        SubstLogData<TFIELDID> result = new();
        result.SetLogStr(data.Text);
        foreach (FieldData field in data.Fields)
        {
            if (field == null || field.Position < 0 || field.Position > data.Text.Length)
            {
                throw new JsonException("Invalid field position in tagging-schema JSON.");
            }
            result.AppendLogInfo(new LogInfo<TFIELDID>(field.Identifier, field.Position));
        }
        return result;
    }

    /// <summary>Writes only the logical schema data needed to reconstruct the document.</summary>
    internal static void Write(Stream stream, SubstLogData<TFIELDID> source)
    {
        SchemaData data = new()
        {
            Format = "PK.TaggingSchema.Json",
            Version = 1,
            Text = source.GetLogStr,
            Fields = new List<FieldData>(),
        };
        foreach (LogInfo<TFIELDID> field in source.GetLogList)
        {
            data.Fields.Add(new FieldData { Identifier = field.What, Position = field.Pos });
        }
        JsonSerializer.Serialize(stream, data);
    }

    /// <summary>The versioned logical schema envelope.</summary>
    public sealed class SchemaData
    {
        /// <summary>Gets or sets the protocol identifier.</summary>
        public string Format { get; set; }
        /// <summary>Gets or sets the protocol version.</summary>
        public int Version { get; set; }
        /// <summary>Gets or sets logical text.</summary>
        public string Text { get; set; }
        /// <summary>Gets or sets the ordered field entries.</summary>
        public List<FieldData> Fields { get; set; }
    }

    /// <summary>A field's identifier and logical position.</summary>
    public sealed class FieldData
    {
        /// <summary>Gets or sets the field identifier.</summary>
        public TFIELDID Identifier { get; set; }
        /// <summary>Gets or sets the logical position.</summary>
        public int Position { get; set; }
    }
}
