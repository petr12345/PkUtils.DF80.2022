using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PK.TestWeakRef
{
    /// <summary>
    /// The purpose of class SourceStatic is to test a scenario when the 
    /// owner of the event is static class.
    /// </summary>
    public static class SourceStatic
    {
        internal static EventHandler<CancelEventArgs> _evStaticEvent;

        /// <summary>
        /// Publishing the event itself
        /// </summary>
        public static event EventHandler<CancelEventArgs> evStaticEvent
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add { _evStaticEvent += value; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove { _evStaticEvent -= value; }
        }

        /// <summary>
        /// Firing the event
        /// </summary>
        public static void FireStaticEvent()
        {
            if (null != _evStaticEvent)
            {
                _evStaticEvent(null, new CancelEventArgs());
            }
        }
    }
}
