/***************************************************************************************************************
*
* FILE NAME:   .\Program\BitmapUtils.cs
* 
* PROJECT:     Demo project regarding Using Dataflow in a Windows Forms Application,
*              based on MSDN project https://msdn.microsoft.com/en-us/library/hh228605(v=vs.110).aspx
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of BitmapUtils class
*
**************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestCompositeImages.Program
{
    /// <summary> A static class containing various bitmap-related utilities. </summary>
    internal static class BitmapUtils
    {
        /// <summary> Loads all bitmap files that exist at the provided path. </summary>
        ///
        /// <exception cref="ArgumentNullException">      Thrown when one or more required arguments are null. </exception>
        /// <exception cref="OperationCanceledException"> Passed when operation has been canceled through
        /// CancellationTokenSource <paramref name="cancellationTokenSource"/>. </exception>
        ///
        /// <param name="path">                     Full pathname of the file. Can't be null. </param>
        /// <param name="supportedImageExtensions"> The collection supported image extensions. Can't be null. </param>
        /// <param name="cancellationTokenSource">  The cancellation token source. It can be null. </param>
        ///
        ///  <returns> Resulting sequence of Bitmaps that can be iterated. </returns>
        public static IEnumerable<Bitmap> LoadBitmaps(
            string path,
            IEnumerable<string> supportedImageExtensions,
            CancellationTokenSource cancellationTokenSource)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (supportedImageExtensions == null)
                throw new ArgumentNullException(nameof(supportedImageExtensions));

            List<Bitmap> bitmaps = [];

            // Load a variety of image types. 
            foreach (string bitmapType in supportedImageExtensions)
            {
                // Load each bitmap for the current extension. 
                foreach (string fileName in Directory.GetFiles(path, bitmapType))
                {
                    // Throw OperationCanceledException if cancellation is requested.
                    if (null != cancellationTokenSource)
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        // Add the Bitmap object to the collection.
                        bitmaps.Add(new Bitmap(fileName));
                    }
                    catch (Exception)
                    {
                        // TODO: A complete application might handle the error.
                    }
                }
            }
            return bitmaps;
        }

        /// <summary>
        /// Creates a composite bitmap from the provided collection of Bitmap objects. This method computes the
        /// average color of each pixel among all bitmaps to create the composite image.
        /// </summary>
        /// 
        /// <exception cref="OperationCanceledException"> Passed when operation has been canceled through
        /// CancellationTokenSource <paramref name="cancellationTokenSource"/>. </exception>
        ///
        /// <param name="bitmaps">                  The bitmaps. </param>
        /// <param name="cancellationTokenSource">  . </param>
        ///
        /// <returns>   The new composite bitmap. </returns>
        public static Bitmap CreateCompositeBitmap(
            IEnumerable<Bitmap> bitmaps,
            CancellationTokenSource cancellationTokenSource)
        {
            Bitmap[] bitmapArray = bitmaps.ToArray();

            // Compute the maximum width and height components of all  
            // bitmaps in the collection.
            Rectangle largest = new();
            foreach (var bitmap in bitmapArray)
            {
                if (bitmap.Width > largest.Width)
                    largest.Width = bitmap.Width;
                if (bitmap.Height > largest.Height)
                    largest.Height = bitmap.Height;
            }

            // Create a 32-bit Bitmap object with the greatest dimensions.
            Bitmap result = new(largest.Width, largest.Height,
               PixelFormat.Format32bppArgb);

            // Lock the result Bitmap. 
            var resultBitmapData = result.LockBits(
               new Rectangle(new Point(), result.Size), ImageLockMode.WriteOnly,
               result.PixelFormat);

            // Lock each source bitmap to create a parallel list of BitmapData objects. 
            var bitmapDataList = (from bitmap in bitmapArray
                                  select bitmap.LockBits(
                                    new Rectangle(new Point(), bitmap.Size),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb))
                                 .ToList();

            // Compute each column in parallel.
            Parallel.For(0, largest.Width, new ParallelOptions
            {
                CancellationToken = cancellationTokenSource.Token
            },
            i =>
            {
                // Compute each row. 
                for (int j = 0; j < largest.Height; j++)
                {
                    // Counts the number of bitmaps whose dimensions 
                    // contain the current location. 
                    int count = 0;

                    // The sum of all alpha, red, green, and blue components. 
                    int a = 0, r = 0, g = 0, b = 0;

                    // For each bitmap, compute the sum of all color components. 
                    foreach (var bitmapData in bitmapDataList)
                    {
                        // Ensure that we stay within the bounds of the image. 
                        if (bitmapData.Width > i && bitmapData.Height > j)
                        {
                            unsafe
                            {
                                byte* row = (byte*)(bitmapData.Scan0 + (j * bitmapData.Stride));
                                byte* pix = row + (4 * i);
                                a += *pix;
                                pix++;
                                r += *pix;
                                pix++;
                                g += *pix;
                                pix++;
                                b += *pix;
                            }
                            count++;
                        }
                    }

                    unsafe
                    {
                        // Compute the average of each color component.
                        a /= count;
                        r /= count;
                        g /= count;
                        b /= count;

                        // Set the result pixel. 
                        byte* row = (byte*)(resultBitmapData.Scan0 + (j * resultBitmapData.Stride));
                        byte* pix = row + (4 * i);
                        *pix = (byte)a;
                        pix++;
                        *pix = (byte)r;
                        pix++;
                        *pix = (byte)g;
                        pix++;
                        *pix = (byte)b;
                    }
                }
            });

            // Unlock the source bitmaps. 
            for (int i = 0; i < bitmapArray.Length; i++)
            {
                bitmapArray[i].UnlockBits(bitmapDataList[i]);
            }

            // Unlock the result bitmap.
            result.UnlockBits(resultBitmapData);

            // Return the result. 
            return result;
        }
    }

}
