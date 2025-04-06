// Ignore Spelling: Validator
//
using System;
using System.Collections.Generic;
using System.Linq;
using PK.PkUtils.UI.CollectionEditorExtending;
using PK.TestPropertyGridCustomCollectionEditor.DemoClass.CollectionsValidation;

#pragma warning disable IDE0290 // Use primary constructor

namespace PK.TestPropertyGridCustomCollectionEditor.DemoClass
{
    /// <summary>
    /// A specialized collection editor demonstrating custom validation of a collection upon closing the CollectionForm.
    /// </summary>
    public class IncompleteLanesCollectionEditor : CustomCollectionEditor<uint>
    {
        private readonly ICollectionValidator<uint> _validator;
        private IReadOnlyList<uint> _originalItems;

        #region Constructor(s)

        /// <summary>
        /// Inherits the default constructor from the standard...
        /// </summary>
        /// <param name="type">The collection type.</param>
        public IncompleteLanesCollectionEditor(Type type) : base(type)
        {
            _validator = new IncompleteLanesCollectionValidator();
        }

        #endregion // Constructor(s)

        #region Methods

        /// <summary>
        /// Overrides the base class method to change the title of the editing form.
        /// </summary>
        /// <returns>A CollectionForm to provide as the user interface for editing the collection.</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();
            collectionForm.Text = $"Edit the set of incomplete lanes (one-based indexes)";

            collectionForm.Activated += CollectionForm_Activated;

            // Do NOT call here, since it's too soon, and "Items" property may not be initialized yet
            /* RetrieveOriginalItems(collectionForm); */

            return collectionForm;
        }

        /// <summary> Gets collection validator used for items validation.  </summary>
        /// <returns>   The collection validator, or null if none present. </returns>
        protected override ICollectionValidator<uint> GetCollectionValidator()
        {
            return _validator;
        }

        /// <summary>
        /// Overrides the base class implementtaion, to set owner collection form modified if needed.
        /// This assures even if the only change is modified list SectionBasicInfo.IncompleteLanes,
        /// still its owner collection form will be marked as dirty,
        /// and therefore the change(s) done in that form are preserved 
        /// when that owner form is closed by OK button.
        /// </summary>
        /// <param name="items"> The items to validate. Can't be null. </param>
        /// <returns>  True if validation succeeds, false if it fails. </returns>
        protected override bool ValidateItemsOnClosing(IReadOnlyList<uint> items)
        {
            bool result = base.ValidateItemsOnClosing(items);

            // If validation passed and the edited list is changed, 
            // lets set the parent CollectionForm as modified,
            // since by default ( for list property being modified ) that's is not done.
            // 
            if (result && ((_originalItems is null) || !items.SequenceEqual(_originalItems)))
            {
                MakeParentCollectionFormDirty();
            }

            return result;
        }

        private void RetrieveOriginalItems(CollectionForm collectionForm)
        {
            _originalItems = CollectionEditorUtils.GetCollectionFormItems<uint>(collectionForm);
        }

        private void MakeParentCollectionFormDirty()
        {
            CollectionForm parentCollectionForm = FindParentCollectionForm();

            if (parentCollectionForm is null)
                throw new InvalidOperationException("Unable to find parent colection form");
            else
                SetCollectionFormDirty(parentCollectionForm);
        }
        #endregion // Methods

        #region Event_handlers

        private void CollectionForm_Activated(object sender, EventArgs e)
        {
            if (_originalItems is null) // prevent calling second time on second activation
            {
                RetrieveOriginalItems((CollectionForm)sender); // if sender is not CollectionForm, something is weird
            }
        }
        #endregion // Event_handlers
    }
}
