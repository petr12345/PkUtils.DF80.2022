using System;
using System.Collections.Generic;
using System.Linq;
using PK.PkUtils.UI.CollectionEditorExtending;
using PK.TestPropertyGridCustomCollectionEditor.SectionsData;


#pragma warning disable IDE0290 // Use primary constructor


namespace PK.TestPropertyGridCustomCollectionEditor.DemoClass.CollectionsValidation
{
    /// <summary> A specialized collection validator. </summary>
    public class SectionBasicInfoCollectionValidator : ICollectionValidator<SectionBasicInfo>
    {
        private readonly uint _totalColumns;

        /// <summary> The only constructor. </summary>
        /// <param name="totalColumns"> Total columns amount. </param>

        public SectionBasicInfoCollectionValidator(uint totalColumns)
        {
            _totalColumns = totalColumns;
        }

        ///  <inheritdoc/>
        public void Validate(IEnumerable<SectionBasicInfo> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            IReadOnlyList<SectionBasicInfo> list = items.ToList();

            SectionInfo.ValidateAndConvert(list, _totalColumns, null);
        }
    }
}

#pragma warning restore IDE0290 // Use primary constructor