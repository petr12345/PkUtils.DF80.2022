using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PK.TestTaskDialog;

/// <summary>Specifies how values are shortened when the measured table exceeds its width limit.</summary>
internal enum MeasuredTextTruncationMode
{
    /// <summary>Preserve all values, even when the resulting TaskDialog text may wrap.</summary>
    None,
    /// <summary>Shorten overflowing values at the end, retaining the beginning.</summary>
    TruncateEnd,
    /// <summary>Keep the distinguishing endings when values share a substantial prefix; otherwise keep their beginnings.</summary>
    Smart
}

/// <summary>
/// Experimental TaskDialog expander formatter. Uses ordinary measured spaces to align columns;
/// optionally shortens cells so the full three-column table fits an approximate pixel budget.
/// </summary>
internal static class MeasuredSpaceTableFormatter
{
    #region Typedefs

    /// <summary>Contains formatted expander text, measurement diagnostics, and the unmodified input values.</summary>
    internal sealed class FormatResult
    {
        #region Properties

        /// <summary>Gets the formatted comparison text for the TaskDialog expander.</summary>
        public string Text { get; }

        /// <summary>Gets details of calculated widths, truncation decisions, and alignment errors.</summary>
        public string Diagnostics { get; }

        /// <summary>Gets the original values for reference or clipboard export.</summary>
        public string FullValues { get; }
        #endregion // Properties

        #region Constructor(s)

        /// <summary>Initializes a formatting result.</summary>
        /// <param name="text">The text to display in the expanded TaskDialog section.</param>
        /// <param name="diagnostics">Measurement and truncation diagnostics.</param>
        /// <param name="fullValues">The complete original values, without truncation.</param>
        public FormatResult(string text, string diagnostics, string fullValues)
        {
            Text = text;
            Diagnostics = diagnostics;
            FullValues = fullValues;
        }
        #endregion // Constructor(s)
    }
    #endregion // Typedefs

    #region Fields

    private static readonly TextFormatFlags MeasureFlags =
        TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
    #endregion // Fields

    #region Methods

    #region Public_Methods

    /// <summary>
    /// Formats a three-column comparison for TaskDialog expanded information using pixel-measured
    /// ordinary spaces; optionally truncates overflowing values to stay within a width budget.
    /// </summary>
    /// <param name="graphics">Graphics context used for text measurement and DPI.</param>
    /// <param name="font">Font approximating the native TaskDialog expanded-information font.</param>
    /// <param name="rows">Setting names and the corresponding Ribbon and Project values.</param>
    /// <param name="settingHeader">Heading for the setting-name column.</param>
    /// <param name="ribbonHeader">Heading for the Ribbon-value column.</param>
    /// <param name="projectHeader">Heading for the Project-value column.</param>
    /// <param name="columnGapPixels">Requested horizontal gap between columns, in pixels.</param>
    /// <param name="maxTableWidthPixels">Approximate maximum width of the expanded text, in pixels.</param>
    /// <param name="truncationMode">Strategy for shortening values if the full table does not fit.</param>
    /// <returns>Formatted text together with diagnostics and original values.</returns>
    /// <remarks>TextRenderer measurements approximate DirectUI rendering; native TaskDialog wrapping may differ.</remarks>
    public static FormatResult Format(
        Graphics graphics,
        Font font,
        IReadOnlyList<(string Setting, string Ribbon, string Project)> rows,
        string settingHeader,
        string ribbonHeader,
        string projectHeader,
        int columnGapPixels = 20,
        int maxTableWidthPixels = 420,
        MeasuredTextTruncationMode truncationMode = MeasuredTextTruncationMode.Smart)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(font);
        ArgumentNullException.ThrowIfNull(rows);
        if (columnGapPixels < 0)
            throw new ArgumentOutOfRangeException(nameof(columnGapPixels));
        if (maxTableWidthPixels <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxTableWidthPixels));

        var original = new List<(string Setting, string Ribbon, string Project)>
        {
            (settingHeader ?? "", ribbonHeader ?? "", projectHeader ?? "")
        };
        original.AddRange(rows.Select(r => (r.Setting ?? "", r.Ribbon ?? "", r.Project ?? "")));

        // Do not change any cell when the complete, untruncated table fits the requested width.
        int settingWidth = original.Max(r => Width(graphics, font, r.Setting));
        int ribbonWidth = original.Max(r => Width(graphics, font, r.Ribbon));
        int projectWidth = original.Max(r => Width(graphics, font, r.Project));
        int originalWidth = checked(settingWidth + ribbonWidth + projectWidth + 2 * columnGapPixels);
        bool overflow = originalWidth > maxTableWidthPixels;
        var formatted = new List<(string Setting, string Ribbon, string Project)>(original);
        var diagnostics = new StringBuilder();
        diagnostics.AppendLine($"Mode: {truncationMode}; table limit: {maxTableWidthPixels}px; gap: {columnGapPixels}px");
        diagnostics.AppendLine($"Original measured table width: {originalWidth}px; fits: {!overflow}");

        if (overflow && truncationMode != MeasuredTextTruncationMode.None)
        {
            // Keep setting labels and both column headers intact whenever feasible.
            // Reserve a few pixels for whole-space rounding in the DirectUI approximation.
            const int safetyPixels = 12;
            int available = maxTableWidthPixels - settingWidth - 2 * columnGapPixels - safetyPixels;
            int minRibbon = Width(graphics, font, original[0].Ribbon);
            int minProject = Width(graphics, font, original[0].Project);
            if (available < minRibbon + minProject)
            {
                diagnostics.AppendLine("Width limit too small to preserve setting labels and headers; unable to guarantee fit.");
            }
            else
            {
                // Allocate the limited space fairly: preserve the shorter column in full before
                // sacrificing it to accommodate a much longer value in the other column.
                int budgetRibbon = minRibbon;
                int budgetProject = minProject;
                int remaining = available - budgetRibbon - budgetProject;
                while (remaining > 0 && (budgetRibbon < ribbonWidth || budgetProject < projectWidth))
                {
                    bool growRibbon = budgetRibbon < ribbonWidth &&
                        (budgetProject >= projectWidth || budgetRibbon <= budgetProject);
                    if (growRibbon) budgetRibbon++;
                    else budgetProject++;
                    remaining--;
                }
                diagnostics.AppendLine($"Allocated cell widths: Ribbon={budgetRibbon}px; Project={budgetProject}px");
                for (int i = 1; i < formatted.Count; i++)
                {
                    var old = original[i];
                    bool preferSuffix = truncationMode == MeasuredTextTruncationMode.Smart &&
                        HasMeaningfulCommonPrefix(old.Ribbon, old.Project);
                    string ribbon = TruncateToWidth(graphics, font, old.Ribbon, budgetRibbon, preferSuffix);
                    string project = TruncateToWidth(graphics, font, old.Project, budgetProject, preferSuffix);
                    formatted[i] = (old.Setting, ribbon, project);
                    if (ribbon != old.Ribbon || project != old.Project)
                    {
                        diagnostics.AppendLine($"Row {i} ({old.Setting}): " +
                            $"{(preferSuffix ? "shared-prefix / keep endings" : "keep beginnings")}; " +
                            $"Ribbon [{old.Ribbon}] => [{ribbon}], Project [{old.Project}] => [{project}]");
                    }
                }
            }
        }
        else if (overflow)
        {
            diagnostics.AppendLine("Overflow retained because mode=None; Windows may wrap the expanded text.");
        }

        int finalSettingWidth = formatted.Max(r => Width(graphics, font, r.Setting));
        int finalRibbonWidth = formatted.Max(r => Width(graphics, font, r.Ribbon));
        int finalProjectWidth = formatted.Max(r => Width(graphics, font, r.Project));
        int finalWidth = checked(finalSettingWidth + finalRibbonWidth + finalProjectWidth + 2 * columnGapPixels);
        int targetRibbon = checked(finalSettingWidth + columnGapPixels);
        int targetProject = checked(targetRibbon + finalRibbonWidth + columnGapPixels);
        diagnostics.AppendLine($"Final cell-width estimate: {finalWidth}px; Ribbon x~{targetRibbon}px; Project x~{targetProject}px");
        var text = new StringBuilder();
        int maximumActualRowWidth = 0;
        for (int i = 0; i < formatted.Count; i++)
        {
            var (setting, ribbon, project) = formatted[i];
            string first = PadToTarget(graphics, font, setting, targetRibbon);
            int actualRibbon = Width(graphics, font, first);
            string second = PadToTarget(graphics, font, first + ribbon, targetProject);
            int actualProject = Width(graphics, font, second);
            string resultLine = second + project;
            text.Append(resultLine);
            if (i + 1 < formatted.Count) text.AppendLine();
            int actualWidth = Width(graphics, font, resultLine);
            maximumActualRowWidth = Math.Max(maximumActualRowWidth, actualWidth);
            diagnostics.AppendLine($"Row {i}: Ribbon x~{actualRibbon}px (delta {actualRibbon - targetRibbon:+#;-#;0}), " +
                $"Project x~{actualProject}px (delta {actualProject - targetProject:+#;-#;0}); row width {actualWidth}px");
        }
        diagnostics.AppendLine($"Maximum measured output line: {maximumActualRowWidth}px / limit {maxTableWidthPixels}px");
        if (maximumActualRowWidth > maxTableWidthPixels)
            diagnostics.AppendLine("WARNING: output exceeds limit; reduce maximum width, gap or label lengths.");
        diagnostics.AppendLine("Positions are TextRenderer estimates, not measured DirectUI positions. Dialog may wrap at a different width.");

        var fullValues = new StringBuilder();
        for (int i = 1; i < original.Count; i++)
        {
            var row = original[i];
            fullValues.AppendLine($"{row.Setting}: Ribbon=[{row.Ribbon}], Project=[{row.Project}]");
        }

        return new FormatResult(text.ToString(), diagnostics.ToString(), fullValues.ToString());
    }
    #endregion // Public_Methods

    #region Private_Methods

    /// <summary>Measures the displayed width of a single-line string using the configured text-rendering flags.</summary>
    /// <param name="graphics">Graphics context for measurement.</param>
    /// <param name="font">Font used to measure the text.</param>
    /// <param name="text">Text to measure.</param>
    /// <returns>Measured width in pixels.</returns>
    private static int Width(Graphics graphics, Font font, string text)
    {
        return TextRenderer.MeasureText(graphics, text, font,
            new Size(int.MaxValue, int.MaxValue), MeasureFlags).Width;
    }

    /// <summary>Checks whether two values share a sufficiently long initial sequence of Unicode text elements.</summary>
    /// <param name="a">First value to compare.</param>
    /// <param name="b">Second value to compare.</param>
    /// <returns>True when keeping the distinct endings is likely to make the comparison clearer.</returns>
    private static bool HasMeaningfulCommonPrefix(string a, string b)
    {
        // Compare Unicode text elements, avoiding splitting surrogate pairs or combining sequences.
        var left = Elements(a);
        var right = Elements(b);
        int count = 0;
        while (count < left.Count && count < right.Count &&
               string.Equals(left[count], right[count], StringComparison.OrdinalIgnoreCase))
            count++;
        return count >= 8 && count * 3 >= Math.Min(left.Count, right.Count);
    }

    /// <summary>Splits a string into Unicode text elements so truncation does not split combining sequences.</summary>
    /// <param name="value">The string to separate into text elements.</param>
    /// <returns>The ordered list of text elements.</returns>
    private static List<string> Elements(string value)
    {
        var elements = new List<string>();
        TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(value);
        while (enumerator.MoveNext())
            elements.Add(enumerator.GetTextElement());
        return elements;
    }

    /// <summary>Truncates a value to its pixel budget, keeping either the beginning or the end.</summary>
    /// <param name="graphics">Graphics context for measurement.</param>
    /// <param name="font">Font used to measure the text.</param>
    /// <param name="value">Original cell value.</param>
    /// <param name="maxPixels">Maximum measured width, in pixels.</param>
    /// <param name="keepEnding">True to preserve the suffix; false to preserve the prefix.</param>
    /// <returns>The original or shortened value, marked with an ellipsis when truncation is possible.</returns>
    private static string TruncateToWidth(Graphics graphics, Font font, string value,
        int maxPixels, bool keepEnding)
    {
        if (Width(graphics, font, value) <= maxPixels) return value;
        const string ellipsis = "…";
        if (Width(graphics, font, ellipsis) > maxPixels) return string.Empty;
        var elements = Elements(value);
        int low = 0;
        int high = elements.Count;
        string best = ellipsis;
        while (low <= high)
        {
            int n = low + (high - low) / 2;
            string part = keepEnding
                ? string.Concat(elements.Skip(elements.Count - n))
                : string.Concat(elements.Take(n));
            string candidate = keepEnding ? ellipsis + part : part + ellipsis;
            if (Width(graphics, font, candidate) <= maxPixels)
            {
                best = candidate;
                low = n + 1;
            }
            else high = n - 1;
        }
        return best;
    }

    /// <summary>Adds measured ordinary spaces to bring the next column near a target pixel position.</summary>
    /// <param name="graphics">Graphics context for measurement.</param>
    /// <param name="font">Font used to measure the text.</param>
    /// <param name="prefix">Text already present on the output line.</param>
    /// <param name="targetPixelWidth">Desired pixel position for the following column.</param>
    /// <returns>Input text with the number of spaces giving the smallest measured alignment error.</returns>
    private static string PadToTarget(Graphics graphics, Font font, string prefix, int targetPixelWidth)
    {
        var candidate = new StringBuilder(prefix);
        int current = Width(graphics, font, prefix);
        if (current >= targetPixelWidth) return prefix + " ";
        string best = prefix;
        int bestError = Math.Abs(current - targetPixelWidth);
        for (int spaces = 0; spaces < 1024; spaces++)
        {
            candidate.Append(' ');
            int width = Width(graphics, font, candidate.ToString());
            int error = Math.Abs(width - targetPixelWidth);
            if (error <= bestError)
            {
                bestError = error;
                best = candidate.ToString();
            }
            if (width >= targetPixelWidth && error > bestError) break;
        }

        return best;
    }
    #endregion // Private_Methods
    #endregion // Methods
}
