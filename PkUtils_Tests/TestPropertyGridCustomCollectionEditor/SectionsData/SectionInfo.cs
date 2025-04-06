using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace PK.TestPropertyGridCustomCollectionEditor.SectionsData
{
    /// <summary>
    /// A Data Transfer Object (DTO), containing information about a section, including its length 
    /// ( amount of columns).
    /// </summary>
    public class SectionInfo : SectionBasicInfo, ISectionInfo, IEquatable<SectionInfo>, IEquatable<ISectionInfo>
    {
        #region Fields

        private int _length;
        private const bool _permitZeroLength = true;
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Default constructor. </summary>
        public SectionInfo() : base()
        { }

        /// <summary> Constructor accepting section name. </summary>
        /// <param name="name"> The section name. </param>
        public SectionInfo(string name) : base(name)
        { }

        /// <summary> Constructor accepting section name and column. </summary>
        /// <param name="name"> The section name. </param>
        /// <param name="column"> The column (1-based index). </param>
        public SectionInfo(string name, int column) : base(name, column)
        { }

        /// <summary> Constructor accepting section name, column, incomplete lanes. </summary>
        /// <param name="name"> The section name. </param>
        /// <param name="column"> The column (1-based index). </param>
        /// <param name="incompleteLanes"> The incomplete lanes (1 - based indexes). Can't be null, but may be empty. </param>
        public SectionInfo(string name, int column, IEnumerable<uint> incompleteLanes)
            : base(name, column, incompleteLanes)
        { }

        /// <summary> Constructor accepting section name, column, incomplete lanes and length. </summary>
        /// <param name="name"> The section name. </param>
        /// <param name="column"> The column (1-based index). </param>
        /// <param name="incompleteLanes"> The incomplete lanes (1 - based indexes). Can't be null, but may be empty. </param>
        /// <param name="length"> The section length. </param>
        public SectionInfo(string name, int column, IEnumerable<uint> incompleteLanes, int length)
            : base(name, column, incompleteLanes)
        {
            SectionLength = length;
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> Gets the section length ( amount of columns ). </summary>
        /// <value> The length of the section. </value>
        [Description("The length of the section (amount of columns).")]
        public int SectionLength
        {
            get => _length;
            set
            {
                if (!IsLengthValid(value))
                {
                    string message = PermitZeroLength ?
                        $"{nameof(SectionLength)} cannot be negative." :
                        $"{nameof(SectionLength)} must be positive.";
                    throw new ArgumentOutOfRangeException(nameof(value), value, message);
                }
                _length = ValidateLength(value);
            }
        }

        /// <summary> Gets a value indicating whether implementation permits zero length section. </summary>
        /// <value> True if permit zero length, false if not. </value>
        protected static bool PermitZeroLength { get => _permitZeroLength; }
        #endregion // Properties

        #region Methods
        #region Public Methods

        /// <summary>
        /// Validates the list of section basic information and converts it into a list of section information.
        /// </summary>
        /// <remarks> Throws the same exceptions as the other overload ValidateAndConvert. </remarks>
        /// <param name="infoList"> The list of section basic information to validate and convert. Can't be null. </param>
        /// <param name="totalColumns"> The total number of columns. </param>
        /// <returns>   A List&lt;SectionInfo&gt; </returns>
        public static List<SectionInfo> ValidateAndConvert(
            IReadOnlyList<SectionBasicInfo> infoList,
            uint totalColumns)
        {
            List<SectionInfo> sections = [];

            ValidateAndConvert(infoList, totalColumns, sections);
            return sections;
        }

        /// <summary>
        /// Validates the list of section basic information and converts it into a list of section information.
        /// </summary>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="infoList"/> is null. </exception>
        /// <exception cref="ArgumentException"> Thrown when:
        /// <list type="bullet">
        ///     <item><description>The value of <paramref name="totalColumns"/> is zero.</description>
        ///     </item>
        ///     <item><description>The value of a section's column exceeds <paramref name="totalColumns"/>.</description>
        ///     </item>
        ///     <item><description>The value of a section's column is less than the previous section's column.</description>
        ///     </item>
        /// </list> </exception>
        /// <param name="infoList"> The list of section basic information to validate and convert. Can't be null. </param>
        /// <param name="totalColumns"> The total number of columns. </param>
        /// <param name="output"> (Optional) The output list of validated and converted section information. </param>
        public static void ValidateAndConvert(
            IReadOnlyList<ISectionBasicInfo> infoList,
            uint totalColumns,
            IList<SectionInfo> output = null)
        {
            ArgumentNullException.ThrowIfNull(infoList);
            if (totalColumns == 0)
            {
                throw new ArgumentException("Cannot use sections list having zero total columns", nameof(totalColumns));
            }

            int countSections = infoList.Count;
            ISectionBasicInfo previousInfo = null;
            static string ErrorPrefix(int index, ISectionBasicInfo inf) => $"Error in {index + 1}. section '{inf.SectionName}'. ";

            output?.Clear();

            for (int zeroBasedIndex = 0; zeroBasedIndex < countSections; zeroBasedIndex++)
            {
                ISectionBasicInfo info = infoList[zeroBasedIndex];
                int column = info.SectionColumn;
                string errorMessage = null;

                if (column > totalColumns)
                {
                    errorMessage = $"{ErrorPrefix(zeroBasedIndex, info)} The value of {nameof(info.SectionColumn)} ({column}) exceeds the total columns amount ({totalColumns}).";
                }
                else if (column == totalColumns && !PermitZeroLength)
                {
                    errorMessage = $"{ErrorPrefix(zeroBasedIndex, info)} The value of '{nameof(info.SectionColumn)}' ({column}) equals the total columns amount, causing a zero-length section.";
                }
                else if (previousInfo != null && column < previousInfo.SectionColumn)
                {
                    errorMessage = $"{ErrorPrefix(zeroBasedIndex, info)} The value of '{nameof(info.SectionColumn)}' ({column}) is less than the previous section's column ({previousInfo.SectionColumn}).";
                }
                else if (previousInfo != null && !PermitZeroLength && column == previousInfo.SectionColumn)
                {
                    errorMessage = $"{ErrorPrefix(zeroBasedIndex, info)} The value of '{nameof(info.SectionColumn)}' ({column}) equals the previous section's column, causing a zero-length section.";
                }

                if (errorMessage != null)
                {
                    throw new ArgumentException(errorMessage, nameof(infoList));
                }
                else if (output != null)
                {
                    int columns = GetSectionColumnsCount(infoList, (uint)zeroBasedIndex, totalColumns);

                    // Must check for invalid length, to prevent exception thrown from SectionInfo constructor.
                    // In case of invalid length, because the value of SectionColumn of next section,
                    // the correct exception will be thrown in the next cycle iteration.
                    // 
                    if (IsLengthValid(columns))
                    {
                        output.Add(new SectionInfo(
                            name: info.SectionName,
                            column: info.SectionColumn,
                            incompleteLanes: info.IncompleteLanes,
                            length: columns));
                    }
                }
                previousInfo = info;
            }
        }

        /// <summary>
        /// Retrieves the number of columns of section specified by <paramref name="zeroBasedIndex"/>.
        /// </summary>
        /// <param name="infoList">The list of section basic information.</param>
        /// <param name="zeroBasedIndex">The zero-based index of the current section.</param>
        /// <param name="totalColumns">The total number of columns.</param>
        /// <returns>The number of columns between the current section and the next section ( or the very end for case of last section) </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when:
        /// <list type="bullet">
        ///     <item><description>The value of <paramref name="zeroBasedIndex"/> is negative.</description></item>
        ///     <item><description>The value of <paramref name="zeroBasedIndex"/> exceeds the total number of sections.</description></item>
        /// </list>
        /// </exception>
        public static int GetSectionColumnsCount(
            IReadOnlyList<ISectionBasicInfo> infoList,
            uint zeroBasedIndex,
            uint totalColumns)
        {
            if (zeroBasedIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroBasedIndex), zeroBasedIndex, $"The value of '{nameof(zeroBasedIndex)}' cannot be negative.");
            if (zeroBasedIndex >= infoList.Count)
                throw new ArgumentOutOfRangeException(nameof(zeroBasedIndex), zeroBasedIndex, $"{nameof(zeroBasedIndex)} cannot be equal or exceed current number of sections ({infoList.Count}).");

            int lastColumnIndex = infoList.Count - 1;
            int currentColumnOneBased = infoList[(int)zeroBasedIndex].SectionColumn;
            int result;

            if (zeroBasedIndex < lastColumnIndex)
            {
                result = infoList[(int)zeroBasedIndex + 1].SectionColumn - currentColumnOneBased;
            }
            else
            {
                result = (int)(totalColumns - (uint)currentColumnOneBased + 1);
            }

            return result;
        }

        /// <summary> Determines whether the specified object is equal to the current object. </summary>
        /// <param name="obj">  The object to compare with the current object. </param>
        /// <returns> true if the specified object is equal to the current object; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return (obj is SectionInfo other) && this.Equals(other);
        }

        /// <summary> Serves as the default hash function. </summary>
        /// <returns>   A hash code for the current object. </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), this.SectionLength);
        }

        /// <summary> Returns a string that represents the current object. </summary>
        /// <returns>   A string that represents the current object. </returns>
        public override string ToString()
        {
            return $"'{SectionName}' at column {SectionColumn}, Length = {SectionLength}";
        }
        #endregion // Public Methods

        #region Protected Methods

        /// <summary> Queries if a column length is valid. </summary>
        /// <param name="value"> The length of a column. </param>
        /// <returns>   True if the length is valid, false if not. </returns>
        protected internal static bool IsLengthValid(int value)
        {
            return PermitZeroLength ? value >= 0 : value > 0;
        }

        /// <summary> Validates the column length described by value. </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when length is not valid. </exception>
        /// <param name="value"> The length of a column. </param>
        /// <param name="argName"> (Optional) Name of the argument. </param>
        /// <returns>   The original value when valid. </returns>
        protected internal static int ValidateLength(
            int value,
            [CallerArgumentExpression(nameof(value))] string argName = null)
        {
            if (!IsLengthValid(value))
            {
                string message = PermitZeroLength ?
                    $"{nameof(SectionLength)} cannot be negative." :
                    $"{nameof(SectionLength)} must be positive.";
                throw new ArgumentOutOfRangeException(argName, value, message);
            }

            return value;
        }

        /// <summary> Member-wise comparison with other instance. </summary>
        /// <param name="other"> The other instance. </param>
        /// <returns>   True if it succeeds, false if it fails. </returns>
        protected internal bool MemberWiseEquals(ISectionInfo other)
        {
            ArgumentNullException.ThrowIfNull(other);
            bool result;

            if (result = base.MemberWiseEquals(other))
            {
                result = (this.SectionLength == other.SectionLength);
            }

            return result;
        }
        #endregion // Protected Methods
        #endregion // Methods

        #region IEquatable<SectionInfo> members

        /// <inheritdoc/>
        public bool Equals(SectionInfo other)
        {
            return Equals(other as ISectionInfo);
        }
        #endregion // IEquatable<SectionInfo> members

        #region IEquatable<ISectionInfo> members

        /// <inheritdoc/>
        public bool Equals(ISectionInfo other)
        {
            bool result;

            if (other is null)
                result = false;
            else if (ReferenceEquals(this, other))
                result = true;
            else
                result = this.MemberWiseEquals(other);

            return result;
        }

        #endregion // IEquatable<SectionInfo> members
    }
}
