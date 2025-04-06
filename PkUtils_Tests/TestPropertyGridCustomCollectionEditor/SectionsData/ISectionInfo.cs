using System;

namespace PK.TestPropertyGridCustomCollectionEditor.SectionsData
{
    /// <summary> Interface for section information. </summary>
    public interface ISectionInfo : ISectionBasicInfo, IEquatable<ISectionInfo>
    {
        /// <summary> Gets the section length ( amount of columns ). </summary>
        int SectionLength { get; }
    }

    /// <summary> Defines extensions of <see cref="ISectionInfo"/>. </summary>
    public static class SectionInfoExtensions
    {
        /// <summary> An ISectionInfo extension method that query if '@this' is empty. </summary>
        /// <param name="this"> The ISectionInfo to act on. Can't be null. </param>
        /// <returns>  True if empty, false if not. </returns>
        public static bool IsEmpty(this ISectionInfo @this)
        {
            ArgumentNullException.ThrowIfNull(@this);

            return (@this.SectionLength == 0);
        }
    }
}
