using System.Diagnostics;
using System.Reflection;
using System.Text;
using PK.PkUtils.Consoles;
using PK.PkUtils.UI.Utils;


namespace TestMultiSelectTreeView;

/// <summary> An simple about box. </summary>
public partial class AboutBox : Form
{
    #region Fields
    private bool _expanded = true;
    private int _collapsedHeight;
    private int _expandedHeight;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public AboutBox()
    {
        InitializeComponent();

        Icon = ConsoleIconManager.CreateIcon(Resources.tree);
        Text = $"About {AssemblyProduct}";

        _labelAssemblyTitle.Text = AssemblyTitle;
        _labelVersion.Text = $"Version {AssemblyVersion}";
        _labelCopyright.Text = AssemblyCopyright;
        if (DesignerSupport.IsDesignMode(this))
        {
            _textBoxDescription.Text = "This is a design-time placeholder text.";
        }
        else
        {
            _textBoxDescription.Text = GetLoadedAssembliesInfo();
            InitializeWebsiteLink("https://www.linkedin.com/in/petr-kodet-3b0bb/");
        }
    }
    #endregion // Constructor(s)

    #region Properties

    private int CollapsedHeight { get => _collapsedHeight; }
    private int ExpandedHeight { get => _expandedHeight; }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Initializes the <see cref="_linkLabelWebsite"/> with the specified link and optional display text.
    /// If no display text is provided, the link itself is shown as the label text.
    /// <para>
    /// If the display text differs from the link, a separate handler is attached
    /// to open the link manually, because automatic browser navigation only occurs when the full URL is displayed.
    /// </para>
    /// </summary>
    /// <param name="link">The URL to navigate to when the link is clicked. Must not be null or empty.</param>
    /// <param name="displayText">
    /// Optional text to display instead of the full link. 
    /// If omitted, the full link is shown and navigation happens automatically.
    /// </param>
    /// <exception cref="ArgumentException">Thrown if the link is null or empty.</exception>
    public void InitializeWebsiteLink(string link, string displayText = null)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(link);
        string linkText = displayText ?? link;

        _linkLabelWebsite.Text = linkText;
        _linkLabelWebsite.Links.Clear();
        _linkLabelWebsite.Links.Add(0, linkText.Length, link);

        // If the displayed text is not the full link, we must handle LinkClicked manually
        if (displayText != null && !displayText.Equals(link, StringComparison.OrdinalIgnoreCase))
        {
            _linkLabelWebsite.LinkClicked -= LinkLabelWebsite_LinkClicked; // Ensure no multiple subscriptions
            _linkLabelWebsite.LinkClicked += LinkLabelWebsite_LinkClicked;
        }
    }

    private void SwitchExpandedCollapsed()
    {
        this.Height = (_expanded = !_expanded) ? ExpandedHeight : CollapsedHeight;
        _textBoxDescription.Visible = _expanded;
        _buttonMore.Text = _expanded ? "<< Less" : "More >>";
    }

    #region Assembly Attribute Accessors

    private static string AssemblyTitle =>
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "Untitled";

    private static string AssemblyVersion =>
        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";

    private static string AssemblyDescription =>
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "";

    private static string AssemblyProduct =>
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "";

    private static string AssemblyCopyright =>
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "";

    private static string GetLoadedAssembliesInfo()
    {
        // Get all currently loaded assemblies in the current AppDomain
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Initialize a StringBuilder to construct the output string
        StringBuilder sb = new();
        string separatorLine = new('-', 40); // Separator line

        // Add an introductory description
        sb.AppendLine("List of loaded assemblies:");
        sb.AppendLine(separatorLine);

        // Iterate through each loaded assembly and gather information
        foreach (var assembly in assemblies)
        {
            // Get basic information: Full Name, Location, and Version
            string fullName = assembly.FullName;
            string location = assembly.Location;
            string version = assembly.GetName().Version.ToString();

            // Add information to the StringBuilder (one line for each assembly)
            sb.AppendLine($"Assembly: {fullName}");
            sb.AppendLine($"Location: {location}");
            sb.AppendLine($"Version: {version}");
            sb.AppendLine(separatorLine);
        }

        // Return the complete string of assembly information
        return sb.ToString();
    }

    #endregion // Assembly Attribute Accessors
    #endregion // Methods

    #region Event_handlers

    private void AboutBox_Load(object sender, EventArgs e)
    {
        _collapsedHeight = (_expandedHeight = Height) - _textBoxDescription.Height;
        if (_expanded)
        {
            SwitchExpandedCollapsed();
        }
    }

    /// <summary>
    /// Handles the click event on <see cref="_linkLabelWebsite"/> and opens the associated link.
    /// </summary>
    private void LinkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (e.Link.LinkData is string target && !string.IsNullOrEmpty(target))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = target,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void OnButtonMore_Click(object sender, EventArgs e)
    {
        SwitchExpandedCollapsed();
    }

    private void OnLinkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        // Process.Start(new ProcessStartInfo(_linkLabelWebsite.Text) { UseShellExecute = true });
        if (e.Link.LinkData is string target && !string.IsNullOrEmpty(target))
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = target, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void OnButtonOK_Click(object sender, EventArgs e)
    {
        Close();
    }
    #endregion // Event_handlers
}
