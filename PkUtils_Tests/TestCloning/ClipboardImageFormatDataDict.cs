using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PK.PkUtils.Cloning.Binary;
using SerializedTupleList = System.Collections.Generic.List<System.Tuple<string, byte[]>>;

// Ignore Spelling: Dict
//
#pragma warning disable SYSLIB0011 // BinaryFormatter serialization is obsolete and should not be used.
#pragma warning disable SYSLIB0050 // IDeserializationCallback is obsolete
#pragma warning disable SYSLIB0051 // GetObjectData is obsolete


namespace PK.TestCloning
{
    #region ClipboardImageFormatDataDict Class

    /// <summary>
    /// Auxiliary class keeping the image data that eventually will be copied to clipboard.
    /// </summary>
    [Serializable]
    public class ClipboardImageFormatDataDict : Dictionary<ImageFormat, Image>, IDisposable, IDeserializationCallback
    {
        #region Public Interface

        /// <summary>
        /// Default argument-less constructor.
        /// </summary>
        public ClipboardImageFormatDataDict()
        {
        }

        /// <summary> Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// 
        /// <remarks>
        /// The method overrides the implementation of base class, saving data of each (ImageFormat, Image)
        /// pair as Tuple(string, byte[]). This is needed for two reasons:
        /// i/ ImageFormat itself is not serializable, but its Guid can be saved as string
        /// ii/ the best way to store/restore the image seems through byte array, 
        /// that could be o deserialization used for memory stream.
        /// 
        /// Note: the method performs even much more elaborate process, by moving the whole contents
        /// of dictionary to backup dictionary, cleaning the data, calling base.GetObjectData,
        /// custom-serializing the backup data, and reverting backup contents back.
        /// This is needed, since the call of base.GetObjectData is essential for preparing the data
        /// "Version", "HashSize", "Comparer", "KeyValuePairs" that could be used later by
        /// de-serialized dictionary to prepare its internal structures for adding the contents.
        /// However, I cannot call base.GetObjectData with not-empty 'this' dictionary, because of problem with
        /// the SerializationException being thrown in that case.
        /// "Type 'System.Drawing.Imaging.ImageFormat' in Assembly 'System.Drawing ...' is not marked as serializable.
        /// </remarks>
        /// 
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> Describes the source and destination of a given serialized stream, 
        /// and provides an additional caller-defined context.
        /// </param>
        [Obsolete("This method overrides an obsolete base member. BinaryFormatter serialization is obsolete and should not be used.")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // backup of 'this' data
            Dictionary<ImageFormat, Image> myDataBackup;
            // a temporary list containing transformed data that will be added to SerializationInfo info
            SerializedTupleList serializedData = [];

            // 1. create a backup of data in this dictionary, and clear the dictionary
            myDataBackup = new Dictionary<ImageFormat, Image>(this);
            this.Clear();

            // 2. call base functionality, in order to fill-in certain basic values into SerializationInfo.
            //   These values include "Version", "HashSize", "Comparer", "KeyValuePairs"
            base.GetObjectData(info, context);

            // 3. fill-in the list serializedData from myDataBackup
            foreach (var pair in myDataBackup)
            {
                string keyData = pair.Key.Guid.ToString();
                byte[] imageData = CloneHelperBinary.ToByteArray(pair.Value);

                serializedData.Add(Tuple.Create(keyData, imageData));
            }
            // 4. add contents of that list into SerializationInfo
            info.AddValue("data", serializedData, typeof(SerializedTupleList));

            // 5. retrieve data from backup back to this dictionary
            foreach (var pair in myDataBackup)
            {
                this.Add(pair.Key, pair.Value);
            }
        }
        #endregion // Public Interface

        #region IDisposable Members
        /// <summary>
        /// Use this method to close or release all resources (such as files, streams, handles etc.)
        /// held by an instance of the class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Members

        #region IDeserializationCallback Members

        /// <summary>
        /// Method is invoked by the ObjectManager involved in the Deserialization-Process.
        /// Runs when (after) the entire object graph has been deserialized.
        /// </summary>
        /// 
        /// <param name="sender"> The object that initiated the callback. 
        /// The functionality for this parameter is not currently implemented.
        /// </param>
        /// 
        /// <remarks>
        /// There is no semantic difference between [OnDeserialized] methods and IDeserializationCallback.OnDeserialization; 
        /// IDeserializationCallback was defined in Framework 1.0 whereas [OnDeserialized] methods 
        /// were introduced in Framework 2.0.
        ///
        /// The only way to make sure that your dictionary is ready for being used is to 
        /// wait with your changes till the ObjectManager invokes OnDeserialization, or to invoke
        /// its IDeserializationCallback.OnDeserialization method from within your [OnDeserialized] method.
        /// </remarks>
        /// 
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            if (null != _DeserializedData)
            {
                foreach (var tp in _DeserializedData)
                {
                    Image img;
                    ImageFormat key = new(new Guid(tp.Item1));

                    using (MemoryStream stream = new(tp.Item2))
                    {
                        var formatter = new BinaryFormatter
                        {
                            Context = new StreamingContext(StreamingContextStates.Clone)
                        };
                        stream.Position = 0;
                        img = formatter.Deserialize(stream) as Image;
                    }

                    this.Add(key, img);
                }
                _DeserializedData = null;
            }
        }
        #endregion // IDeserializationCallback Members

        #region Protected Interface

        /// <summary> A specific constructor need for .NET serialization support. 
        /// It is called during object deserialization, when BinaryFormatter extracts an object from the byte stream.
        /// </summary>
        /// 
        /// <param name="info"> This object stores all the data needed to serialize or deserialize an instance.
        /// BinaryFormatter creates this object when a formatter serializes or deserializes an object graph.
        /// </param>
        /// <param name="context"> Describes the source and destination of a given serialized stream, 
        /// and provides an additional caller-defined context.
        /// </param>
        /// 
        /// <seealso href="http://msdn.microsoft.com/es-es/magazine/cc301761(en-us).aspx">
        /// .NET Run-time Serialization, part 1, April 2002 issue of MSDN Magazine
        /// </seealso>
        /// <seealso href="http://msdn.microsoft.com/en-us/magazine/cc301767.aspx">
        /// .NET Run-time Serialization, part 2, July 2002 issue of MSDN Magazine
        /// </seealso>
        /// <seealso href="http://msdn.microsoft.com/en-us/magazine/cc188950.aspx">
        /// .NET Run-time Serialization, part 3, September 2002 issue of MSDN Magazine
        /// </seealso>
        /// 
        /// <remarks>
        /// The code does nothing else but extracting a custom-serialized data and saving them in auxiliary field
        /// _DeserializedData. the reason of that is unless ( until ) the virtual method 
        ///   public virtual void OnDeserialization(object sender)
        /// gets called, the dictionary is not prepared for adding any (key, value) pairs, 
        /// and throws NullReferenceException upon such call.
        /// But OnDeserialization is called much later after this constructor gets called.
        /// For more info about this problem, see for instance
        /// 
        /// <seealso href="http://connect.microsoft.com/VisualStudio/feedback/details/579500/ideserializationcallback-calling-order-incorrect">
        /// IDeserializationCallback calling order incorrect </seealso>
        /// 
        /// <seealso href="http://social.msdn.microsoft.com/Forums/en-US/e5644081-fd01-4646-81c0-c283dc5d0fd4/do-dictionaries-deserialize-later-than-ondeserialized-marked-methods-are-called?forum=netfxremoting">
        /// Do Dictionaries Deserialize Later Than OnDeserialized Marked Methods Are Called?</seealso>
        ///
        /// <seealso href="http://stackoverflow.com/questions/8444788/strange-deserialization-error-child-objects-are-not-fully-deserialized">
        /// Strange deserialization error, child objects are not fully deserialized</seealso>
        /// </remarks>
        /// 
        protected ClipboardImageFormatDataDict(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
            var data = info.GetValue("data", typeof(SerializedTupleList)) as SerializedTupleList;

            _DeserializedData = data;
        }

        /// <summary>
        /// Cleanup any resources being used.
        /// </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
        /// Otherwise it is called by finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            foreach (KeyValuePair<ImageFormat, Image> pair in this)
            {
                pair.Value?.Dispose();
            }
            this.Clear();
        }
        #endregion // Protected Interface

        #region Private members

        /// <summary>
        /// An auxiliary field, used in <see cref="OnDeserialization"/>.
        /// </summary>
        [NonSerialized]
        private SerializedTupleList _DeserializedData;
        #endregion // Private members
    }
    #endregion // ClipboardImageFormatDataDict Class
}
#pragma warning restore SYSLIB0051
#pragma warning restore SYSLIB0050
#pragma warning restore SYSLIB0011