using System;
using System.Collections.Generic;
using System.Linq;
using PK.PkUtils.UI.CollectionEditorExtending;
using PK.TestPropertyGridCustomCollectionEditor.SectionsData;

namespace PK.TestPropertyGridCustomCollectionEditor.DemoClass.CollectionsValidation
{
    /// <summary> A specialized collection validator. </summary>
    internal class IncompleteLanesCollectionValidator : ICollectionValidator<uint>
    {
        /// <summary> The only constructor. </summary>
        public IncompleteLanesCollectionValidator()
        { }

        ///  <inheritdoc/>
        public void Validate(IEnumerable<uint> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            IReadOnlyList<uint> indexes = items.ToList();

            SectionBasicInfo.CheckLaneIndexesArePositive(indexes);
            SectionBasicInfo.CheckLaneIndexesAreNotDuplicated(indexes);
        }
    }
}
