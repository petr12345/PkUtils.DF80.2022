#nullable disable

using System.Xml.Linq;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests;

[TestFixture]
public class XObjectExtensionsTests
{
    #region GetPath_Tests
    #region GetPath_ArgumentNullException Tests

    [Test(Description = "Tests throwing ArgumentNullException when the object is null.")]
    public void GetPath_ThrowsArgumentNullException_WhenObjIsNull()
    {
        // Arrange
        XObject obj = null;

        // Act & Assert
        Assert.That(() => XObjectExtensions.GetPath(obj), Throws.ArgumentNullException);
    }
    #endregion // GetPath_ArgumentNullException Tests

    #region GetPath_Without_Line_Info_Tests

    [Test(Description = "Tests returning correct path for XElement without line info.")]
    public void GetPath_ReturnsCorrectPath_ForElementWithoutLineInfo()
    {
        // Arrange
        XDocument doc = XDocument.Parse("<root><child><subchild /></child></root>");
        XElement subchild = doc.Root.Element("child").Element("subchild");

        // Act
        string path = subchild.GetPath(showLineNumber: false);

        // Assert
        Assert.That(path, Does.StartWith("/root/child/subchild"));
        Assert.That(path, Does.Not.Contain("Line "));
    }

    [Test(Description = "Tests returning correct path for XAttribute without line info.")]
    public void GetPath_ReturnsCorrectPath_ForAttributeWithoutLineInfo()
    {
        // Arrange
        XDocument doc = XDocument.Parse("<root><child name=\"value\" /></root>");
        XAttribute attr = doc.Root.Element("child").Attribute("name");

        // Act
        string path = attr.GetPath(showLineNumber: false);

        // Assert
        Assert.That(path, Does.StartWith("/root/child/@name"));
        Assert.That(path, Does.Not.Contain("Line "));
    }
    #endregion // GetPath_Without_Line_Info_Tests

    #region GetPath_With_Line_Info_Tests

    [Test(Description = "Tests returning correct path for XElement with line info.")]
    public void GetPath_ReturnsCorrectPath_ForElementWithLineInfo()
    {
        // Arrange
        const string expectedLineInfo = "Line 3:";
        XDocument doc = XDocument.Load(
            new System.IO.StringReader("<root>\n  <child>\n    <subchild />\n  </child>\n</root>"),
            LoadOptions.SetLineInfo
        );
        XElement subchild = doc.Root.Element("child").Element("subchild");

        // Act
        string path = subchild.GetPath(showLineNumber: true);

        // Assert
        Assert.That(path, Does.Contain(expectedLineInfo));
        path = path.Substring(path.IndexOf(expectedLineInfo) + expectedLineInfo.Length).TrimStart();
        Assert.That(path, Does.StartWith("/root/child/subchild"));

    }

    [Test(Description = "Tests returning correct path for XAttribute with line info.")]
    public void GetPath_ReturnsCorrectPath_ForAttributeWithLineInfo()
    {
        // Arrange
        const string expectedLineInfo = "Line 2:";
        XDocument doc = XDocument.Load(
            new System.IO.StringReader("<root>\n  <child name=\"value\" />\n</root>"),
            LoadOptions.SetLineInfo
        );
        XAttribute attr = doc.Root.Element("child").Attribute("name");

        // Act
        string path = attr.GetPath(showLineNumber: true);

        // Assert
        Assert.That(path, Does.StartWith(expectedLineInfo));
        path = path[(path.IndexOf(expectedLineInfo) + expectedLineInfo.Length)..].TrimStart();
        Assert.That(path, Does.StartWith("/root/child/@name"));
    }
    #endregion // GetPath_With_Line_Info_Tests

    #region GetPath_Sibling_Index_Tests

    [Test(Description = "Tests including sibling index when multiple siblings with the same name exist.")]
    public void GetPath_IncludesSiblingIndex_WhenMultipleSiblingsWithSameName()
    {
        // Arrange
        XDocument doc = XDocument.Parse("<root><item /><item /><item /><item2/></root>");
        XElement[] items = [.. doc.Root.Elements("item")];

        // Act
        string path1 = items[0].GetPath(showLineNumber: false);
        string path2 = items[1].GetPath(showLineNumber: false);
        string path3 = items[2].GetPath(showLineNumber: false);
        string path4 = doc.Root.Element("item2").GetPath(showLineNumber: false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(path1, Does.Contain("/root/item(1)"));
            Assert.That(path2, Does.Contain("/root/item(2)"));
            Assert.That(path3, Does.Contain("/root/item(3)"));
            Assert.That(path4, Is.EqualTo("/root/item2"));
        }
    }
    #endregion // GetPath_Sibling_Index_Tests
    #endregion // GetPath_Tests

    #region GetPathAndLineNumber_Tests

    #region GetPathAndLineNumber_ArgumentNullException Tests

    [Test(Description = "Tests throwing ArgumentNullException when the object is null.")]
    public void GetPathAndLineNumber_ThrowsArgumentNullException_WhenObjIsNull()
    {
        // Arrange
        XObject obj = null;

        // Act & Assert
        Assert.That(() => XObjectExtensions.GetPathAndLineNumber(obj, out _), Throws.ArgumentNullException);
    }
    #endregion // GetPathAndLineNumber_ArgumentNullException Tests

    #region GetPathAndLineNumber_Without_Line_Info_Tests

    [Test(Description = "Tests returning correct path for XElement without line info.")]
    public void GetPathAndLineNumber_ReturnsCorrectPath_ForElementWithoutLineInfo()
    {
        // Arrange
        XDocument doc = XDocument.Parse("<root><child><subchild /></child></root>");
        XElement subchild = doc.Root.Element("child").Element("subchild");

        // Act
        string path = subchild.GetPathAndLineNumber(out _);

        // Assert
        Assert.That(path, Is.EqualTo("/root/child/subchild"));
    }

    [Test(Description = "Tests returning correct path for XAttribute without line info.")]
    public void GetPathAndLineNumber_ReturnsCorrectPath_ForAttributeWithoutLineInfo()
    {
        // Arrange
        XDocument doc = XDocument.Parse("<root><child name=\"value\" /></root>");
        XAttribute attr = doc.Root.Element("child").Attribute("name");

        // Act
        string path = attr.GetPathAndLineNumber(out _);

        // Assert
        Assert.That(path, Is.EqualTo("/root/child/@name"));
    }
    #endregion // GetPathAndLineNumber_Without_Line_Info_Tests

    #region GetPathAndLineNumber_With_Line_Info_Tests

    [Test(Description = "Tests returning correct path for XElement with line info.")]
    public void GetPathAndLineNumber_ReturnsCorrectPath_ForElementWithLineInfo()
    {
        // Arrange
        const int expectedLineNumber = 3;
        const string expectedPath = "/root/child/subchild";
        XDocument doc = XDocument.Load(
            new System.IO.StringReader("<root>\n  <child>\n    <subchild />\n  </child>\n</root>"),
            LoadOptions.SetLineInfo
        );
        XElement subchild = doc.Root.Element("child").Element("subchild");

        // Act
        string path = subchild.GetPathAndLineNumber(out int? lineNumber);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(lineNumber, Is.EqualTo(expectedLineNumber));
            Assert.That(path, Is.EqualTo(expectedPath));
        }
    }

    [Test(Description = "Tests returning correct path for XAttribute with line info.")]
    public void GetPathAndLineNumber_ReturnsCorrectPath_ForAttributeWithLineInfo()
    {
        // Arrange
        const int expectedLineNumber = 2;
        const string expectedPath = "/root/child/@name";
        XDocument doc = XDocument.Load(
            new System.IO.StringReader("<root>\n  <child name=\"value\" />\n</root>"),
            LoadOptions.SetLineInfo
        );
        XAttribute attr = doc.Root.Element("child").Attribute("name");

        // Act
        string path = attr.GetPathAndLineNumber(out int? lineNumber);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(lineNumber, Is.EqualTo(expectedLineNumber));
            Assert.That(path, Is.EqualTo(expectedPath));
        }
    }
    #endregion // GetPathAndLineNumber_With_Line_Info_Tests

    #region GetPathAndLineNumber_Sibling_Index_Tests

    [Test(Description = "Tests including sibling index when multiple siblings with the same name exist.")]
    public void GetPathAndLineNumber_IncludesSiblingIndex_WhenMultipleSiblingsWithSameName()
    {
        // Arrange
        const string inputXml = "<root>\n  <item />\n  <item />\n  <item />\n  <item2/>\n</root>";
        XDocument doc = XDocument.Parse(inputXml, LoadOptions.SetLineInfo);
        XElement[] items = [.. doc.Root.Elements("item")];

        // Act
        string path1 = items[0].GetPathAndLineNumber(out int? lineInfo1);
        string path2 = items[1].GetPathAndLineNumber(out int? lineInfo2);
        string path3 = items[2].GetPathAndLineNumber(out int? lineInfo3);
        string path4 = doc.Root.Element("item2").GetPathAndLineNumber(out int? lineInfo4);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(path1, Is.EqualTo("/root/item(1)"));
            Assert.That(path2, Is.EqualTo("/root/item(2)"));
            Assert.That(path3, Is.EqualTo("/root/item(3)"));
            Assert.That(path4, Is.EqualTo("/root/item2"));

            Assert.That(lineInfo1, Is.EqualTo(2));
            Assert.That(lineInfo2, Is.EqualTo(3));
            Assert.That(lineInfo3, Is.EqualTo(4));
            Assert.That(lineInfo4, Is.EqualTo(5));
        }
    }
    #endregion // GetPathAndLineNumber_Sibling_Index_Tests
    #endregion // GetPathAndLineNumber_Tests
}
