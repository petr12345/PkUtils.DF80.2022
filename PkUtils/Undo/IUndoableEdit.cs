/***************************************************************************************************************
*
* FILE NAME:   .\Undo\IUndoableEdit.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of interfaces IUndoableEdit and ICompoundEdit
*
**************************************************************************************************************/

#define UNDO_ACTIVATION_SUPPORT
// Ignore Spelling: Utils, Undoable
//
using System;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Undo;

/// <summary>
/// IUndoableEdit is a single item in undo-redo buffer.
/// </summary>
[CLSCompliant(true)]
public interface IUndoableEdit : IUndoable, IDisposableEx
{
    /// <summary>Is this edit item significant enough for shaping individual Undo operation, 
    /// with related menu text.</summary>
    bool Significant { get; }

    /// <summary> Gets the name (characteristic) of this item in undo-redo buffer. 
    ///  Can be used in User Interface, for instance in the menu item text.</summary>
    string PresentationName { get; }

    /// <summary> Gets the name (characteristic) of this item in undo-redo buffer, that is specific for <see cref="IUndoable.Undo"/>operation.
    ///  Can be used in User Interface, for instance in the menu item text.</summary>
    string UndoPresentationName { get; }

    /// <summary> Gets the name (characteristic) of this item in undo-redo buffer, that is specific for <see cref="IUndoable.Redo"/>operation.
    ///  Can be used in User Interface, for instance in the menu item text.</summary>
    string RedoPresentationName { get; }

#if UNDO_ACTIVATION_SUPPORT

    /// <summary>
    /// This property is used in situation if the recorded undo operation is compound operation 
    /// which consist of multiple IUndoableEdit elements. <br/>
    /// The property IsActive on the last element helps to determine whether that particular sub-operation is still in progress;
    /// for instance, assuming that sub-operation is typing into one of more text controls, whether the typing is still in progress.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Returns true if Undo operation can be called for this edit even while it is in Active state.
    /// Otherwise returns false.
    /// </summary>
    /// <seealso cref="IsActive"/>
    bool CanUndoWhileActive { get; }

    /// <summary>
    /// Returns true if Redo operation can be called for this edit even while it is in Active state.
    /// Otherwise returns false.
    /// </summary>
    /// <seealso cref="IsActive"/>
    bool CanRedoWhileActive { get; }

    /// <summary>
    /// Should return true if successfully deactivated, or if no deactivation was needed.
    /// Return false for unsuccessfully deactivated (if the edit had to stay active ).
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    bool Deactivate();

#endif // UNDO_ACTIVATION_SUPPORT

#if DEBUG
    /// <summary>
    /// Returns a description (textual representation) of the undo information currently held by this object.
    /// </summary>
    string Say { get; }
#endif // DEBUG
}

/// <summary>
/// ICompoundEdit consists of more child IUndoableEdit items.
/// Undo/redo of ICompoundEdit involves in sequence data of all these child items.
/// </summary>
/// <remarks> Every ICompoundEdit must be created with <see cref="ICompoundEdit.IsOpenMultiMode"/>
/// property set to true, and remaining true till <see cref="ICompoundEdit.EndMultiMode"/> is called.<br/>
/// By calling <see cref="ICompoundEdit.EndMultiMode"/>, the compound edit is effectively 'closed',
/// and will prevent adding any more by <see cref="IUndoableEdit "/> child items 
/// by <see cref="AddEdit"/> call.</remarks>
[CLSCompliant(true)]
public interface ICompoundEdit : IUndoableEdit
{
    /// <summary>
    /// Returns true while the edit is 'open' for adding child edits.
    /// After the last child edit is added, the using code should call EndMultiMode.
    /// </summary>
    bool IsOpenMultiMode { get; }

    /// <summary>
    /// Add child edit item
    /// </summary>
    /// <param name="e">The child edit item being added.</param>
    /// <returns>True on success, false on failure.</returns>
    bool AddEdit(IUndoableEdit e);

    /// <summary>
    /// End the 'multi mode'. Should be called once ( after) the last action item has been added.
    /// </summary>
    /// <remarks> There is no counterparting 'StartMultiMode' method. 
    /// For more details, see the remarks of <see cref="ICompoundEdit"/> description.
    /// </remarks>
    void EndMultiMode();
}
