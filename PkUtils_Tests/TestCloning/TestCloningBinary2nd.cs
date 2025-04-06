
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.Cloning.Binary;

namespace PK.TestCloning
{

    /// <summary>
    /// Static class performing two tests. 
    /// The first tests is serializing a simple dictionary, the second serializes ClipboardImageFormatDataDict.
    /// </summary>
    public static class TestCloningBinary2nd
    {
        /// <summary> Tests cloning  a class ClipboardImageFormatDataDict  derived from generic dictionary. 
        /// </summary>
        public static void TestCloningSimpleDictionary()
        {
            Console.WriteLine("----TestCloningBinary2nd - TestCloningSimpleDictionary");

            Dictionary<string, int> firstDict = new() {
        {"cat", 2},
        {"dog", 1},
        {"llama", 0},
        {"iguana", -1}
      };

            Dictionary<string, int> cloneDict = firstDict.DeepClone();

            Console.WriteLine("Check clone is not original: {0}", cloneDict != firstDict); //true - new object
            Console.WriteLine("Check keys equal: {0}", firstDict.Keys.SequenceEqual(cloneDict.Keys));
            Console.WriteLine("Check values equal: {0}", firstDict.Values.SequenceEqual(cloneDict.Values));
            foreach (var key in firstDict.Keys)
            {
                Console.WriteLine("Check the key '{0}' has equal value '{1}': {2}",
                  key, firstDict[key], firstDict[key] == cloneDict[key]);
            }

            Console.WriteLine("---- end of test");
        }

        /// <summary> Tests cloning  a class ClipboardImageFormatDataDict  derived from generic dictionary. </summary>
        public static void TestCloningImageDict()
        {
            Console.WriteLine("----TestCloningBinary2nd - TestCloningDictionary");

            Bitmap screenSh = ScreenshotBmp();
            ClipboardImageFormatDataDict cloneDict;
            ClipboardImageFormatDataDict firstDict = new()
            {
                { ImageFormat.Bmp, screenSh }
            };

            cloneDict = firstDict.DeepClone();
            Console.WriteLine("Check clone is not original: {0}", cloneDict != firstDict); //true - new object
            Console.WriteLine("Check keys equal: {0}", firstDict.Keys.SequenceEqual(cloneDict.Keys));
            Console.WriteLine("Check values equal: {0}", firstDict.Values.SequenceEqual(cloneDict.Values));

            Console.WriteLine("---- end of test");
        }

        /// <summary>
        /// Creates a screenshot of current screen.
        /// </summary>
        /// <returns> A bitmap image.</returns>
        private static Bitmap ScreenshotBmp()
        {
            // Set the bitmap object to the size of the screen
            Bitmap bmpScreenshot = new(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap
            Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

            return bmpScreenshot;
        }
    }
}
