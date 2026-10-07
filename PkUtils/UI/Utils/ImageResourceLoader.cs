// Ignore Spelling: Png
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace PK.PkUtils.UI.Utils;

/// <summary>Loads plain PNG resource bytes without serialized image-list objects.</summary>
public static class ImageResourceLoader
{
    /// <summary>Decodes a PNG resource and returns an independently owned bitmap.</summary>
    /// <param name="resources">The resource manager for the owning form.</param>
    /// <param name="name">The byte-array resource name.</param>
    /// <returns>A bitmap whose lifetime is independent of the decoding stream.</returns>
    /// <exception cref="InvalidOperationException">The PNG byte resource is missing.</exception>
    public static Bitmap LoadPng(ComponentResourceManager resources, string name)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(name);
        byte[] bytes = resources.GetObject(name) as byte[]
            ?? throw new InvalidOperationException($"Missing PNG resource '{name}'.");
        using MemoryStream stream = new(bytes);
        using Image image = Image.FromStream(stream);
        return new Bitmap(image);
    }
}
