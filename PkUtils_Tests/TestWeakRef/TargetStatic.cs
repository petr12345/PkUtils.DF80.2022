using System;
using System.ComponentModel;
using System.Globalization;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.TestWeakRef
{
    /// <summary>
    /// The purpose of this class TargetStatic is to test WeakEventHandler for the case 
    /// when the target method is static, or the whole class who implements target method is static.
    /// In case of TargetStatic, the relevant methods called by WeakEventHandler are 
    /// OnTestFor_DoubleClick and OnTestFor_HelpButtonClicked.
    /// </summary>
    public static class TargetStatic
    {
        #region Fields
        private static IDumper _dumper;

        #endregion // Fields

        #region Properties
        public static IDumper Dumper
        {
            get { return TargetStatic._dumper; }
        }
        #endregion // Properties

        #region Methods

        public static void SetDumper(IDumper dumper)
        {
            TargetStatic._dumper = dumper;
        }

        /// <summary>
        /// This is the handler I want to invoke when the tested Form is double-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnTestFor_DoubleClick(object sender, EventArgs e)
        {
            if (TargetShortLiving.SupportCallbacksDump && (null != Dumper))
            {
                string strMsg = string.Format(
                  CultureInfo.InvariantCulture,
                  "Received DoubleClick event in {0}",
                  (typeof(TargetStatic)).TypeToReadable());
                Dumper.DumpLine(strMsg);
            }
        }

        /// <summary>
        /// This is the handler I want to subscribe to static event 
        /// SourceStatic.evStaticEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnStaticEventReceived(object sender, CancelEventArgs e)
        {
            if (TargetShortLiving.SupportCallbacksDump && (null != Dumper))
            {
                string strMsg = string.Format(
                  CultureInfo.InvariantCulture,
                  "Received evStaticEvent event in {0}",
                  (typeof(TargetStatic)).TypeToReadable());
                Dumper.DumpLine(strMsg);
            }
        }
        #endregion // Methods
    }
}
