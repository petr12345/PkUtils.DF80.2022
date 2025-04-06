/***************************************************************************************************************
*
* FILE NAME:   .\Undo\IUndoableEdit.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of interface IUndoable
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Undoable
//
using System;

namespace PK.PkUtils.Undo;

/// <summary>
/// Interface IUndoable defines functionality that should be supported by any object
/// with undo/redo.
/// </summary>
[CLSCompliant(true)]
public interface IUndoable
{
    /// <summary>
    /// Returns true if there is anything to undo and this object is ready to perform such undo operation; returns false otherwise.
    /// </summary>
    bool CanUndo { get; }

    /// <summary>
    /// Returns true if there is anything to redo and this object is ready to perform such redo operation; returns false otherwise.
    /// </summary>
    bool CanRedo { get; }

    /// <summary>
    /// Performs the undo operation. 
    /// If there is nothing to undo or if the implementation engine is not in proper state, InvalidOperationException is thrown.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this object cannot undo; which could be determined by the value of property <see cref="CanUndo"/>.
    /// </exception>
    void Undo();

    /// <summary>
    /// Performs the redo operation. 
    /// If there is nothing to redo or if the implementation engine is not in proper state, InvalidOperationException is thrown.
    /// </summary>
    /// <exception cref="InvalidOperationException"> Thrown when this object cannot redo; which could be determined by the value of property <see cref="CanRedo"/> </exception>
    void Redo();

    /// <summary>
    /// Cleans all the Undo and Redo information that is currently held by this object.
    /// </summary>
    void EmptyUndoBuffer();
}
