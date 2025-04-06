// Ignore Spelling: Finalizers
//
using System;
using System.Globalization;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;

namespace PK.TestWeakRef
{
    /// <summary>
    /// TargetShortLiving is an example of target (subscriber) of events.
    /// </summary>
    /// <remarks>
    /// It is derived from ICountable just for the purpose of this test
    /// ( to be able to track down type instances statistics );
    /// In real usage, there is no need of such inheritance.
    /// </remarks>
    public class TargetShortLiving : CountableGeneric<TargetShortLiving>, ICountable
    {
        #region Fields

        private readonly IDumper _dumper;
        private static bool _supportFinalizersDump;
        private static bool _supportCallbacksDump;
        #endregion // Fields

        #region Constructor(s)

        /// <summary>
        /// Argument-less constructor
        /// </summary>
        public TargetShortLiving()
          : this(null)
        {
        }

        /// <summary>
        /// Constructor accepting the dumper interface, that'll be used for dumping.
        /// </summary>
        /// <param name="dumper"></param>
        public TargetShortLiving(IDumper dumper)
        {
            _dumper = dumper;
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary>
        /// Nomen est omen
        /// </summary>
        public static bool SupportFinalizersDump
        {
            get { return _supportFinalizersDump; }
            set { _supportFinalizersDump = value; }
        }

        /// <summary>
        /// Nomen est omen
        /// </summary>
        public static bool SupportCallbacksDump
        {
            get { return _supportCallbacksDump; }
            set { _supportCallbacksDump = value; }
        }

        /// <summary>
        /// Give me the dumper ( if there is any )
        /// </summary>
        public IDumper Dumper
        {
            get { return _dumper; }
        }
        #endregion // Properties

        #region Methods

        /// <summary>
        /// This is the handler I want to invoke when the tested Form is resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTestFor_Resize(object sender, EventArgs e)
        {
            if (SupportCallbacksDump && (null != Dumper))
            {
                string strMsg = string.Format(
                  CultureInfo.InvariantCulture,
                  "Received Resize event in {0}",
                  this.ToString());
                Dumper.DumpLine(strMsg);
            }
        }

        /// <summary>
        /// This is the handler I want to invoke when the tested Form is single-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTestFor_SingleClick(object sender, EventArgs e)
        {
            if (SupportCallbacksDump && (null != Dumper))
            {
                string strMsg = string.Format(
                  CultureInfo.InvariantCulture,
                  "Received SingleClick event in {0}",
                  this.ToString());
                Dumper.DumpLine(strMsg);
            }
        }

        public override string ToString()
        {
            string strOrder = Conversions.IntegerToReadable(this.Order, CultureInfo.InvariantCulture);

            return string.Format(CultureInfo.InvariantCulture,
              "instance #{0} of type {1}",
              strOrder,
              this.GetType().TypeToReadable());
        }

        /// <summary>
        /// Overrides the implementation of disposing of base class.
        /// Dumps the information text to the dumper ( if there is any ).
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (SupportFinalizersDump && (null != Dumper))
                {
                    string strFmt = disposing ? " disposed {0}" : " finalized {0} ";
                    string strMsg = string.Format(CultureInfo.InvariantCulture, strFmt, this.ToString());
                    Dumper.DumpLine(strMsg);
                }

                base.Dispose(disposing);
            }
        }
        #endregion // Methods
    }
}