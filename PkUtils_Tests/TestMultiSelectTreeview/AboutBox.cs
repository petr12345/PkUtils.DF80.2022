using System.Reflection;
using System.Text;
using PK.PkUtils.Consoles;
using PK.PkUtils.Extensions;
using PK.PkUtils.UI.Utils;

namespace TestMultiSelectTreeView;

/// <summary> An simple about box. </summary>
public partial class AboutBox : Form
{
    #region Fields
    private bool _expanded = true;
    private int _verticalDelta;
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
            _linkLabelWebsite.InitializeWebsiteLink("https://www.linkedin.com/in/petr-kodet-3b0bb/");
        }
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets a value indicating whether this object is expanded. </summary>
    protected bool IsExpanded { get => _expanded; }

    private int VerticalDelta { get => _verticalDelta; }

    #endregion // Properties

    #region Methods

    /// <summary>   Switch expanded/collapsed state. </summary>
    /// <param name="enforceChange"> (Optional) True to enforce change, regardless of _textBoxDescription.Visible
    /// current value. This is needed for instance when being called for instance from Load event handler. </param>
    protected void SwitchExpandedCollapsed(bool enforceChange = false)
    {
        _expanded = !IsExpanded;

        if (enforceChange || (_textBoxDescription.Visible != IsExpanded))
        {
            if (_textBoxDescription.Visible = IsExpanded)
                Height += VerticalDelta;
            else
                Height -= VerticalDelta;
        }
        _buttonMore.Text = IsExpanded ? "<< Less" : "More >>";
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
        _verticalDelta = _textBoxDescription.Height;
        if (IsExpanded)
        {
            SwitchExpandedCollapsed(enforceChange: true);
        }
    }

    private void OnButtonMore_Click(object sender, EventArgs e)
    {
        SwitchExpandedCollapsed(enforceChange: false);
    }

    private void OnButtonOK_Click(object sender, EventArgs e)
    {
        Close();
    }
    #endregion // Event_handlers
}
