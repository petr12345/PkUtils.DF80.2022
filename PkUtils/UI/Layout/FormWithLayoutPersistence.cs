// Ignore Spelling: frm
//

using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.Utils;
using static PK.PkUtils.UI.Layout.FormLayoutPersister;

namespace PK.PkUtils.UI.Layout;

/// <summary>
/// A Windows Forms base class that supports layout persistence.
/// This form automatically saves and restores its layout (size, position, and other relevant settings)
/// between application sessions.
/// </summary>
[CLSCompliant(true)]
public partial class FormWithLayoutPersistence : Form
{
    /// <summary>
    /// For layout persistence functionality. ( This has actually nothing to do with trace listening ).
    /// </summary>
    private FormLayoutPersister _frmLayoutPersister;

    /// <summary> Has the last layout persistence operation failed?. </summary>
    private bool _lastLayoutLoadHasFailed;

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public FormWithLayoutPersistence()
    {
        InitializeComponent();
    }
    #endregion // Constructor(s)

    #region Protected Properties

    /// <summary>
    /// Gets the layout persister responsible for saving and restoring the form's layout.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the layout persister has not been initialized.
    /// </exception>
    protected FormLayoutPersister LayoutPersister
    {
        get
        {
            if (_frmLayoutPersister is null)
                throw new InvalidOperationException("The layout persister has not been initialized.");
            return _frmLayoutPersister;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the last layout load operation has failed.
    /// </summary>
    protected bool LastLayoutLoadHasFailed
    {
        get { return _lastLayoutLoadHasFailed; }
    }

    /// <summary> Gets the dumper for error logging. The value could be null. </summary>
    protected virtual IDumper Dumper { get => null; }

    #endregion // Protected Properties

    #region Methods

    /// <summary> Initializes the layout persistence logic. </summary>
    protected virtual void LoadLayout()
    {
        if (!DesignerSupport.IsDesignMode(this))
        {
            Initialize(StorageAction.SizeAndLocation);
        }
    }

    /// <summary> Initializes the layout persistence logic. </summary>
    protected virtual void LoadLocation()
    {
        if (!DesignerSupport.IsDesignMode(this))
        {
            Initialize(StorageAction.Location);
        }
    }


    /// <summary> Saves the form layout when closing. </summary>
    protected virtual void SaveLayout()
    {
        try
        {
            _frmLayoutPersister?.StoreLastLocationAndSize(LastLayoutLoadHasFailed);
            _lastLayoutLoadHasFailed = false;
        }
        catch (SerializationException)
        {
            ResetLayoutPersister();
            _lastLayoutLoadHasFailed = true;
        }
    }

    /// <summary> Resets the layout persister. </summary>
    /// <returns> A new FormLayoutPersister. </returns>
    protected virtual FormLayoutPersister ResetLayoutPersister()
    {
        _frmLayoutPersister = new FormLayoutPersister(this, Dumper);
        _lastLayoutLoadHasFailed = false;

        return LayoutPersister;
    }

    /// <summary> Ensures layout is saved when the form is closing. </summary>
    /// <param name="args"> A FormClosingEventArgs that contains the event data. </param>
    protected override void OnFormClosing(FormClosingEventArgs args)
    {
        SaveLayout();
        base.OnFormClosing(args);
    }

    private IComplexResult Initialize(StorageAction actions)
    {
        IComplexResult initResult = null;

        if (!DesignerSupport.IsDesignMode(this))
        {
            try
            {
                initResult = ResetLayoutPersister().Initialize(actions);
                _lastLayoutLoadHasFailed = initResult.NullSafe(x => x.Failed());
            }
            catch (SerializationException)
            {
                ResetLayoutPersister();
                _lastLayoutLoadHasFailed = true;
            }
        }

        return initResult;
    }
    #endregion // Methods
}
