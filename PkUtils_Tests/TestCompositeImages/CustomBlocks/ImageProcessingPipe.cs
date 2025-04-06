/***************************************************************************************************************
*
* FILE NAME:   .\CustomBlocks\ImageProcessingPipe.cs
* 
* PROJECT:     Demo project regarding Using Dataflow in a Windows Forms Application,
*              based on MSDN project https://msdn.microsoft.com/en-us/library/hh228605(v=vs.110).aspx
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of ImageProcessingPipe class
*
**************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using TestCompositeImages.Program;
using TestCompositeImages.UI;

namespace TestCompositeImages.CustomBlocks
{
    /// <summary> A static class containing various bitmap-related utilities. </summary>
    public class ImageProcessingPipe : IDisposable
    {
        #region Fields
        /// <summary> The head of the dataflow network. </summary>
        protected ITargetBlock<string> _headBlock;

        /// <summary> Enables the user interface to signal cancellation to the network. </summary>
        /// <remarks> Should be subject of disposing, since the implementation contains ManualResetEvent, containing native handle.</remarks>
        protected CancellationTokenSource _cancellationTokenSource;
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Constructor. </summary>
        /// 
        /// <exception cref="ArgumentNullException"> Passed when one or more required arguments are null. </exception>
        ///
        /// <param name="form">                     The main form. </param>
        /// <param name="supportedImageExtensions"> The supported image extensions. </param>
        public ImageProcessingPipe(
            MainForm form,
            IEnumerable<string> supportedImageExtensions) : this()
        {
            CreateImageProcessingNetwork(form, supportedImageExtensions);
        }

        /// <summary> Specialized default constructor for use only by derived class or other constructors. </summary>
        protected ImageProcessingPipe()
        {
            // Create a new CancellationTokenSource object to enable cancellation.
            _cancellationTokenSource = new CancellationTokenSource();
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> Gets or sets the head block. </summary>
        ///
        /// <value> The head block. </value>
        public ITargetBlock<string> HeadBlock
        {
            get { return _headBlock; }
            protected set { _headBlock = value; }
        }

        public CancellationTokenSource CancellationTokenSource
        {
            get { return _cancellationTokenSource; }
            protected set { _cancellationTokenSource = value; }
        }
        #endregion // Properties

        #region Methods

        #region Public Methods

        /// <summary>
        /// Creates the image processing dataflow network and returns the head node of the network.
        /// </summary>
        ///
        /// <param name="supportedImageExtensions"> The supported image extensions. </param>
        ///
        /// <returns>   The head node of the network. </returns>
        public static ITargetBlock<string> CreateImageProcessingPipe(
            MainForm form,
            IEnumerable<string> supportedImageExtensions)
        {
            ImageProcessingPipe pipe = new();

            pipe.CreateImageProcessingNetwork(form, supportedImageExtensions);
            return pipe.HeadBlock;
        }
        #endregion // Public Methods

        #region Protected Methods

        /// <summary>
        /// Creates the image processing dataflow network and returns the head node of the network.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        ///
        /// <param name="form">                     The main form. </param>
        /// <param name="supportedImageExtensions"> The supported image extensions. </param>
        ///
        /// <returns>   The head node of the network. </returns>
        protected ITargetBlock<string> CreateImageProcessingNetwork(
            MainForm form,
            IEnumerable<string> supportedImageExtensions)
        {
            if (form == null)
                throw new ArgumentNullException("form");
            if (supportedImageExtensions == null)
                throw new ArgumentNullException("supportedImageExtensions ");
            // 
            // Create the dataflow blocks that form the network. 
            // 

            // Create a dataflow block that takes a folder path as input 
            // and returns a collection of Bitmap objects. 
            var loadBitmaps = new TransformBlock<string, IEnumerable<Bitmap>>(path =>
            {
                try
                {
                    return BitmapUtils.LoadBitmaps(path, supportedImageExtensions, _cancellationTokenSource);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation by passing the empty collection 
                    // to the next stage of the network. 
                    return Enumerable.Empty<Bitmap>();
                }
            });

            // Create a dataflow block that takes a collection of Bitmap objects 
            // and returns a single composite bitmap. 
            var createCompositeBitmap = new TransformBlock<IEnumerable<Bitmap>, Bitmap>(bitmaps =>
            {
                try
                {
                    return BitmapUtils.CreateCompositeBitmap(bitmaps, _cancellationTokenSource);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation by passing null to the next stage  
                    // of the network. 
                    return null;
                }
            });

            // Create a dataflow block that displays the provided bitmap on the form. 
            var displayCompositeBitmap = new ActionBlock<Bitmap>(bitmap =>
            {
                // Display the bitmap.
                form.DisplayFinalBitmap(bitmap);
            },
               // Specify a task scheduler from the current synchronization context 
               // so that the action runs on the UI thread. 
               new ExecutionDataflowBlockOptions
               {
                   TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext()
               });

            // Create a dataflow block that responds to a cancellation request by  
            // displaying an image to indicate that the operation is canceled and  
            // enables the user to select another folder. 
            var operationCancelled = new ActionBlock<object>(delegate
            {
                form.DisplayErrorBitmap();
            },
               // Specify a task scheduler from the current synchronization context 
               // so that the action runs on the UI thread. 
               new ExecutionDataflowBlockOptions
               {
                   TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext()
               });

            // 
            // Connect the network. 
            // 

            // Link loadBitmaps to createCompositeBitmap.  
            // The provided predicate ensures that createCompositeBitmap accepts the  
            // collection of bitmaps only if that collection has at least one member.
            loadBitmaps.LinkTo(createCompositeBitmap, bitmaps => bitmaps.Count() > 0);

            // Also link loadBitmaps to operationCancelled. 
            // When createCompositeBitmap rejects the message, loadBitmaps  
            // offers the message to operationCancelled. 
            // operationCancelled accepts all messages because we do not provide a  
            // predicate.
            loadBitmaps.LinkTo(operationCancelled);

            // Link createCompositeBitmap to displayCompositeBitmap.  
            // The provided predicate ensures that displayCompositeBitmap accepts the  
            // bitmap only if it is non-null.
            createCompositeBitmap.LinkTo(displayCompositeBitmap, bitmap => bitmap != null);

            // Also link createCompositeBitmap to operationCancelled.  
            // When displayCompositeBitmap rejects the message, createCompositeBitmap  
            // offers the message to operationCancelled. 
            // operationCancelled accepts all messages because we do not provide a  
            // predicate.
            createCompositeBitmap.LinkTo(operationCancelled);

            // Return the head of the network. 
            return _headBlock = loadBitmaps;
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
        /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
        /// disposing equals false, the method has been called by the runtime from inside the finalizer and you should
        /// not reference other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }
        #endregion // Protected Methods
        #endregion // Methods

        #region IDisposable Members

        /// <summary>
        /// Implements IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue to prevent finalization code 
            // for this object from executing a second time.
            GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Members

    }
}
