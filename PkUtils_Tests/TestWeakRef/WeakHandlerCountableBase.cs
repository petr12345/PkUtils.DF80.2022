
using PK.PkUtils.DataStructures;

namespace PK.TestWeakRef
{
    /// <summary>
    /// The Raison d'être of this class is to provide common base of all WeakEventHandler
    /// generics, so the code could easily count all existing instances of all WeakEventHandler(s).
    /// This is needed since for instance WeakEventHandler{CancelEventArgs} is NOT derived 
    /// from WeakEventHandler{EventArgs}. They are just completely different types.
    /// </summary>
    /// <remarks>
    /// The WeakEventHandler derives from WeakHandlerCountableBase here just for the testing purpose,
    /// to allow the using code to count their instances.
    /// In the release code, there is no need for such inheritance.
    /// </remarks>
    public class WeakHandlerCountableBase : CountableGeneric<WeakHandlerCountableBase>
    {
        #region Fields
        #endregion // Fields

        #region Constructor(s)
        public WeakHandlerCountableBase()
        {
        }
        #endregion // Constructor(s)

        #region Methods
        /// <summary>
        /// Overrides the virtual method of the base class.
        /// For testing (debugging) purpose only.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion // Methods
    }
}
