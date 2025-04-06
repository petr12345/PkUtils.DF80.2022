using System;
using System.Collections.Generic;
using System.Linq;
using PK.PkUtils.UI.CollectionEditorExtending;
using PK.TestPropertyGridCustomCollectionEditor.SectionsData;


#pragma warning disable IDE0290 // Use primary constructor


namespace PK.TestPropertyGridCustomCollectionEditor.DemoClass.CollectionsValidation
{
    /// <summary> A specialized collection validator. </summary>
    public class SectionInfoCollectionValidator : ICollectionValidator<SectionInfo>
    {
        private readonly uint _totalColumns;

        /// <summary> The only constructor. </summary>
        /// <param name="totalColumns"> Total columns amount. </param>

        public SectionInfoCollectionValidator(uint totalColumns)
        {
            _totalColumns = totalColumns;
        }

        /// <summary> Gets the total number of columns. </summary>
        /// <value> The total number of columns. </value>
        public uint TotalColumns => _totalColumns;

        ///  <inheritdoc/>
        public void Validate(IEnumerable<SectionInfo> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            IReadOnlyList<SectionInfo> list = items.ToList();

            SectionInfo.ValidateAndConvert(list, TotalColumns, null);
        }
    }
}

#pragma warning restore IDE0290 // Use primary constructor