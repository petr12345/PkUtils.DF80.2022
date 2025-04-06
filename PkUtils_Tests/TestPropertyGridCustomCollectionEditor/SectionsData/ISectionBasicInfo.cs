using System;
using System.Collections.Generic;

namespace PK.TestPropertyGridCustomCollectionEditor.SectionsData
{
    /// <summary> Interface for section basic information ( excluding section length ). </summary>
    public interface ISectionBasicInfo : IEquatable<ISectionBasicInfo>
    {
        /// <summary> Gets the name of the section. </summary>
        string SectionName { get; }

        /// <summary> Gets the section column. Columns are 1-based; i.e. the first column in the first section has index 1. </summary>
        int SectionColumn { get; }

        /// <summary>
        /// Gets the set of "incomplete" lanes in this station. 
        /// Incomplete stations are stations where there is at least one incomplete lane.
        /// </summary>
        /// <remark> The setter should throw <see cref="ArgumentException"/> in case the provided value contains duplicities. </remark>
        List<uint> IncompleteLanes { get; }
    }

    /// <summary> Contains utilities concerning <see cref="ISectionBasicInfo"/>. </summary>
    public static class SectionBasicInfoExtensions
    {
        /// <summary> Query if '@this' contains any incomplete lane. </summary>
        /// <param name="this"> this. </param>
        /// <returns>   True if incomplete, false if not. </returns>
        public static bool IsIncomplete(ISectionBasicInfo @this)
        {
            ArgumentNullException.ThrowIfNull(@this);
            return (@this.IncompleteLanes.Count > 0);
        }
    }
}
