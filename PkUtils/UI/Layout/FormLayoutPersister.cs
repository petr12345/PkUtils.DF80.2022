// Ignore Spelling: Persister, FormLayoutPersister, Utils
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows.Forms;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.IO;

namespace PK.PkUtils.UI.Layout;


/// <summary>
/// FormLayoutPersister allows to persist the size and location of the Form.
/// Internally, it involves the <see cref="ApplicationObjStorage"/> class to do this job.<br/>
/// Usage example:
/// <code>      
/// public class SimpleClass : Form 
/// { 
///   // create class member
///   protected FormLayoutPersister _frmLayoutPersister;
///   // initialize size and location in the constructor
///   public SimpleClass()
///   {
///     _frmLayoutPersister = new FormLayoutPersister(this);
///     _frmLayoutPersister.InitializeLastSize();
///     _frmLayoutPersister.InitializeLastLocation();
///   }
///      
///   // attach store to the FormClosing event
///   private void SimpleClass_FormClosing(object sender, FormClosingEventArgs args)
///   {
///     _frmLayoutPersister.StoreLastSize();
///     _frmLayoutPersister.StoreLastLocation();
///   }
/// }
/// </code>
/// </summary>
/// <remarks>
/// The code respects screen resolution (each resolution has unique storage for its size and location);
/// this is taken care about in the virtual method <see cref="CreateStorageName"/>.
/// The code saves individual data for each form type 
/// ( again, this is taken care in that virtual method CreateStorageName  )
/// </remarks>
public class FormLayoutPersister
{
    #region Typedefs

    /// <summary>
    /// Available actions what to store/restore
    /// </summary>
    [Flags]
    public enum StorageAction
    {
        /// <summary> A binary constant representing the width flag. </summary>
        Width = 0x1,
        /// <summary> A binary constant representing the height flag. </summary>
        Height = 0x2,
        /// <summary> A binary constant representing the size flag. </summary>
        Size = 0x4,
        /// <summary> A binary constant representing the location flag. </summary>
        Location = 0x8,
        /// <summary> A binary constant representing the width and height flag. </summary>
        WidthAndHeight = Width | Height,
        /// <summary> A binary constant representing the width and location flag. </summary>
        WidthAndLocation = Width | Location,
        /// <summary> A binary constant representing the height and location flag. </summary>
        HeightAndLocation = Height | Location,
        /// <summary> A binary constant representing the size and location flag. </summary>
        SizeAndLocation = Size | Location,
        /// <summary> A binary constant representing all flag. </summary>
        All = Width | Height | Size | Location
    }

    /// <summary>
    /// Works as an argument for a CreateStorageName method. Describes the particular kind of storage information.
    /// </summary>
    protected enum StorageInfoKind
    {
        /// <summary>
        /// Identifies that the form width is stored.
        /// </summary>
        Width = 0,
        /// <summary>
        /// Identifies that the form height is stored.
        /// </summary>
        Height = 1,
        /// <summary>
        /// Identifies that the form size is stored.
        /// </summary>
        Size = 2,
        /// <summary>
        /// Identifies that the form location is stored.
        /// </summary>
        Location = 3
    }
    #endregion // Typedefs

    #region Fields

    /// <summary> attached form </summary>
    private readonly Form _form;
    /// <summary> error logging </summary>
    private readonly IDumper _dumper;
    private readonly IsolatedStorageScope _storageScope;
    private readonly Dictionary<StorageInfoKind, bool> _mapInitialized = [];
    #endregion // Fields

    #region Constructors
    /// <summary> Class constructor. </summary>
    /// <exception cref="ArgumentNullException"> Form is null. </exception>
    /// <param name="form"> Form that wants to store its size and location. </param>
    /// <param name="dumper"> (Optional) serves for error logging. </param>
    /// <param name="storageScope"> (Optional) Scope of underlying isolated storage. </param>
    public FormLayoutPersister(
        Form form,
        IDumper dumper,
        IsolatedStorageScope storageScope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly)
    {
        ArgumentNullException.ThrowIfNull(form);
        _form = form;
        _dumper = dumper;
        _storageScope = storageScope;
    }
    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Returns attached form
    /// </summary>
    public Form CurrentForm
    {
        get { return _form; }
    }

    /// <summary> Returns underlying isolated storage scope, as provided by constructor. </summary>        
    public IsolatedStorageScope StorageScope { get => _storageScope; }

    #endregion // Properties

    #region Methods
    #region Public Methods
    #region Initialize

    /// <summary>
    /// Overloads InitializeLastWidth, calling internally the overloaded version with the argument 
    /// of storage name as form type name.
    /// </summary>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    public IComplexResult InitializeLastWidth()
    {
        IComplexResult initResult = InitializeLastWidth(CreateStorageName(StorageInfoKind.Width));

        if (initResult is not null)
        {
            if (initResult.Success)
                SetInitialized(StorageInfoKind.Width);
            else
                LogError(initResult);
        }

        return initResult;
    }

    /// <summary>
    /// Overloads InitializeLastHeight, calling internally the overloaded version with the argument 
    /// of storage name as form type name.
    /// </summary>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    public IComplexResult InitializeLastHeight()
    {
        IComplexResult initResult = InitializeLastHeight(CreateStorageName(StorageInfoKind.Height));

        if (initResult is not null)
        {
            if (initResult.Success)
                SetInitialized(StorageInfoKind.Height);
            else
                LogError(initResult);
        }

        return initResult;
    }

    /// <summary>
    /// Overloads InitializeLastSize, calling internally the overloaded version with the argument 
    /// of storage name as form type name.
    /// </summary>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    public IComplexResult InitializeLastSize()
    {
        IComplexResult initResult = InitializeLastSize(CreateStorageName(StorageInfoKind.Size));

        if (initResult is not null)
        {
            if (initResult.Success)
                SetInitialized(StorageInfoKind.Size);
            else
                LogError(initResult);
        }

        return initResult;
    }

    /// <summary>
    /// Overloads InitializeLastLocation, calling internally the overloaded version with the argument 
    /// of storage name as form type name.
    /// </summary>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    public IComplexResult InitializeLastLocation()
    {
        IComplexResult initResult = InitializeLastLocation(CreateStorageName(StorageInfoKind.Location));

        if (initResult is not null)
        {
            if (initResult.Success)
                SetInitialized(StorageInfoKind.Location);
            else
                LogError(initResult);
        }

        return initResult;
    }

    /// <summary> Initializes last location and size. </summary>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    public IComplexResult InitializeLastLocationAndSize()
    {
        IComplexResult result = InitializeLastLocation();

        if (result is null || result.Success)
        {
            result = InitializeLastSize();
        }

        return result;
    }

    /* Such method probably does not make sense
    /// <summary> Initializes all possible values </summary>
    /// <returns>True on success, false otherwise</returns>
    public bool InitializeAll()
    {
    }
    */

    /// <summary>
    /// Allows to use initialization combinations, given by the bitmask argument actions
    /// </summary>
    /// <param name="actions">Actions mask, using StorageActions flags enum</param>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    public IComplexResult Initialize(StorageAction actions)
    {
        static IComplexResult CombineResult(IComplexResult existing, IComplexResult additional)
        {
            IComplexResult modified;

            if (existing is null)
                modified = additional;
            else if (existing.Success)
                modified = additional ?? existing;
            else
                modified = existing;

            return modified;
        }

        IComplexResult result = null;

        Dictionary<StorageAction, Func<IComplexResult>> actionMap = new()
        {
            { StorageAction.Height, InitializeLastHeight },
            { StorageAction.Width, InitializeLastWidth },
            { StorageAction.Size, InitializeLastSize },
            { StorageAction.Location, InitializeLastLocation }
        };

        foreach (KeyValuePair<StorageAction, Func<IComplexResult>> entry in actionMap)
        {
            if (actions.HasFlag(entry.Key) && (result is null || result.Success))
            {
                result = CombineResult(result, entry.Value());
            }
        }

        return result;
    }
    #endregion // Initialize

    #region Store

    /// <summary>
    /// Overloads StoreLastWidth, calling internally the overloaded version with the argument 
    /// of storage name as form type name.
    /// </summary>
    /// <param name="bSafeLoad">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    public void StoreLastWidth(bool bSafeLoad)
    {
        StoreLastWidth(CreateStorageName(StorageInfoKind.Width), bSafeLoad);
    }

    /// <summary>
    /// Overloads StoreLastHeight, calling internally the overloaded version with the argument 
    /// of storage name as form type name.
    /// </summary>
    /// <param name="bSafeLoad">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    public void StoreLastHeight(bool bSafeLoad)
    {
        StoreLastHeight(CreateStorageName(StorageInfoKind.Height), bSafeLoad);
    }

    /// <summary>
    /// Overloads StoreLastSize, calling internally the overloaded version with the argument 
    /// of storage name as form type name.
    /// </summary>
    /// <param name="bSafeLoad">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    public void StoreLastSize(bool bSafeLoad)
    {
        StoreLastSize(CreateStorageName(StorageInfoKind.Size), bSafeLoad);
    }

    /// <summary>
    /// Overloads StoreLastLocation, calling internally the overloaded version with the argument 
    /// of storage name as form type name.
    /// </summary>
    /// <param name="bSafeStore">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    public void StoreLastLocation(bool bSafeStore)
    {
        StoreLastLocation(CreateStorageName(StorageInfoKind.Location), bSafeStore);
    }

    /// <summary>
    /// Stores location and size at once
    /// </summary>
    /// <param name="bSafeStore">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    public void StoreLastLocationAndSize(bool bSafeStore)
    {
        StoreLastLocation(bSafeStore);
        StoreLastSize(bSafeStore);
    }

    /// <summary>
    /// Allows you to use combinations, which ones you want
    /// </summary>
    /// <param name="actions">Actions mask, use StorageActions enum</param>
    /// <param name="bSafeStore">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    public void Store(int actions, bool bSafeStore)
    {
        if ((actions & (int)StorageAction.Height) == (int)StorageAction.Height)
        {
            StoreLastHeight(bSafeStore);
        }
        if ((actions & (int)StorageAction.Width) == (int)StorageAction.Width)
        {
            StoreLastWidth(bSafeStore);
        }
        if ((actions & (int)StorageAction.Size) == (int)StorageAction.Size)
        {
            StoreLastSize(bSafeStore);
        }
        if ((actions & (int)StorageAction.Location) == (int)StorageAction.Location)
        {
            StoreLastLocation(bSafeStore);
        }
    }

    /// <summary>
    /// Stores all
    /// </summary>
    /// <param name="bSafeStore">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    public void StoreAll(bool bSafeStore)
    {
        StoreLastHeight(bSafeStore);
        StoreLastWidth(bSafeStore);
        StoreLastSize(bSafeStore);
        StoreLastLocation(bSafeStore);
    }

    /// <summary>
    /// Stores all previously initialized values ( only )
    /// </summary>
    /// <param name="bSafeStore">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    public void StorePreviouslyInitialized(bool bSafeStore)
    {
        if (IsInitialized(StorageInfoKind.Height))
        {
            StoreLastHeight(bSafeStore);
        }
        if (IsInitialized(StorageInfoKind.Width))
        {
            StoreLastWidth(bSafeStore);
        }
        if (IsInitialized(StorageInfoKind.Size))
        {
            StoreLastSize(bSafeStore);
        }
        if (IsInitialized(StorageInfoKind.Location))
        {
            StoreLastLocation(bSafeStore);
        }
    }
    #endregion // Store
    #endregion // Public Methods

    #region Protected methods

    /// <summary> Logs an error. </summary>
    /// <param name="result"> The result. Can't be null. </param>
    protected virtual void LogError(IComplexResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Failed())
        {
            _dumper?.DumpLine(result.ErrorMessage);
        }
    }

    /// <summary>
    /// Has been the value for the given enum <see cref="StorageInfoKind"/> value initialized ?
    /// </summary>
    /// <param name="item">Describes the particular kind of storage information.</param>
    /// <returns>True if for the particular <paramref name="item"/> the value has been initialized; false otherwise.</returns>
    /// <seealso cref="SetInitializedVal"/>
    protected bool IsInitialized(StorageInfoKind item)
    {
        return _mapInitialized.ContainsKey(item) && _mapInitialized[item];
    }

    /// <summary>
    /// Mark the value for the given StorageInfoKind as true (initialized).
    /// </summary>
    /// <param name="item">Describes the particular kind of storage information.</param>
    /// <returns>returns true</returns>
    protected bool SetInitialized(StorageInfoKind item)
    {
        return SetInitializedVal(item, true);
    }

    /// <summary>
    /// Assign the value for the given StorageInfoKind to the given bVal.
    /// </summary>
    /// <param name="item">Describes the particular kind of storage information.</param>
    /// <param name="bVal">The assigned value.</param>
    /// <returns>Returns the original argument value ( bVal )</returns>
    protected bool SetInitializedVal(StorageInfoKind item, bool bVal)
    {
        _mapInitialized[item] = bVal;
        return bVal;
    }

    /// <summary> Helper method creating a structured-storage name, given the enum <see cref="StorageInfoKind"/>
    /// value, which specifies the kind of information being stored. </summary>
    ///
    /// <param name="item"> Specifies the kind of information being stored. </param>
    ///
    /// <returns> Name of the index where the item is stored. </returns>
    protected virtual string CreateStorageName(StorageInfoKind item)
    {
        string result = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetStorageGeneralName(), item);

        return result;
    }
    #endregion // Protected methods

    #region Private Methods
    #region Initialize

    /// <summary> Initializes last stored width. </summary>
    /// <param name="strStorageItemName"> The name of the item in ApplicationObjStorage where the value resides. </param>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    private IComplexResult InitializeLastWidth(string strStorageItemName)
    {
        var storage = new ApplicationObjStorage(StorageScope, true);
        IComplexResult result = storage.LastSafeLoadResult;

        if (result.NullSafe(x => x.Success))
        {
            object obj = storage.GetValueOrDefault(strStorageItemName);
            bool succeeded = false;

            if (obj != null)
            {
                try
                {
                    succeeded = AssignWidth((int)obj);
                }
                catch (InvalidCastException)
                {
                    // Empty on purpose
                }
            }
            result = succeeded ? ComplexResult.OK : null;
        }

        return result;
    }

    /// <summary> Initializes stored height. </summary>
    /// <param name="strStorageItemName"> The name of the item in ApplicationObjStorage where the value resides. </param>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    private IComplexResult InitializeLastHeight(string strStorageItemName)
    {
        var storage = new ApplicationObjStorage(StorageScope, true);
        IComplexResult result = storage.LastSafeLoadResult;

        if (result.NullSafe(x => x.Success))
        {
            object obj = storage.GetValueOrDefault(strStorageItemName);
            bool succeeded = false;

            if (obj != null)
            {
                try
                {
                    succeeded = AssignHeight((int)obj);
                }
                catch (InvalidCastException)
                {
                    // Empty on purpose
                }
            }
            result = succeeded ? ComplexResult.OK : null;
        }

        return result;
    }

    /// <summary> Initializes last stored size. </summary>
    /// <param name="strStorageItemName"> The name of the item in ApplicationObjStorage where the value resides. </param>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    private IComplexResult InitializeLastSize(string strStorageItemName)
    {
        var storage = new ApplicationObjStorage(StorageScope, true);
        IComplexResult result = storage.LastSafeLoadResult;

        if (result.NullSafe(x => x.Success))
        {
            object obj = storage.GetValueOrDefault(strStorageItemName);
            bool succeeded = false;

            if (obj != null)
            {
                try
                {
                    succeeded = AssignSize((Size)obj);
                }
                catch (InvalidCastException)
                {
                    // Empty on purpose
                }
            }
            result = succeeded ? ComplexResult.OK : null;
        }

        return result;
    }

    /// <summary> Initializes last stored location. </summary>
    /// <param name="strStorageItemName"> The name of the item in ApplicationObjStorage where the value resides. </param>
    /// <returns> IComplexResult encapsulates success or failure; or null if just nothing happened. </returns>
    private IComplexResult InitializeLastLocation(string strStorageItemName)
    {
        var storage = new ApplicationObjStorage(StorageScope, true);
        IComplexResult result = storage.LastSafeLoadResult;

        if (result.NullSafe(x => x.Success))
        {
            object obj = storage.GetValueOrDefault(strStorageItemName);
            bool succeeded = false;

            if (obj != null)
            {
                try
                {
                    succeeded = AssignLocation((Point)obj);
                }
                catch (InvalidCastException)
                {
                    // Empty on purpose
                }
            }
            result = succeeded ? ComplexResult.OK : null;
        }

        return result;
    }
    #endregion // Initialize

    #region Store

    /// <summary>
    /// The method counter-parting to InitializeLastWidth.
    /// </summary>
    /// <param name="strStorageItemName">The name of the item in ApplicationObjStorage where the value resides</param>
    /// <param name="bSafeLoad">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    private void StoreLastWidth(string strStorageItemName, bool bSafeLoad)
    {
        var storage = new ApplicationObjStorage(StorageScope, bSafeLoad)
        {
            [strStorageItemName] = CurrentForm.Width
        };
        storage.Save();
    }

    /// <summary>
    /// The method counter-parting to InitializeLastHeight.
    /// </summary>
    /// <param name="strStorageItemName">The name of the item in ApplicationObjStorage where the value resides</param>
    /// <param name="bSafeLoad">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    private void StoreLastHeight(string strStorageItemName, bool bSafeLoad)
    {
        var storage = new ApplicationObjStorage(StorageScope, bSafeLoad)
        {
            [strStorageItemName] = CurrentForm.Height
        };
        storage.Save();
    }

    /// <summary>
    /// The method counter-parting to InitializeLastSize.
    /// </summary>
    /// <param name="strStorageItemName">The name of the item in ApplicationObjStorage where the value resides</param>
    /// <param name="bSafeLoad">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    private void StoreLastSize(string strStorageItemName, bool bSafeLoad)
    {
        var storage = new ApplicationObjStorage(StorageScope, bSafeLoad)
        {
            [strStorageItemName] = CurrentForm.Size
        };
        storage.Save();
    }

    /// <summary>
    /// The method counter-parting to InitializeLastLocation.
    /// </summary>
    /// <param name="strStorageItemName">The name of the item in ApplicationObjStorage where the value resides</param>
    /// <param name="bSafeStore">
    /// If this argument is false, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.LoadData()"/> <br/>
    /// If this argument is true, the involved constructor of ApplicationStorage{object}
    /// calls  <see cref="ApplicationStorage{T}.SafeLoadData()"/> <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the
    /// property <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </param>
    private void StoreLastLocation(string strStorageItemName, bool bSafeStore)
    {
        bool bSafeLoad = bSafeStore;
        ApplicationObjStorage storage = new(StorageScope, bSafeLoad)
        {
            [strStorageItemName] = CurrentForm.Location
        };
        storage.Save();
    }
    #endregion // Store

    #region Assigns
    /// <summary>
    /// Implementation helper called by InitializeLastWidth.
    /// </summary>
    /// <param name="nLastWidth">Last stored width</param>
    /// <returns>True on success, false otherwise</returns>
    private bool AssignWidth(int nLastWidth)
    {
        bool result = false;
        if (CurrentForm.MinimumSize.Width <= nLastWidth && nLastWidth < SystemInformation.PrimaryMonitorSize.Width)
        {
            CurrentForm.Width = nLastWidth;
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Implementation helper called by InitializeLastHeight.
    /// </summary>
    /// <param name="nLastHeight">Last stored height</param>
    /// <returns>True on success, false otherwise</returns>
    private bool AssignHeight(int nLastHeight)
    {
        bool result = false;
        if (CurrentForm.MinimumSize.Width <= nLastHeight && nLastHeight < SystemInformation.PrimaryMonitorSize.Height)
        {
            CurrentForm.Height = nLastHeight;
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Implementation helper.
    /// </summary>
    /// <param name="sz">Size with size :-)</param>
    /// <returns>True on success, false otherwise</returns>
    private bool AssignSize(Size sz)
    {
        bool bCxOk, bCyOk, result;

        bCxOk = CurrentForm.MinimumSize.Width <= sz.Width && sz.Width < SystemInformation.PrimaryMonitorSize.Width;
        bCyOk = CurrentForm.MinimumSize.Height <= sz.Height && sz.Height < SystemInformation.PrimaryMonitorSize.Height;
        if (result = bCxOk && bCyOk)
        {
            CurrentForm.Size = sz;
        }
        return result;
    }

    /// <summary>
    /// Implementation helper.
    /// </summary>
    /// <param name="pt">Point with location</param>
    /// <returns>True on success, false otherwise</returns>
    private bool AssignLocation(Point pt)
    {
        bool bCxOk, bCyOk, result;

        bCxOk = 0 <= pt.X && pt.X < SystemInformation.PrimaryMonitorSize.Width;
        bCyOk = 0 <= pt.Y && pt.Y < SystemInformation.PrimaryMonitorSize.Height;
        if (result = bCxOk && bCyOk)
        {
            CurrentForm.StartPosition = FormStartPosition.Manual;
            CurrentForm.Location = pt;
        }
        return result;
    }
    #endregion // Assigns

    #region Helper methods

    /// <summary>
    /// Generates a general prefix for an item in <c>ApplicationObjStorage</c>.
    /// The resulting format follows this pattern: <c>1680x1050.frmSettings</c>.
    /// </summary>
    /// <returns>The generated name prefix.</returns>
    private string GetStorageGeneralName()
    {
        return $"{GetStorageResolutionPrefix()}.{CurrentForm.GetType()}";
    }

    /// <summary>
    /// Generates a resolution prefix based on the current screen size.
    /// The resulting format follows this pattern: <c>1680x1050</c>.
    /// </summary>
    /// <returns>The resolution prefix.</returns>
    private static string GetStorageResolutionPrefix()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0}x{1}",
            SystemInformation.PrimaryMonitorSize.Width,
            SystemInformation.PrimaryMonitorSize.Height);
    }

    #endregion // Helper methods
    #endregion // Private Methods
    #endregion // Methods
}
