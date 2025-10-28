// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.Extensions;

/// <summary> Implements extension methods for raising events. </summary>
public static class EventHandlerExtensions
{
    /// <summary> An EventHandler extension method that fires event. </summary>
    ///
    /// <typeparam name="TEventArgs"> Type of the event arguments. </typeparam>
    /// <param name="handler">  The generic handler to act on. </param>
    /// <param name="sender">   Source of the event. </param>
    /// <param name="args">  Event information to send to registered event handlers. </param>
    public static void Fire<TEventArgs>(
        this EventHandler<TEventArgs> handler,
        object sender,
        TEventArgs args)
    {
        handler?.Invoke(sender, args);
    }

    /// <summary> An EventHandler extension method that fires event. </summary>
    ///
    /// <param name="handler">  The handler to act on. </param>
    /// <param name="sender">   Source of the event. </param>
    public static void Fire(this EventHandler handler, object sender)
    {
        handler?.Invoke(sender, EventArgs.Empty);
    }

}