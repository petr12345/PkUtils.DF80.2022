﻿// Ignore Spelling: Utils, Subfolders
//
using PK.PkUtils.Interfaces;
using PK.PkUtils.IO;

namespace PK.PkUtils.NUnitTests.IOTests;

/// <summary>   (Unit Test Fixture) of a class <see cref="FolderNodeDeferred"/>. </summary>
public class FolderNodeDeferredTests
{
    [SetUp]
    public void SetUp()
    { }

    #region Tests
    #region BuildFileTree Tests

    /// <summary>
    /// BuildFileTree should throw ArgumentNullException when files is null.
    /// </summary>
    [Test, Description("BuildFileTree should throw ArgumentNullException when files is null.")]
    public void BuildFileTree_NullFiles_ThrowsArgumentNullException()
    {
        // Arrange
        const string rootPath = "e:/root";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FolderNodeDeferred.BuildFileTree(null!, rootPath));
    }

    /// <summary>
    /// BuildFileTree should throw ArgumentNullException when rootPath is null.
    /// </summary>
    [Test, Description("BuildFileTree should throw ArgumentNullException when rootPath is null.")]
    public void BuildFileTree_NullRootPath_ThrowsArgumentNullException()
    {
        // Arrange
        const string rootPath = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FolderNodeDeferred.BuildFileTree([], rootPath!));
    }

    /// <summary>
    /// BuildFileTree should throw an exception when a file is outside rootPath.
    /// </summary>
    [Test, Description("BuildFileTree should throw an exception when a file is outside rootPath.")]
    public void BuildFileTree_FileOutsideRootPath_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<FileInfo> files = [new FileInfo("/outside/file.txt")];
        const string rootPath = "e:/root";

        // Act
        ArgumentException ex = Assert.Throws<ArgumentException>(() => FolderNodeDeferred.BuildFileTree(files, rootPath));

        // Assert
        Assert.That(ex!.Message, Does.Contain("is not under the root path"));
    }

    /// <summary>
    /// BuildFileTree should create an empty node when no files are provided.
    /// </summary>
    [Test, Description("BuildFileTree should create an empty node when no files are provided.")]
    [TestCase("e:/root")]
    [TestCase("e:/root/")]
    [TestCase("e:/")]
    [TestCase(@"e:\")]
    public void BuildFileTree_EmptyFiles_ReturnsEmptyNode(string rootPath)
    {
        // Arrange
        string normalizedRoot = FolderNodeDeferred.NormalizeRootPath(rootPath);

        // Act
        FolderNodeDeferred result = FolderNodeDeferred.BuildFileTree([], rootPath);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(normalizedRoot));
            Assert.That(result.Files, Is.Empty);
            Assert.That(result.Subfolders, Is.Empty);
        });
    }

    /// <summary>
    /// BuildFileTree should correctly categorize files into folders.
    /// </summary>
    [Test, Description("BuildFileTree should correctly categorize files into folders.")]
    public void BuildFileTree_ValidFiles_CreatesCorrectStructure()
    {
        // Arrange
        const string rootPath = "e:/root";
        IEnumerable<FileInfo> files =
        [
            new FileInfo("e:/root/file1.txt"),
            new FileInfo("e:/root/folderA/file2.txt"),
            new FileInfo("e:/root/folderA/folderB/file3.txt")
        ];

        // Act
        FolderNodeDeferred result = FolderNodeDeferred.BuildFileTree(files, rootPath);

        // Convert Subfolders to Dictionary for easier access
        Dictionary<string, IFolderNode> subfoldersDict = result.Subfolders.ToDictionary(f => f.Name, f => f);
        var folderB = subfoldersDict["folderA"].Subfolders.FirstOrDefault(f => f.Name == "folderB");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(subfoldersDict, Contains.Key("folderA"));
            Assert.That(subfoldersDict["folderA"].Subfolders.Any(subfolder => subfolder.Name == "folderB"), Is.True);
            Assert.That(result.Files.Any(f => f.Name == "file1.txt"), Is.True);
            Assert.That(subfoldersDict["folderA"].Files.Select(f => f.Name), Contains.Item("file2.txt"));
            Assert.That(folderB?.Files.Select(f => f.Name), Contains.Item("file3.txt"));
        });
    }

    /// <summary>
    /// BuildFileTree should correctly categorize files into a more complex folder structure.
    /// </summary>
    [Test, Description("BuildFileTree should correctly categorize files into a more complex folder structure.")]
    public void BuildFileTree_ValidFiles_CreatesCorrectComplexStructure()
    {
        // Arrange
        const string rootPath = "e:/root";
        IEnumerable<FileInfo> files =
        [
            new FileInfo("e:/root/file1.txt"),
            new FileInfo("e:/root/folderA/file2.txt"),
            new FileInfo("e:/root/folderA/folderB/file3.txt"),
            new FileInfo("e:/root/folderA/folderB/folderC/file4.txt"),
            new FileInfo("e:/root/folderA/folderB/folderC/folderD/file5.txt"),
            new FileInfo("E:/root/folderA/folderE/file6.txt"),
            new FileInfo("E:/root/folderF/file7.txt"),
            new FileInfo("E:/root/folderF/folderG/file8.txt"),
            new FileInfo("E:/root/folderF/folderG/folderH/file9.txt")
        ];

        // Act
        FolderNodeDeferred result = FolderNodeDeferred.BuildFileTree(files, rootPath);

        // Convert Subfolders to Dictionary for easier access
        Dictionary<string, IFolderNode> subfoldersDict = result.Subfolders.ToDictionary(f => f.Name, f => f);
        IFolderNode? folderB = subfoldersDict["folderA"].Subfolders.FirstOrDefault(f => f.Name == "folderB");
        IFolderNode? folderC = folderB?.Subfolders.FirstOrDefault(f => f.Name == "folderC");
        IFolderNode? folderD = folderC?.Subfolders.FirstOrDefault(f => f.Name == "folderD");
        IFolderNode? folderE = subfoldersDict["folderA"].Subfolders.FirstOrDefault(f => f.Name == "folderE");
        IFolderNode? folderG = subfoldersDict["folderF"].Subfolders.FirstOrDefault(f => f.Name == "folderG");
        IFolderNode? folderH = folderG?.Subfolders.FirstOrDefault(f => f.Name == "folderH");

        // Assert
        Assert.Multiple(() =>
        {
            // Root folder
            Assert.That(result.Files.Any(f => f.Name == "file1.txt"), Is.True);

            // FolderA and its subfolders
            Assert.That(subfoldersDict, Contains.Key("folderA"));
            Assert.That(subfoldersDict["folderA"].Files.Select(f => f.Name), Contains.Item("file2.txt"));
            Assert.That(folderB?.Subfolders.Any(f => f.Name == "folderC"), Is.True);

            // FolderB and its subfolders
            Assert.That(folderB?.Files.Select(f => f.Name), Contains.Item("file3.txt"));
            Assert.That(folderC?.Subfolders.Any(f => f.Name == "folderD"), Is.True);

            // FolderC and its subfolder
            Assert.That(folderC?.Files.Select(f => f.Name), Contains.Item("file4.txt"));
            Assert.That(folderD?.Files.Select(f => f.Name), Contains.Item("file5.txt"));

            // FolderE
            Assert.That(folderE?.Files.Select(f => f.Name), Contains.Item("file6.txt"));

            // FolderF and its subfolder
            Assert.That(subfoldersDict, Contains.Key("folderF"));
            Assert.That(subfoldersDict["folderF"].Files.Select(f => f.Name), Contains.Item("file7.txt"));
            Assert.That(folderG?.Subfolders.Any(f => f.Name == "folderH"), Is.True);

            // FolderG and its subfolder
            Assert.That(folderG?.Files.Select(f => f.Name), Contains.Item("file8.txt"));
            Assert.That(folderH?.Files.Select(f => f.Name), Contains.Item("file9.txt"));
        });
    }
    #endregion // BuildFileTree Tests
    #endregion // Tests
}
