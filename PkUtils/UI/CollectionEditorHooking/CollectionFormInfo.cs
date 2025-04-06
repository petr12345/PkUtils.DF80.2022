// Ignore Spelling: Utils, Dict, listbox, validator
//
using System;
using System.Windows.Forms;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.UI.CollectionEditorHooking;

/// <summary>
/// Auxiliary class keeping information about CollectionForm. </summary>
/// <remarks>On purpose it is not structure - to be able to derive from that in descendants of CollectionEditorObserver</remarks>
public class CollectionFormInfo
{
    /// <summary> The window handle. </summary>
    protected readonly IntPtr _hwnd;

    /// <summary>
    /// The window title. </summary>
    protected readonly string _title;

    internal CollectionFormInfo(IntPtr hwnd)
        : this(hwnd, User32.GetWindowText(hwnd))
    { }

    /// <summary> A constructor. </summary>
    /// <param name="hwnd"> The window handle. </param>
    /// <param name="title"> The window title. </param>
    internal CollectionFormInfo(IntPtr hwnd, string title)
    {
        _hwnd = hwnd;
        _title = title;
    }

    /// <summary> Returns the window handle. </summary>
    public IntPtr Hwnd { get => _hwnd; }

    /// <summary> Returns the window title. </summary>
    public string Title { get => _title; }

    /// <summary> Gets the collection form. </summary>
    public Form MapCollectionForm { get => Control.FromHandle(Hwnd) as Form; }
}
