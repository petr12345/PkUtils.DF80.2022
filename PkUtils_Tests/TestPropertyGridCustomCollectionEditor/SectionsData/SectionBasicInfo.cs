// Ignore Spelling: rhs
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.TestPropertyGridCustomCollectionEditor.DemoClass;

namespace PK.TestPropertyGridCustomCollectionEditor.SectionsData
{
    /// <summary> A Data Transfer Object (DTO), containing information about a section name and column. </summary>
    /// <remarks> Note, the information here is not complete; to determine the section length, 
    ///           one has to know the column of the next section or in case of the last section total amount of columns.
    /// </remarks>
    public class SectionBasicInfo : ISectionBasicInfo, IEquatable<ISectionBasicInfo>, IEquatable<SectionBasicInfo>, IDeepCloneable<SectionBasicInfo>
    {
        #region Fields

        /// <summary> (Immutable) The minimum lane index, is used to validate IncompleteLanes. Lane indexes in User Interface are 1-based. </summary>
        private const int _minimumLaneIndex = 1;
        /// <summary> (Immutable) The default and minimum section column. Section columns in User Interface are 1-based. </summary>
        private const int _minimumColumn = 1;
        private const string _defaultSectionName = "Station #";

        private int _column = _minimumColumn;
        private string _name = _defaultSectionName;
        /// <summary> Sorted list of 1-based indexes of incomplete lanes (if any). </summary>
        private List<uint> _incompleteLanes = [];
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Default constructor. </summary>
        public SectionBasicInfo()
        { }

        /// <summary> Constructor accepting section name. </summary>
        /// <param name="name"> The section name. </param>
        /// <exception cref="ArgumentException"> Thrown when section name is null, empty or whitespace. </exception>
        public SectionBasicInfo(string name) : this(name, _minimumColumn)
        { }

        /// <summary> Constructor accepting section name and column. </summary>
        /// <param name="name"> The section name. </param>
        /// <param name="column"> The column (1-based index). </param>
        /// <exception cref="ArgumentException"> Thrown when section name is null, empty or whitespace. </exception>
        public SectionBasicInfo(string name, int column)
        {
            SectionName = name;
            SectionColumn = column;
        }

        /// <summary> Constructor accepting section name, column and incomplete lanes indexes. </summary>
        /// <param name="name"> The name. </param>
        /// <param name="column"> The column (1-based index). </param>
        /// <param name="incompleteLanes"> The incomplete lanes (1 - based indexes). Can't be null, but may be empty. </param>
        /// <exception cref="ArgumentException"> Thrown when section name is null, empty or whitespace. </exception>
        public SectionBasicInfo(string name, int column, IEnumerable<uint> incompleteLanes)
            : this(name, column)
        {
            ArgumentNullException.ThrowIfNull(incompleteLanes);
            IncompleteLanes = incompleteLanes.ToList();
        }

        /// <summary> A copy-constructor. </summary>
        /// <param name="rhs"> The right hand side SectionBasicInfo. Can't be null. </param>
        public SectionBasicInfo(ISectionBasicInfo rhs)
        {
            ArgumentNullException.ThrowIfNull(rhs);
            SectionName = rhs.SectionName;
            SectionColumn = rhs.SectionColumn;
            IncompleteLanes = rhs.IncompleteLanes;  // making a deep copy of rhs.IncompleteLanes
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> The name of the section. </summary>
        /// <exception cref="ArgumentException"> Thrown when section name is null, empty or whitespace. </exception>
        /// <value> The name of the section. </value>
        [Description("The name of the section.")]
        [DefaultValue(_defaultSectionName)]
        public string SectionName
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(SectionName)} cannot be empty or whitespace.", nameof(value));
                }
                _name = value;
            }
        }

        /// <summary>
        /// Represents the starting column index of the section (1-based).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than the minimum column.</exception>
        /// <value>The starting column index of the section (1-based).</value>
        [Description("The starting column index of the section (1-based).")]
        public int SectionColumn
        {
            get => _column;
            set
            {
                if (value < _minimumColumn)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value), value, $"{nameof(SectionColumn)} must be at least {_minimumColumn}.");
                }
                _column = value;
            }
        }

        /// <inheritdoc/>
        [Editor(typeof(IncompleteLanesCollectionEditor), typeof(UITypeEditor))]
        [Description("The sorted list of 1-based indexes of incomplete lanes (if any)")]
        public List<uint> IncompleteLanes
        {
            get { return _incompleteLanes; }
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                List<uint> indexes = CheckLaneIndexesArePositive(value);
                List<uint> sorted = CheckLaneIndexesAreNotDuplicated(indexes);

                _incompleteLanes = sorted;
            }
        }
        #endregion // Properties

        #region Methods

        /// <summary> Check lane indexes are positive. </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when any line index is less then <see cref="_minimumLaneIndex"/></exception>
        /// <param name="indexes"> The list of indexed. </param>
        /// <param name="argName"> (Optional) Name of the argument. </param>
        /// <returns>   If succeeds, returns a list of indexes from <paramref name="indexes"/> </returns>
        public static List<uint> CheckLaneIndexesArePositive(
            IEnumerable<uint> indexes,
            [CallerArgumentExpression(nameof(indexes))] string argName = "indexes")
        {
            foreach (uint laneIndex in indexes)
            {
                if (laneIndex < _minimumLaneIndex)
                {
                    throw new ArgumentOutOfRangeException(
                        argName, laneIndex, $"A value '{laneIndex}' is invalid. The value of lane index must be at least {_minimumLaneIndex}.");
                }
            }

            return indexes.ToList();
        }

        /// <summary> Check that lane indexes are not duplicated, and if valid, sorts values. </summary>
        /// <exception cref="ArgumentException"> Thrown when <paramref name="indexes"/> contains duplicities. </exception>
        /// <param name="indexes"> The value. </param>
        /// <param name="argName"> (Optional) Name of the argument. </param>
        /// <returns>  If succeeds, returns a sorted list of indexes from <paramref name="indexes"/> </returns>
        public static List<uint> CheckLaneIndexesAreNotDuplicated(
            IEnumerable<uint> indexes,
            [CallerArgumentExpression(nameof(indexes))] string argName = "indexes")
        {
            ArgumentNullException.ThrowIfNull(indexes);
            return indexes.CheckNotDuplicated(argName).Order().ToList();
        }

        /// <summary>
        /// Format section name to a string displayed by its TextVisual. This means, if <paramref name="sectionName"/>
        /// contains '#' character, not followed by another '#', 
        /// it will be replaced by oneBaseIndex.ToString() or by fnIndexToString(oneBaseIndex) if provided.
        /// A pair of consecutive '#' characters will be replaced by a single '#'.
        /// </summary>
        /// <remarks> Note that _defaultSectionName = "Section #" </remarks>
        /// <param name="sectionName"> The name of the section. </param>
        /// <param name="oneBaseIndex"> One-based index of the section. </param>
        /// <param name="fnIndexToString"> (Optional) The function converting index to string. </param>
        /// <returns> The formatted section name to display. </returns>
        public static string FormatForSectionTextVisual(
            string sectionName,
            int oneBaseIndex,
            Func<int, string> fnIndexToString = null)
        {
            string result;

            if (string.IsNullOrEmpty(sectionName) || !sectionName.Contains('#', StringComparison.CurrentCulture))
            {
                result = sectionName;
            }
            else
            {
                string indexString = (fnIndexToString is not null)
                    ? fnIndexToString(oneBaseIndex)
                    : oneBaseIndex.ToString(CultureInfo.CurrentCulture);
                result = sectionName.Split("##").Select(x => x.Replace("#", indexString, StringComparison.CurrentCulture)).Join("#");
            }

            return result;
        }

        /// <summary> Determines whether the specified object is equal to the current object. </summary>
        /// <param name="obj">  The object to compare with the current object. </param>
        /// <returns> true if the specified object is equal to the current object; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return (obj is SectionBasicInfo other) && this.Equals(other);
        }

        /// <summary> Serves as the default hash function. </summary>
        /// <returns> A hash code for the current object. </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.SectionName, this.SectionColumn, this.IncompleteLanes.SequenceHashCode());
        }

        /// <summary> Returns a string that represents the current object. </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return $"\"{SectionName}\" at column {SectionColumn}";
        }

        /// <summary> Member-wise comparison with other instance. </summary>
        /// <param name="other"> The other instance. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        protected internal bool MemberWiseEquals(ISectionBasicInfo other)
        {
            ArgumentNullException.ThrowIfNull(other);

            bool result;

            if (this.SectionName != other.SectionName)
                result = false;
            else if (this.SectionColumn != other.SectionColumn)
                result = false;
            else if (!this.IncompleteLanes.SequenceEqual(other.IncompleteLanes))
                result = false;
            else
                result = true;

            return result;
        }
        #endregion // Methods

        #region IEquatable<ISectionBasicInfo> members

        /// <inheritdoc/>
        public bool Equals(ISectionBasicInfo other)
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
        #endregion // IEquatable<ISectionBasicInfo> members

        #region IEquatable<SectionBasicInfo> members

        /// <inheritdoc/>
        public bool Equals(SectionBasicInfo other)
        {
            return Equals(other as ISectionBasicInfo);
        }
        #endregion // IEquatable<SectionBasicInfo> members

        #region IDeepCloneable members

        /// <inheritdoc/>
        public SectionBasicInfo DeepClone() => new(this);

        /// <inheritdoc/>
        object IDeepCloneable.DeepClone() => DeepClone();

        #endregion  // IDeepCloneable members
    }

}
