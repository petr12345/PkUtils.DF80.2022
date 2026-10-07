// Ignore Spelling: substring
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using PK.PkUtils.XmlSerialization;

namespace PK.TestTgSchema;


/// <summary>
/// For demo purpose, in this prototype we identify instances of FieldLine 
/// by two ways; one of which is this enum (see TaggingFieldTables._myLinesFieldsDescr ).
/// Generally, usage of enum for such purpose is not a good design...
/// </summary>
/// <remarks>
/// The other way of fields identification in this prototype is the class FieldTypeId 
/// </remarks>
public enum EFieldLineType
{
    kIdFieldInvalid = 0,
    IdField_ProjName,
    IdField_Resistance,
    IdField_Diameter,
}

internal static class FieldLineTypeExtensions
{
    public static bool IsValid(this EFieldLineType fieldType)
    {
        return fieldType != EFieldLineType.kIdFieldInvalid;
    }

    public static EFieldLineType FromString(string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        // 1. Normalize and try direct enum parse (case-insensitive).
        string s = str.Trim();
        if (Enum.TryParse<EFieldLineType>(s, ignoreCase: true, out EFieldLineType result))
        {
            return result;
        }

        // 2. Use simple substring matching for common synonyms.
        string lowered = s.ToLowerInvariant();

        if (lowered.Contains("project") || lowered.Contains("proj") || lowered.Contains("name") || lowered.Contains("title"))
            return EFieldLineType.IdField_ProjName;

        if (lowered.Contains("resist") || lowered.Contains("ohm") || lowered.Contains("res"))
            return EFieldLineType.IdField_Resistance;

        if (lowered.Contains("diam") || lowered.Contains("size"))
            return EFieldLineType.IdField_Diameter;

        // 3. No match found.
        return EFieldLineType.kIdFieldInvalid;
    }
}


/// <summary>
/// Base class of any field
/// </summary>
public abstract class FieldGeneral : Object
{
}

/// <summary>
/// Base class of any component field
/// </summary>
public abstract class FieldComponent : FieldGeneral
{
}

/// <summary>
/// Base class of any line field
/// </summary>
public abstract class FieldLine : FieldGeneral
{
    public abstract EFieldLineType WhatIs { get; }
}

#region Components_Fields_classes
public class Field_PRJ_Title : FieldComponent
{
}

public class Field_DOC_Author : FieldComponent
{
}

public class Field_DOC_Title : FieldComponent
{
}

public class Field_DOC_Copyright : FieldComponent
{
}

public class Field_Year : FieldComponent
{
}

public class Field_Month : FieldComponent
{
}

public class Field_DayOfWeek : FieldComponent
{
}

public class Field_Dog : FieldComponent
{
}
#endregion // Components_Fields_classes

#region Lines_Fields_classes

public class Field_Resistance : FieldLine
{
    public override EFieldLineType WhatIs { get { return EFieldLineType.IdField_Resistance; } }
}
public class Field_Diameter : FieldLine
{
    public override EFieldLineType WhatIs { get { return EFieldLineType.IdField_Diameter; } }
}
public class Field_ProjName : FieldLine
{
    public override EFieldLineType WhatIs { get { return EFieldLineType.IdField_ProjName; } }
}
#endregion // Lines_Fields_classes

/// <summary>
/// Represents a field type identifier during runtime and serialization.
/// For XML serialization, it is derived from the NodeSerializer of the specified type.
/// </summary>
/// <remarks>
/// JSON uses a stable, explicitly permitted field discriminator; it never resolves CLR names from JSON.
/// </remarks>
[JsonConverter(typeof(FieldTypeIdJsonConverter))]
public class FieldTypeId : NodeSerializer<Type>, IEquatable<FieldTypeId>
{

    #region Constructors

    public FieldTypeId() : base(null)
    { }
    public FieldTypeId(Type t) : base(t)
    { }

    #endregion // Constructors

    #region Methods

    public override int GetHashCode()
    {
        return Node.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return (obj is FieldTypeId id) && Equals(id);
    }
    #endregion // Methods

    #region IEquatable<FieldTypeId> Members

    public bool Equals(FieldTypeId other)
    {
        if (other is null)
            return false;
        if (object.ReferenceEquals(this, other))
            return true;
        if (object.ReferenceEquals(this.Node, other.Node))
            return true;
        return false;
    }
    #endregion // IEquatable<FieldTypeId> Members

}

/// <summary>Maps field identifiers to the sample's fixed set of allowed field types.</summary>
public sealed class FieldTypeIdJsonConverter : JsonConverter<FieldTypeId>
{
    private static readonly System.Collections.Generic.Dictionary<string, Type> _types = new(StringComparer.Ordinal)
    {
        ["project-title"] = typeof(Field_PRJ_Title),
        ["document-author"] = typeof(Field_DOC_Author),
        ["document-title"] = typeof(Field_DOC_Title),
        ["copyright"] = typeof(Field_DOC_Copyright),
        ["year"] = typeof(Field_Year),
        ["month"] = typeof(Field_Month),
        ["day-of-week"] = typeof(Field_DayOfWeek),
        ["dog"] = typeof(Field_Dog),
        ["resistance"] = typeof(Field_Resistance),
        ["diameter"] = typeof(Field_Diameter),
        ["project-name"] = typeof(Field_ProjName),
    };

    /// <inheritdoc />
    public override FieldTypeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string discriminator = reader.GetString();
        if (discriminator == null || !_types.TryGetValue(discriminator, out Type type))
        {
            throw new JsonException("Unsupported field type discriminator.");
        }
        return new FieldTypeId(type);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FieldTypeId value, JsonSerializerOptions options)
    {
        string discriminator = null;
        foreach (System.Collections.Generic.KeyValuePair<string, Type> pair in _types)
        {
            if (pair.Value == value.Node)
            {
                discriminator = pair.Key;
                break;
            }
        }
        if (discriminator == null)
        {
            throw new NotSupportedException("The field type is not registered for tagging-schema JSON.");
        }
        writer.WriteStringValue(discriminator);
    }
}
