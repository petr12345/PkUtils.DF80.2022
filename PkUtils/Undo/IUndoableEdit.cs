
// Define UNDO_ACTIVATION_SUPPORT if you want to support activation/deactivation of compound undo operations 
// (e.g., tracking whether an edit is still in progress).
#define UNDO_ACTIVATION_SUPPORT

// Ignore Spelling: Utils, Undoable
//
using System;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Undo;

/// <summary>
/// Represents a single edit operation in the undo-redo buffer.
/// </summary>
[CLSCompliant(true)]
public interface IUndoableEdit : IUndoable, IDisposableEx
{
    /// <summary>
    /// Gets a value indicating whether this edit is significant enough to be treated as a separate undo operation, 
    /// including its associated menu text.
    /// </summary>
    bool Significant { get; }

    /// <summary>
    /// Gets the display name of this edit in the undo-redo buffer. 
    /// This can be used in the user interface, such as in menu item text.
    /// </summary>
    string PresentationName { get; }

    /// <summary>
    /// Gets the display name of this edit for the undo operation.
    /// This can be used in the user interface, such as in menu item text.
    /// </summary>
    string UndoPresentationName { get; }

    /// <summary>
    /// Gets the display name of this edit for the redo operation.
    /// This can be used in the user interface, such as in menu item text.
    /// </summary>
    string RedoPresentationName { get; }

    /// <summary>
    /// Gets a textual description of the undo information currently held by this object.
    /// </summary>
    string Say { get; }

#if UNDO_ACTIVATION_SUPPORT

    /// <summary>
    /// Gets a value indicating whether this edit is currently active.
    /// This is relevant for compound operations consisting of multiple <see cref="IUndoableEdit"/> elements.
    /// For example, this can indicate whether a typing operation is still in progress.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Gets a value indicating whether the undo operation can be performed while this edit is active.
    /// </summary>
    /// <seealso cref="IsActive"/>
    bool CanUndoWhileActive { get; }

    /// <summary>
    /// Gets a value indicating whether the redo operation can be performed while this edit is active.
    /// </summary>
    /// <seealso cref="IsActive"/>
    bool CanRedoWhileActive { get; }

    /// <summary>
    /// Attempts to deactivate this edit. 
    /// Returns <c>true</c> if deactivation was successful or not required; otherwise, <c>false</c> if the edit must remain active.
    /// </summary>
    /// <returns><c>true</c> if successfully deactivated or no deactivation was needed; otherwise, <c>false</c>.</returns>
    bool Deactivate();

#endif // UNDO_ACTIVATION_SUPPORT
}

/// <summary>
/// Represents a compound edit consisting of multiple child <see cref="IUndoableEdit"/> items.
/// Undoing or redoing a <see cref="ICompoundEdit"/> processes all its child edits in sequence.
/// </summary>
/// <remarks>
/// Every <see cref="ICompoundEdit"/> must be created with <see cref="IsOpenMultiMode"/> set to <c>true</c>,
/// and remain open until <see cref="EndMultiMode"/> is called.
/// Calling <see cref="EndMultiMode"/> closes the compound edit, preventing further child edits from being added via <see cref="AddEdit"/>.
/// </remarks>
[CLSCompliant(true)]
public interface ICompoundEdit : IUndoableEdit
{
    /// <summary>
    /// Gets a value indicating whether the compound edit is open for adding child edits.
    /// After the last child edit is added, <see cref="EndMultiMode"/> should be called.
    /// </summary>
    bool IsOpenMultiMode { get; }

    /// <summary>
    /// Adds a child edit item to this compound edit.
    /// </summary>
    /// <param name="e">The child edit item to add. Cannot be <c>null</c>.</param>
    /// <returns><c>true</c> if the child edit was added successfully; otherwise, <c>false</c>.</returns>
    bool AddEdit(IUndoableEdit e);

    /// <summary>
    /// Closes the compound edit, preventing any further child edits from being added.
    /// Should be called after the last action item has been added.
    /// </summary>
    /// <remarks>
    /// There is no corresponding 'StartMultiMode' method.
    /// For more details, see the remarks for <see cref="ICompoundEdit"/>.
    /// </remarks>
    void EndMultiMode();
}
