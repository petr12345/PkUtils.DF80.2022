using System;
using PK.PkUtils.UI.CollectionEditorExtending;
using PK.TestPropertyGridCustomCollectionEditor.DemoClass.CollectionsValidation;
using PK.TestPropertyGridCustomCollectionEditor.SectionsData;


#pragma warning disable IDE0290 // Use primary constructor

namespace PK.TestPropertyGridCustomCollectionEditor.DemoClass
{
    /// <summary>
    /// A specialized collection editor demonstrating custom validation of a collection upon closing the CollectionForm.
    /// </summary>
    public class SectionBasicInfoCollectionEditor : CustomCollectionEditor<SectionBasicInfo>
    {
        private const int _totalColumns = 24;
        private readonly ICollectionValidator<SectionBasicInfo> _validator;

        #region Constructor(s)

        /// <summary>
        /// Inherits the default constructor from the standard...
        /// </summary>
        /// <param name="type">The collection type.</param>
        public SectionBasicInfoCollectionEditor(Type type) : base(type)
        {
            _validator = new SectionBasicInfoCollectionValidator(_totalColumns);
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
            collectionForm.Text = $"Edit sections list, limited by {_totalColumns} total columns maximum";
            return collectionForm;
        }

        protected override ICollectionValidator<SectionBasicInfo> GetCollectionValidator()
        {
            return _validator;
        }
        #endregion // Methods
    }
}

#pragma warning restore IDE0290 // Use primary constructor
