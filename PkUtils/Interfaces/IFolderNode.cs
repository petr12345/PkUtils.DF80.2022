// Ignore Spelling: CCA, Utils, Subfolders
//
using System.Collections.Generic;
using System.IO;

namespace PK.PkUtils.Interfaces;


/// <summary> Interface for folder node. </summary>
/// <summary> Interface for folder node. </summary>
public interface IFolderNode
{
    /// <summary> Gets the name of the folder. </summary>
    string Name { get; }

    /// <summary> Gets the subfolders. </summary>
    IEnumerable<IFolderNode> Subfolders { get; }

    /// <summary> Gets the collection of files contained in this folder. </summary>
    IEnumerable<FileInfo> Files { get; }
}
