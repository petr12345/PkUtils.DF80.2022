// Ignore Spelling: CCA, Concat
// 
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PK.PkUtils.Extensions;

/// <summary> A class implementing extensions of LinkLabel. </summary>
public static class LinkLabelExtensions
{
    /// <summary>
    /// Initializes the <paramref name="linkLabel"/> with the specified link and optional display text. If no
    /// display text is provided, the link itself is shown as the label text.
    /// <para>
    /// If the display text differs from the link, a separate handler is attached to open the link manually,
    /// because automatic browser navigation only occurs when the full URL is displayed.
    /// </para>
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown if the <paramref name="link"/> is null or empty. </exception>
    /// <param name="linkLabel"> The link label. Can't be null. </param>
    /// <param name="link"> The URL to navigate to when the link is clicked. Must not be null or empty. </param>
    /// <param name="displayText"> (Optional)
    /// Optional text to display instead of the full link. If omitted, the full link is shown and navigation
    /// happens automatically. </param>
    public static void InitializeWebsiteLink(this LinkLabel linkLabel, string link, string displayText = null)
    {
        ArgumentNullException.ThrowIfNull(linkLabel);
        ArgumentNullException.ThrowIfNullOrEmpty(link);

        string linkText = displayText ?? link;

        linkLabel.Text = linkText;
        linkLabel.Links.Clear();
        linkLabel.Links.Add(0, linkText.Length, link);

        // Prefer always to handle LinkClicked manually
        /* if (displayText != null && !displayText.Equals(link, StringComparison.OrdinalIgnoreCase)) */
        {
            linkLabel.LinkClicked -= LinkLabelWebsite_LinkClicked; // Ensure no multiple subscriptions
            linkLabel.LinkClicked += LinkLabelWebsite_LinkClicked;
        }
    }

    /// <summary>
    /// Handles the click event on a link and opens the associated link.
    /// </summary>
    private static void LinkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
}
