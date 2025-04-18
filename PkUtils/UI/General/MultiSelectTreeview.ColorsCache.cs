// Ignore Spelling: Ctrl, Treeview, treeview, unselects, Bg, Fg
//
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UI.General;

public partial class MultiSelectTreeView : TreeView
{
    #region Typedefs

    /// <summary>
    /// An auxiliary class used by <see cref="MultiSelectTreeView"/>.
    /// Caches the background and foreground colors used for tree nodes when visual styles are enabled.
    /// </summary>
    protected class ColorsCache
    {
        #region Fields

        private Nullable<Color> _bgColor;
        private Nullable<Color> _fgColor;

        #endregion // Fields

        /// <summary> Initializes a new instance of the <see cref="ColorsCache"/> class. </summary>
        public ColorsCache()
        { }

        #region Properties

        /// <summary> Gets a value indicating whether the cache has been initialized. </summary>
        /// <value> <c>true</c> if both background and foreground colors have been set; otherwise, <c>false</c>. </value>
        public bool IsInitialized => _bgColor.HasValue && _fgColor.HasValue;

        /// <summary> Gets the cached background color. </summary>
        /// <exception cref="InvalidOperationException"> Thrown if the cache has not been initialized. </exception>
        public Color BgColor { get { CheckIsInitialized(); return _bgColor.Value; } }

        /// <summary> Gets the cached foreground color. </summary>
        /// <exception cref="InvalidOperationException"> Thrown if the cache has not been initialized. </exception>
        public Color FgColor { get { CheckIsInitialized(); return _fgColor.Value; } }

        #endregion // Properties

        #region Methods

        /// <summary> Initializes the cache with system-defined colors. </summary>
        public void Initialize()
        {
            InitColorsFromSystem(enforceRefresh: false);
        }

        /// <summary>
        /// Attempts to initialize the cache with colors retrieved from the specified <see cref="TreeView"/>.
        /// </summary>
        /// <param name="treeView"> The tree view from which to retrieve colors. </param>
        /// <returns> <c>true</c> if colors were successfully retrieved; otherwise, <c>false</c>. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if <paramref name="treeView"/> is <c>null</c>. </exception>
        public bool InitializeFromNodes(TreeView treeView)
        {
            ArgumentNullException.ThrowIfNull(treeView);
            return InitColorsFromNodes(treeView, enforceRefresh: false);
        }

        /// <summary> Forces a refresh of the cached colors from the system. </summary>
        public void Refresh()
        {
            InitColorsFromSystem(enforceRefresh: true);
        }

        /// <summary>
        /// Ensures that the cache has been initialized before accessing its values.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown if the cache has not been initialized. </exception>
        protected void CheckIsInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException($"{GetType().Name} is not initialized");
        }

        /// <summary>
        /// Initializes the cache with system colors, optionally forcing a refresh.
        /// </summary>
        /// <param name="enforceRefresh"> If set to <c>true</c>, forces the cache to refresh even if already initialized. </param>
        protected virtual void InitColorsFromSystem(bool enforceRefresh = false)
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
        /// Attempts to initialize the cache with colors from a tree view's nodes.
        /// </summary>
        /// <param name="treeView"> The tree view from which to retrieve colors. </param>
        /// <param name="enforceRefresh"> If set to <c>true</c>, forces the cache to refresh even if already initialized. </param>
        /// <returns> True if colors were successfully retrieved; or if initialized already and not enforcing refresh;
        ///           otherwise, false. </returns>
        protected bool InitColorsFromNodes(TreeView treeView, bool enforceRefresh = false)
        {
            ArgumentNullException.ThrowIfNull(treeView);

            if (IsInitialized && !enforceRefresh) return true;

            static bool IsNotRootAndNotSelected(TreeNode tn) => tn.Level > 0 && !tn.IsSelected;
            TreeNode node = treeView.FindNode(IsNotRootAndNotSelected);
            bool result = node is not null;

            if (result)
            {
                _bgColor = node.BackColor;
                _fgColor = node.ForeColor;
            }

            return result;
        }

        /// <summary>
        /// Attempts to retrieve themed colors for a tree view using the visual style renderer.
        /// </summary>
        /// <param name="bgColor"> The retrieved background color. </param>
        /// <param name="fgColor"> The retrieved foreground color. </param>
        /// <returns> True if colors were successfully retrieved; otherwise, false. </returns>
        private static bool TryGetThemedColorForTreeView(out Color bgColor, out Color fgColor)
        {
            return TryGetThemedColorsFromRenderer(VisualStyleElement.TreeView.Item.Normal, out bgColor, out fgColor);
        }

        /// <summary>
        /// Attempts to retrieve themed colors for a text box using the visual style renderer.
        /// </summary>
        /// <param name="bgColor"> The retrieved background color. </param>
        /// <param name="fgColor"> The retrieved foreground color. </param>
        /// <returns> True if colors were successfully retrieved; otherwise, false. </returns>
        private static bool TryGetThemedColorForTextBox(out Color bgColor, out Color fgColor)
        {
            return TryGetThemedColorsFromRenderer(VisualStyleElement.TextBox.TextEdit.Normal, out bgColor, out fgColor);
        }

        private static bool TryGetThemedColorsFromRenderer(
            VisualStyleElement element,
            out Color bgColor,
            out Color fgColor)
        {
            bool result = true;

            try
            {
                var renderer = new VisualStyleRenderer(element);
                bgColor = renderer.GetColor(ColorProperty.FillColor);
                fgColor = renderer.GetColor(ColorProperty.TextColor);
            }
            catch
            {
                // The call above could throw ArgumentException, saying 
                // "Given combination of Class, Part And State is not defined in current Visual style"
                // 
                bgColor = fgColor = Color.Empty;
                result = false;
            }

            return result;
        }

        #endregion // Methods
    }
    #endregion // Typedefs
}
