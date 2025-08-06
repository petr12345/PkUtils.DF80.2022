using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace PK.PkUtils.Extensions;

/// <summary>
/// Provides extension methods for <see cref="XObject"/> to assist with XML path extraction and diagnostics.
/// </summary>
public static class XObjectExtensions
{
    #region Public Methods

    /// <summary>
    /// Gets the XML path of the specified <see cref="XObject"/>, which may be an <see cref="XElement"/> or <see cref="XAttribute"/>.
    /// The path includes element and attribute names, and optionally the line number if available.
    /// For elements with siblings of the same name, a 1-based index is appended to distinguish them.
    /// </summary>
    /// <param name="obj">The XML object to get the path for. Must not be <c>null</c>.</param>
    /// <param name="showLineNumber">If <c>true</c>, includes the line number in the path if available.</param>
    /// <returns>
    /// A string representing the XML path of the object, including element/attribute names,
    /// optional sibling index, and optional line number.
    /// </returns>
    /// <remarks>
    /// The resulting path may look like "/root/child/subchild" without line number info,
    /// and "Line 2: /root/child/@name" with line number info.
    /// </remarks>
    /// <seealso cref="GetPathAndLineNumber"/>
    public static string GetPath(this XObject obj, bool showLineNumber = true)
    {
        ArgumentNullException.ThrowIfNull(obj);

        (List<string> pathParts, int? lineNumber) = GetPathPartsAndLineNumber(obj);

        // Add line number if requested and availableů otherwise add null for the trailing slash
        if (showLineNumber && lineNumber.HasValue)
        {
            pathParts.Add($"Line {lineNumber.Value}: ");
        }
        else
        {
            pathParts.Add(null);
        }

        string result = string.Join('/', ((IEnumerable<string>)pathParts).Reverse());
        return result;
    }

    /// <summary>
    /// Gets the XML path of the specified <see cref="XObject"/> and outputs the line number if available.
    /// The path includes element and attribute names, and for elements with siblings of the same name,
    /// a 1-based index is appended to distinguish them. The line number is returned via the out parameter.
    /// </summary>
    /// <param name="obj">The XML object to get the path for. Must not be <c>null</c>.</param>
    /// <param name="lineNumber">Outputs the line number if available; otherwise, <c>null</c>.</param>
    /// <returns>
    /// A string representing the XML path of the object, including element/attribute names and optional sibling index.
    /// </returns>
    /// <seealso cref="GetPath"/>
    /// <remarks> Unlike with <see cref="GetPath"/>, the line number is NOT included in resulting string, but returned sepaartely.
    ///           The resulting string will look like "/root/child/@name/".
    /// </remarks>
    public static string GetPathAndLineNumber(this XObject obj, out Nullable<int> lineNumber)
    {
        ArgumentNullException.ThrowIfNull(obj);

        (List<string> pathParts, int? line) = GetPathPartsAndLineNumber(obj);

        lineNumber = line;
        pathParts.Add(null);  // Add last part for the trailing slash

        string result = string.Join('/', ((IEnumerable<string>)pathParts).Reverse());
        return result;
    }
    #endregion // Public Methods

    #region Private Methods

    /// <summary>
    /// Core logic for building the XML path for an XObject, returning the path parts and line number if available.
    /// </summary>
    /// <param name="obj">The XML object to get the path for. Can't be null. </param>
    /// <returns>
    /// Tuple containing:
    ///   - List of path parts (from leaf to root, not reversed)
    ///   - Nullable line number if available
    /// </returns>
    private static (List<string> pathParts, int? lineNumber) GetPathPartsAndLineNumber(XObject obj)
    {
        static string NameWithIndex(XElement e)
        {
            // Return element name and append the index if it has siblings with the same name.
            // Index is 1-based, so we add 1 to the count of elements before self.
            XName name = e.Name;
            int index = e.ElementsBeforeSelf(name).Count();
            return $"{name}{(index > 0 || e.ElementsAfterSelf(name).Any() ? $"({index + 1})" : null)}";
        }

        string firstPart = obj switch
        {
            XAttribute a => $"@{a.Name}",
            XElement e => NameWithIndex(e),
            _ => obj.ToString()
        };
        List<string> pathParts = [firstPart];

        for (XElement parent = obj.Parent; parent != null; parent = parent.Parent)
        {
            pathParts.Add(NameWithIndex(parent));
        }

        int? lineNumber = (obj is IXmlLineInfo lineInfo && lineInfo.LineNumber > 0) ? lineInfo.LineNumber : null;
        return (pathParts, lineNumber);
    }
    #endregion // Private Methods
}
