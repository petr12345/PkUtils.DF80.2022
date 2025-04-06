/***************************************************************************************************************
*
* FILE NAME:   .\Reflection\EventsUtils.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class EventsUtils
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Reflection;

namespace PK.PkUtils.Reflection;

/// <summary>
/// Contains useful reflection-implemented utilities accessing events.
/// </summary>
/// <seealso cref="FieldsUtils"/>
/// <seealso cref="MethodsUtils"/>
/// <seealso cref="PropertiesUtils"/>
/// <seealso cref="ReflectionUtils"/>
[CLSCompliant(true)]
public static class EventsUtils
{
    #region Public Methods

    /// <summary>
    /// Removes a subscribed event handler from the specified event of <paramref name="eventOwner"/>.
    /// The event handler itself is identified by <paramref name="subscriber"/> and <paramref name="mInfo"/>.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of the event arguments.</typeparam>
    /// <param name="eventOwner">The object that declares or inherits the event.</param>
    /// <param name="eventName">The name of the event to remove the handler from.</param>
    /// <param name="subscriber">The instance that contains the event handler method.</param>
    /// <param name="mInfo">The method info of the event handler to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown if any argument is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if the specified event is not found.</exception>
    public static void RemoveEventHandler<TEventArgs>(
        this object eventOwner,
        string eventName,
        object subscriber,
        MethodInfo mInfo) where TEventArgs : EventArgs
    {
        ArgumentNullException.ThrowIfNull(eventOwner);
        ArgumentNullException.ThrowIfNull(eventName);
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(mInfo);

        Type ownerType = eventOwner.GetType();
        EventInfo eventInfo = ownerType.GetEvent(eventName)
            ?? throw new ArgumentException($"Event '{eventName}' not found on type '{ownerType.FullName}'.", nameof(eventName));

        MethodInfo removeHandler = eventInfo.GetRemoveMethod(true)
            ?? throw new InvalidOperationException($"Event '{eventName}' does not have a remove accessor.");

        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, subscriber, mInfo)
            ?? throw new InvalidOperationException($"Failed to create delegate for method '{mInfo.Name}'.");

        removeHandler.Invoke(eventOwner, [handler]);
    }

    #endregion // Public Methods
}
