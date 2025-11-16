// Ignore Spelling: Ctrl, TreeView, treeview, unselects, Bg, Fg
//
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

#pragma warning disable IDE0290     // Use primary constructor


namespace PK.PkUtils.UI.Controls;

public partial class MultiSelectTreeView : TreeView
{
    #region Typedefs

    /// <summary>
    /// Provides read-only access to cached background and foreground colors for tree nodes.
    /// </summary>
    public interface IColorsCache
    {
        /// <summary>
        /// Gets a value indicating whether the cache has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the cached background color.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the cache has not been initialized.</exception>
        Color BgColor { get; }

        /// <summary>
        /// Gets the cached foreground color.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the cache has not been initialized.</exception>
        Color FgColor { get; }
    }

    /// <summary>
    /// An auxiliary class used by <see cref="MultiSelectTreeView"/>.
    /// Caches the background and foreground colors used for tree nodes when visual styles are enabled.
    /// </summary>
    protected class ColorsCache : IColorsCache
    {
        #region Fields

        private Nullable<Color> _bgColor;
        private Nullable<Color> _fgColor;
        private readonly IDumper _dumper;
        #endregion // Fields

        /// <summary>   Initializes a new instance of the <see cref="ColorsCache"/> class. </summary>
        /// <param name="dumper"> The dumper provided by caller, may be null. </param>
        public ColorsCache(IDumper dumper)
        {
            _dumper = dumper;
        }
        #region Properties

        /// <summary> Gets a value indicating whether the cache has been initialized. </summary>
        /// <value> trueif both background and foreground colors have been set; otherwise, false. </value>
        public bool IsInitialized => _bgColor.HasValue && _fgColor.HasValue;

        /// <summary> Gets the cached background color. </summary>
        /// <exception cref="InvalidOperationException"> Thrown if the cache has not been initialized. </exception>
        public Color BgColor { get { CheckIsInitialized(); return _bgColor.Value; } }

        /// <summary> Gets the cached foreground color. </summary>
        /// <exception cref="InvalidOperationException"> Thrown if the cache has not been initialized. </exception>
        public Color FgColor { get { CheckIsInitialized(); return _fgColor.Value; } }

        /// <summary> Gets the dumper provided to constructor ( if any ). </summary>
        protected IDumper Dumper { get => _dumper; }
        #endregion // Properties

        #region Methods
        #region Public Methods

        /// <summary> Initializes the cache with system-defined colors. </summary>
        /// <param name="enforceRefresh"> If set to <c>true</c>, forces the cache to refresh even if already initialized. </param>/// 
        public void Initialize(bool enforceRefresh = false)
        {
            InitColorsFromSystem(enforceRefresh);
        }

        /// <summary>
        /// Attempts to initialize the cache with colors retrieved from the specified <paramref name="treeView"/>.
        /// </summary>
        /// <param name="treeView"> The tree view from which it will attempt to retrieve colors. Can't be null. </param>
        /// <param name="enforceRefresh"> If set to true, forces the cache to refresh even if already initialized. </param>/// 
        /// <returns> true if colors were successfully retrieved; otherwise, false. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if <paramref name="treeView"/> is null. </exception>
        public bool InitializeFromTreeNodes(MultiSelectTreeView treeView, bool enforceRefresh = false)
        {
            ArgumentNullException.ThrowIfNull(treeView);
            return InitColorsFromNodes(treeView, enforceRefresh);
        }

        /// <summary> Forces a refresh of the cached colors from the system. </summary>
        public void Refresh()
        {
            InitColorsFromSystem(enforceRefresh: true);
        }
        #endregion // Public Methods

        #region Protected Methods

        /// <summary> Ensures that the cache has been initialized before accessing its values. </summary>
        /// <exception cref="InvalidOperationException"> Thrown if the cache has not been initialized. </exception>
        protected void CheckIsInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException($"{GetType().Name} is not initialized");
        }

        /// <summary> Initializes the cache with system colors, optionally forcing a refresh. </summary>
        /// <param name="enforceRefresh"> If set to <c>true</c>, forces the cache to refresh even if already initialized. </param>
        protected internal void InitColorsFromSystem(bool enforceRefresh = false)
        {
            if (IsInitialized && !enforceRefresh) return;

            Color bgColor, fgColor;

            if (VisualStyleRenderer.IsSupported)
            {
                if (!TryGetThemedColorForTreeView(out bgColor, out fgColor) &&
                    !TryGetThemedColorForTextBox(out bgColor, out fgColor))
                {
                    bgColor = SystemColors.Window;
                    fgColor = SystemColors.WindowText;
                }
            }
            else
            {
                bgColor = SystemColors.Window;
                fgColor = SystemColors.WindowText;
            }

            _bgColor = bgColor;
            _fgColor = fgColor;
        }

        /// <summary>
        /// Initializes the cache with the background and foreground colors from the specified <paramref name="node"/>,
        /// optionally forcing a refresh.
        /// </summary>
        /// <remarks>
        /// - If the cache is already initialized and <paramref name="enforceRefresh"/> is false, the method returns true without modifying the cache.
        /// - If <paramref name="node"/> is not null, and its BackColor and ForeColor are not Color.Empty and not both Color.Black,
        ///   assigns its BackColor and ForeColor to the cache and returns true.
        /// - If <paramref name="node"/> is null, or its colors are Color.Empty or both Color.Black, the cache is not modified and the method returns false.
        /// </remarks>
        /// <param name="node">The <see cref="TreeNode"/> from which to retrieve colors. Can be null.</param>
        /// <param name="enforceRefresh">If set to <c>true</c>, forces the cache to refresh even if already initialized.</param>
        /// <returns>
        /// true if the cache was initialized (either already initialized or successfully set from a non-null node with valid colors);
        /// otherwise, false.
        /// </returns>
        protected internal bool InitColorsFromNode(TreeNode node, bool enforceRefresh = false)
        {
            bool result;

            if (IsInitialized && !enforceRefresh)
            {
                result = true;
            }
            else if (node is not null)
            {
                Color bg = node.BackColor;
                Color fg = node.ForeColor;

                // Only use node colors if they are not Color.Empty and not both black.
                // WinForms TreeNode.BackColor and ForeColor default to Color.Empty, which means the node is using inherited or system colors.
                // However, reading these properties when unset often returns Color.Black, which does not reflect the actual color rendered by the TreeView.
                // This is a quirk of the WinForms implementation: unless you explicitly set these properties, the control draws using system or themed colors, not the node's property values.
                // To avoid incorrect color caching, we ignore nodes whose colors are Color.Empty or both black, and instead fall back to system or themed colors.

                if (bg == Color.Empty || fg == Color.Empty || (bg == Color.Black && fg == Color.Black))
                {
                    // The caller should fallback to system or themed colors
                    result = false;
                }
                else
                {
                    _bgColor = bg;
                    _fgColor = fg;
                    result = true;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Attempts to initialize the cache with colors from a tree view's nodes,
        /// optionally forcing a refresh.
        /// </summary>
        /// <param name="treeView"> The tree view from which to retrieve colors. </param>
        /// <param name="enforceRefresh"> (Optional) If set to true, forces the cache to refresh even if already
        /// initialized. </param>
        /// <returns>
        /// True if colors were successfully retrieved; or if initialized already and not enforcing refresh;
        /// otherwise, false.
        /// </returns>
        protected internal bool InitColorsFromNodes(MultiSelectTreeView treeView, bool enforceRefresh = false)
        {
            ArgumentNullException.ThrowIfNull(treeView);

            if (IsInitialized && !enforceRefresh) return true;

            // Find not selected node to build colors from.
            // When all nodes are selected, I can't initialize the colors cache this way
            // But I should not needed colors of unselected nodes in that case.
            TreeNode node = treeView.FindNode(x => !treeView.IsSelected(x));
            return InitColorsFromNode(node, enforceRefresh);
        }
        #endregion // Protected Methods

        #region Private Methods

        /// <summary> Attempts to retrieve themed colors for a tree view using the visual style renderer. </summary>
        /// <param name="bgColor"> The retrieved background color. </param>
        /// <param name="fgColor"> The retrieved foreground color. </param>
        /// <returns> True if colors were successfully retrieved; otherwise, false. </returns>
        private bool TryGetThemedColorForTreeView(out Color bgColor, out Color fgColor)
        {
            return TryGetThemedColorsFromRenderer(VisualStyleElement.TreeView.Item.Normal, out bgColor, out fgColor);
        }

        /// <summary>
        /// Attempts to retrieve themed colors for a text box using the visual style renderer.
        /// </summary>
        /// <param name="bgColor"> The retrieved background color. </param>
        /// <param name="fgColor"> The retrieved foreground color. </param>
        /// <returns> True if colors were successfully retrieved; otherwise, false. </returns>
        private bool TryGetThemedColorForTextBox(out Color bgColor, out Color fgColor)
        {
            return TryGetThemedColorsFromRenderer(VisualStyleElement.TextBox.TextEdit.Normal, out bgColor, out fgColor);
        }

        private bool TryGetThemedColorsFromRenderer(
            VisualStyleElement element,
            out Color bgColor,
            out Color fgColor)
        {
            const string me = nameof(TryGetThemedColorsFromRenderer);
            bool result = true;

            try
            {
                var renderer = new VisualStyleRenderer(element);
                bgColor = renderer.GetColor(ColorProperty.FillColor);
                fgColor = renderer.GetColor(ColorProperty.TextColor);
            }
            catch (Exception ex)
            {
                // The call above could throw ArgumentException, saying 
                // "Given combination of Class, Part And State is not defined in current Visual style"
                // 
                Dumper?.DumpLine($"[INFO] {GetType().Name}: In {me}, {ex.GetType().Name} has occurred. ({ex.Message}).");
                bgColor = fgColor = Color.Empty;
                result = false;
            }

            return result;
        }
        #endregion // Private Methods
        #endregion // Methods
    }
    #endregion // Typedefs
}
