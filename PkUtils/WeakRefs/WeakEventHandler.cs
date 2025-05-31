/***************************************************************************************************************
*
* FILE NAME:   .\WeakRefs\WeakEventHandler.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: Contains the class WeakEventHandler
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Deregister, deregistered, deregisters, Codeproject
//
using System;
using System.Diagnostics;
using System.Reflection;
using PK.PkUtils.Extensions;

// Following conditional directive is needed because of the base class WeakHandlerCountableBase.
// In case of the test project, i do not refer to the source from the original PkUtils assembly,
// but rather include that source through a link in that separate testing project.
//
// It looks like this is the only way how to make WeakEventHandler countable
// in the testing project ( by deriving from WeakHandlerCountableBase ),
// at the same time without that slowing-down inheritance in the original PkUtils assembly.

#if TEST_WEAK_HANDLER
namespace PK.TestWeakRef;
#else
namespace PK.PkUtils.WeakRefs;
#endif

/// <summary>
/// <para>
/// Weak event handlers purpose is to help you to avoid creating strong reference 
/// to short-living object, if subscribing that object to an event of long-living object.
/// </para>
/// <para>
/// In more detail:<br/>
/// When you subscribe to an event, the event handler keeps a list of subscribers. 
/// When the event is raised, it loops through the subscribers and notifies each one 
/// - it's a simple form of the observer pattern. <br/>
/// Hence, to be able to free the short-ling object by GC collector, you generally need 
/// to have an unhook option. You might have a way to "remove" the short-lived object 
/// from the collection managed by the long-lived object, or you might unsubscribe from an event. 
/// When unsubscribing isn't an option (because you don't trust people to call your 
/// Dispose/Unsubscribe method), you can make use of weak event handlers.<br/>
/// </para>
/// <para>
/// <b>Usage of WeakEventHandler</b><br/>
/// When subscribing to events, instead of writing:
/// <code>  
///   _timer.Tick += target1.On_timer_Tick; 
///   /* or identically  _timer.Tick += new EventHandler(target1.On_timer_Tick) */
///   alarm.Beep += target2.Alar_Beeped;
/// </code>
/// just write:
/// <code>  
/// <![CDATA[
///   _timer.Tick += WeakEventHandler<EventArgs>.Create(target1.On_timer_Tick);
///   alarm.Beeped += WeakEventHandler<AlarmEventArgs>.CreateGeneric(target2.Alar_Beeped);
/// ]]>
/// </code>
/// Your subscribers target1, target2 now can be garbage collected 
/// without needing to manually unsubscribe (and without having to remember to). 
/// </para>
/// 
/// <para>
/// Observant readers will note that this code does keep a small "sacrifice" object alive.
/// The subscribing code with the += assignment still "silently" generates a 'regular' delegate object,
/// which is assigned (added) to the particular event invocation list of delegates.
/// That delegate object refers to our WeakEventHandler, and as long as the 'regular' delegate 
/// remains reachable, the WeakEventHandler remains reachable and cannot be collected by GC. 
/// This disadvantage can be overcome by creating self-deregisterAble WeakEventHandler,
/// providing more arguments for its construction, like
/// <code>  
/// <![CDATA[
///   this.alarm.Beeped += WeakEventHandler<AlarmEventArgs>.CreateGeneric(this.alarm, "Beeped", target2.Alar_Beeped);
/// ]]>
/// </code>
///  When providing the event owner and the event name this way, the constructor attempts 
///  to retrieve EventInfo object representing the specified event.<br/>
///  Later, when handling the event in WeakEventHandler, the method can recognize that the original 
///  event target has been garbage-collected, and will attempt to self-deregister the WeakEventHandler,
///  which includes removing the 'regular' delegate object from the invocation list of the event.
///  Afterwards, both the System.EventHandler and WeakEventHandler can be garbage-collected.
///  </para>
///  <para>
///  Note: There is still minor disadvantage with this approach : if the event is never fired, 
///  you leak both objects ( WeakEventHandler and the 'regular' delegate ). <br/>
///  You can periodically check for deregister-able WeakEventHandler(s) and release (deregister)
///  these objects by calling the method <see cref="EventHandlerUtilities.InvocationListClear(ref EventHandler, Func{Delegate, bool})"/>.
///  To be able to apply this method for a particular event,
///  you must supply the underlying EventHandler as the input-output argument.
///  In the implementation, that argument can be casted to MulticastDelegate, 
///  and for that you can call .GetInvocationList() ).<br/>
///  This is not a problem if you actually have implemented the event in your code, 
///  so there is the underlying delegate provided.<br/>
///  Otherwise, you have to resort to Reflection, like in the Codeproject article
/// <a href="http://www.codeproject.com/KB/cs/DelegateFromEvent.aspx">
/// Get Delegate from Event's Subscription
/// </a>.
///  
/// </para>
/// </summary>
/// 
/// <typeparam name="TEventArgs"> Type of the event arguments. </typeparam>
/// 
/// <remarks>
/// WeakEventHandler is declared with DebuggerNonUserCode attribute.<br/>
/// Applying DebuggerNonUserCode identifies a code ( method or a whole class) that is not part 
/// of the user code for an application.<br/>
/// When the debugger encounters this attribute when stepping through user code, the user
/// cannot see the related code, as the debugger steps to the next user-supplied code statement.
/// </remarks>
/// 
/// <seealso href="http://msdn.microsoft.com/en-us/vstudio//bb508935">
/// Multicast Delegate Internals</seealso>
/// 
/// <seealso href="http://msdn.microsoft.com/en-us/library/ms404247.aspx">
/// Weak References </seealso>
/// 
#if TEST_WEAK_HANDLER
public class WeakEventHandler<TEventArgs> : WeakHandlerCountableBase where TEventArgs : EventArgs
#else
[DebuggerNonUserCode]
public class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
#endif // TEST_WEAK_HANDLER
{
    #region Typedefs
    /// <summary>
    /// Defines possible status of what we know about the callback object (the subscriber).
    /// </summary>
    public enum TargetStatus
    {
        /// <summary> Represents the case when the target method is static </summary>
        TargetIsStatic,

        /// <summary> Represents the case when TargetReference.Target has not been released by GC collector yet. </summary>
        TargetIsAlive,

        /// <summary> Represents the case when TargetReference.Target == null </summary>
        TargetIsCollected,

        /// <summary> Represents the case when the WeakEventHandler has deregistered itself </summary>
        TargetIsDetached,
    }
    #endregion // Typedefs

    #region Fields
    private WeakReference _targetReference;
    private readonly MethodInfo _targetMethod;
    private readonly EventInfo _eventInfo;
    private readonly object _eventOwner;
    private readonly Type _ownerType;
    #endregion // Fields

    #region Constructors

    // For more reasons, constructors are not public.
    // For instance, if all constructors were public, in the calling the calling code 
    // they were likely to cause compilation error
    // CS0121: The call is ambiguous between the following methods or properties...


    /// <summary>
    /// Constructor creating a new instance of WeakEventHandler, with generic callback.
    /// Generated object is not SelfDeregisterAble.
    /// </summary>
    /// <remarks>
    /// When subscribing to the event, you do not create new WeakEventHandler directly,
    /// but you call some WeakEventHandler.Create or WeakEventHandler.CreateGeneric overload.
    /// </remarks>
    /// <param name="callback">Represents the method that will handle an event for this particular WeakEventHandler subscriber.</param>
    protected WeakEventHandler(EventHandler<TEventArgs> callback)
        : this(null, null, callback) { }

    /// <summary>
    /// Constructor creating a new instance of WeakEventHandler, with generic callback.
    /// Generated object is SelfDeregisterAble.
    /// </summary>
    /// <remarks>
    /// When subscribing to the event, you do not create new WeakEventHandler directly,
    /// but you call some WeakEventHandler.Create or WeakEventHandler.CreateGeneric overload.
    /// </remarks>
    /// <param name="eventOwner">The owner (publisher) of the event.</param>
    /// <param name="eventName">The string containing the name of an event which is declared or inherited by the current <paramref name="eventOwner"/> type.</param>
    /// <param name="callback">Represents the method that will handle an event for this particular WeakEventHandler subscriber.</param>
    protected WeakEventHandler(object eventOwner, string eventName, EventHandler<TEventArgs> callback)
        : this(eventOwner, eventName, callback as Delegate) { }

    /// <summary>
    /// Constructor creating a new instance of WeakEventHandler, with non-generic callback.
    /// Generated object is not SelfDeregisterAble.
    /// </summary>
    protected WeakEventHandler(EventHandler callback)
        : this(null, null, callback) { }

    /// <summary>
    /// Constructor creating a new instance of WeakEventHandler, with non-generic callback.
    /// Generated object is SelfDeregisterAble.
    /// </summary>
    /// <param name="eventOwner">The owner (publisher) of the event.</param>
    /// <param name="eventName">The string containing the name of an event which is declared or inherited by the current <paramref name="eventOwner"/> type.</param>
    /// <param name="callback">Represents the method that will handle an event for this particular WeakEventHandler subscriber.</param>
    protected WeakEventHandler(object eventOwner, string eventName, EventHandler callback)
        : this(eventOwner, eventName, callback as Delegate) { }

    private WeakEventHandler(object eventOwner, string eventName, Delegate callback)
    {
        // --- 1. initialize callback target data
        ArgumentNullException.ThrowIfNull(callback);

        // In the scope of this method the callback.Target cannot be collected by GC,  
        // because existing Delegate callback refers to it. 
        // Hence, callback.Target is null if and only if the _targetMethod is static
        // ( at that case, callback.Target is null from the very beginning )
        //
        // Note that _targetReference is created by following code even in that case, 
        // and this in on purpose
        // ( to be able to recognize registered and deregistered weak handlers ).
        _targetMethod = callback.Method;
        _targetReference = TargetMethod.IsStatic || (callback.Target == null)
            ? new WeakReference(null)
            : new WeakReference(callback.Target);

        if (eventOwner != null)
        {
            if (eventOwner is Type evOwnerAsType)
            { // In this case the eventOwner argument is actually a Type. 
              // The caller provides such argument if the event is its static member, 
              // hence there is no actual instance related (owning) the event.
                _eventOwner = null;
                _ownerType = evOwnerAsType;
            }
            else
            {  // classic case when the event is non-static
                _eventOwner = eventOwner;
                _ownerType = EventOwner.GetType();
            }

            // if provided event name is not null, try to initialize event representation
            if (!string.IsNullOrEmpty(eventName))
            {
                _eventInfo = _ownerType.GetEvent(eventName);
                // if could not find corresponding _eventInfo ( if _eventInfo == null)
                if (!IsSelfDeregisterAble)
                {
                    string strErr = $"Failed to retrieve the event '{eventName}' from the type '{_ownerType}'";
                    throw new InvalidOperationException(strErr);
                }
            }
        }
    }
    #endregion // Constructors

    #region Public
    #region Public properties

    /// <summary> Returns true if TargetReference is null, false otherwise. </summary>
    /// <value> True if this object is deregistered, false if not. </value>
    /// <seealso cref="MarkAsDeregistered"/>
    /// <seealso cref="IsSelfDeregisterAble"/>
    public bool IsDeregistered { get => (null == TargetReference); }
    #endregion // Public properties

    #region Protected properties

    /// <summary>
    /// Returns the weak reference to the target ( the subscriber of the event ).
    /// The actual value of WeakReference.Target may be null, if the target method is static.
    /// </summary>
    protected WeakReference TargetReference { get => _targetReference; }

    /// <summary>
    /// Returns the information about that callback method that will be called, 
    /// as determined from the provided callback Delegate.
    /// </summary>
    protected MethodInfo TargetMethod { get => _targetMethod; }

    /// <summary>
    /// Returns the 
    /// <see href="http://msdn.microsoft.com/en-us/library/system.reflection.eventinfo.aspx"> EventInfo </see>
    /// object which provides access to subscribed event metadata. <br/>
    /// The returned value has been determined from the arguments of constructor (the event name or event owner).
    /// It may be null if the constructor has not received enough information.
    /// </summary>
    protected EventInfo EventInfo { get => _eventInfo; }

    /// <summary>
    /// The owner (publisher) of the subscribed event, that has been specified as actual argument of constructor.
    /// </summary>
    protected object EventOwner { get => _eventOwner; }

    /// <summary>
    /// Returns true if this object can itself remove from related event 
    /// ( from its list of subscribers ), because it has initialized EventInfo.
    /// </summary>
    /// <remarks>
    /// DO NOT confuse this with the other information about the current target status, 
    /// retrieved from TargetReference.Target. See the property GetTargetStatus.
    /// </remarks>
    /// <seealso cref="IsDeregistered"/>
    /// <seealso cref="IsDeregisterAbleNow"/>
    /// <seealso cref="GetTargetStatus"/>
    protected bool IsSelfDeregisterAble
    {
        get { return (null != _eventInfo); }
    }

    /// <summary>
    /// Get what we know about the callback object (the subscriber) right now. <br/>
    /// Note that for static target we always return TargetStatus.TargetIsStatic, regardless 
    /// whether the object is still registered, or deregistered ( TargetReference == null).
    /// </summary>
    protected TargetStatus GetTargetStatus
    {
        get
        {
            TargetStatus result;

            if (TargetMethod.IsStatic)
            {  // static target
                result = TargetStatus.TargetIsStatic;
            }
            else if (null == TargetReference)
            { // is deregistered
                result = TargetStatus.TargetIsDetached;
            }
            else if (null == TargetReference.Target)
            { // GC has collected the target
                result = TargetStatus.TargetIsCollected;
            }
            else
            {  // target still alive
                result = TargetStatus.TargetIsAlive;
            }
            return result;
        }
    }
    #endregion // Protected properties
    #endregion // Properties

    #region Methods
    #region Public Methods
    #region Object-Creating

    /// <summary>
    /// Creating of non-generic event handler, based on non-generic callback.<br/>
    /// Generated object is not SelfDeregisterAble.<br/>
    /// This is a non-generic version of the method.<br/>
    /// </summary>
    /// <param name="callback">Represents the method that will handle an event for this particular WeakEventHandler subscriber.</param>
    /// <returns>An EventHandler delegate which belongs to newly created instance of non-generic WeakEventHandler.</returns>
    public static EventHandler Create(EventHandler callback)
    {
        return new WeakEventHandler<TEventArgs>(callback).FnHandlerNonGeneric;
    }

    /// <summary>
    /// Creating of generic event handler, based on generic callback.<br/>
    /// Generated object is not SelfDeregisterAble.<br/>
    /// This is a generic version of the method.<br/>
    /// </summary>
    /// <param name="callback">Represents the method that will handle an event for this particular WeakEventHandler subscriber.</param>
    /// <returns>An EventHandler delegate which belongs to newly created instance of generic WeakEventHandler.</returns>
    public static EventHandler<TEventArgs> CreateGeneric(EventHandler<TEventArgs> callback)
    {
        return new WeakEventHandler<TEventArgs>(callback).FnHandlerGeneric;
    }

    /// <summary>
    /// Creating of non-generic event handler, based on non-generic callback.<br/>
    /// Generated object is SelfDeregisterAble.<br/>
    /// This is a non-generic version of the method.<br/>
    /// </summary>
    /// <param name="eventOwner">The owner (publisher) of the event.</param>
    /// <param name="eventName">The string containing the name of an event which is declared or inherited by the current <paramref name="eventOwner"/> type.</param>
    /// <param name="callback">Represents the method that will handle an event for this particular WeakEventHandler subscriber.</param>
    /// <returns>An EventHandler delegate which belongs to newly created instance of non-generic WeakEventHandler.</returns>
    public static EventHandler Create(object eventOwner, string eventName, EventHandler callback)
    {
        return new WeakEventHandler<EventArgs>(eventOwner, eventName, callback).FnHandlerNonGeneric;
    }

    /// <summary>
    /// Creating of generic event handler, based on generic callback.<br/>
    /// This is a generic version of the method.<br/>
    /// </summary>
    /// <param name="eventOwner"> The owner (publisher) of the event. </param>
    /// <param name="eventName"> The string containing the name of an event which is declared or inherited by the
    /// current <paramref name="eventOwner"/> type. </param>
    /// <param name="callback"> Represents the method that will handle an event for this particular
    /// WeakEventHandler subscriber. </param>
    /// <returns>
    /// An EventHandler delegate which belongs to newly created instance of generic WeakEventHandler.
    /// </returns>
    public static EventHandler<TEventArgs> CreateGeneric(object eventOwner, string eventName, EventHandler<TEventArgs> callback)
    {
        return new WeakEventHandler<TEventArgs>(eventOwner, eventName, callback).FnHandlerGeneric;
    }
    #endregion // Object-Creating

    #region Invocation_List_modifiers

    /// <summary>
    /// Removes from the invocation list if given event handler all delegates whose target is WeakEventHandler,
    /// and that target returns <see cref="IsDeregisterAbleNow"/> value true. <br/>
    /// Note: <br/>
    /// This is a non-generic version of the method (while the similar overload is generic).
    /// </summary>
    /// <param name="handler">The event handler that is subject of 'purification'.</param>
    /// <param name="bIncludeStaticTarget">
    ///  If this argument is true, the 'purification' will include WeakEventHandler(s) that have static target method.<br/>
    ///  If this argument is false, the 'purification' will exclude such WeakEventHandler(s).
    /// </param>
    /// <returns> true if any change has been done, false otherwise</returns>
    public static bool InvocationListPurify(
        ref EventHandler handler,
        bool bIncludeStaticTarget)
    {
        bool result = false;

        if (null != handler)
        {
            result = EventHandlerUtilities.InvocationListClear(
              ref handler,
              deleg => (deleg.Target as WeakEventHandler<EventArgs>).NullSafe(d => d.IsDeregisterAbleNow(bIncludeStaticTarget)));
        }
        return result;
    }

    /// <summary>
    /// Remove from invocation list all delegates whose target is WeakEventHandler,
    /// and that target returns <see cref="IsDeregisterAbleNow"/> value true. <br/>
    /// Note:<br/>
    /// This is a generic version of the method (while the similar overload is non-generic).
    /// </summary>
    /// <param name="handler">The event handler that is subject of 'purification'.</param>
    /// <param name="bIncludeStaticTarget">
    ///  If this argument is true, the 'purification' will include WeakEventHandler(s) that have static target method.<br/>
    ///  If this argument is false, the 'purification' will exclude such WeakEventHandler(s).
    /// </param>
    /// <returns> true if any change has been done, false otherwise</returns>
    public static bool InvocationListPurify(
        ref EventHandler<TEventArgs> handler,
        bool bIncludeStaticTarget)
    {
        bool result = false;

        if (null != handler)
        {
            // In this case, the code calls the generic NullSafe, which generally takes care about the null input argument.
            result = EventHandlerUtilities.InvocationListClear(
              ref handler,
              deleg => (deleg.Target as WeakEventHandler<TEventArgs>).NullSafe(d => d.IsDeregisterAbleNow(bIncludeStaticTarget)));
        }
        return result;
    }
    #endregion // Invocation_List_modifiers
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// Return true if the handler could be deregistered
    /// ( regardless which way - either by self-deregistering, or by the method InvocationListPurify )
    /// </summary>
    /// <param name="includeStaticTarget">
    ///  If this argument is true,  and if the target method is static, the returned value is !this.IsDeregistered.<br/>
    ///  If this argument is false, and if the target method is static, the returned value is false.<br/>
    /// </param>
    /// <returns>True if this instance could be deregistered now; false otherwise.</returns>
    /// <seealso cref="IsSelfDeregisterAble"/>

    protected bool IsDeregisterAbleNow(bool includeStaticTarget)
    {
        var status = GetTargetStatus;
        return status switch
        {
            TargetStatus.TargetIsStatic => includeStaticTarget && !IsDeregistered,
            TargetStatus.TargetIsCollected => true,
            _ => false
        };
    }

    /// <summary>
    /// This general event handler delegates the callback call from the event to the existing 
    /// target (subscriber), if there is any.
    /// In order to do that, creates on the fly a delegate of the type Action{object, EventArgs}
    /// with arguments the related target and TargetMethod; and uses that callback.
    /// In case the target is no longer reachable, the weak handler attempts to deregister itself.
    /// </summary>
    /// <remarks>
    /// Note that for case TargetMethod.IsStatic the handler never tries to deregister itself.
    /// </remarks>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An object that contains the event data.</param>
    protected void FnHandlerNonGeneric(object sender, EventArgs args)
    {
        Action<object, EventArgs> callback;
        Debug.Assert(!this.IsDeregistered);
        object target = TargetReference.Target;

        if ((target != null) || TargetMethod.IsStatic)
        {
            callback = (Action<object, EventArgs>)Delegate.CreateDelegate(
              typeof(Action<object, EventArgs>), target, TargetMethod, true);
            callback?.Invoke(sender, args);
        }
        else
        {
            TrySelfDeregister();
        }
    }

    /// <summary>
    /// This general event handler delegates the callback call from the event to the existing 
    /// target (subscriber), if there is any.
    /// In order to do that, it creates on the fly a delegate of the type Action{object, TEventArgs}
    /// with arguments the related target object and TargetMethod; and uses that callback delegate to call.
    /// In case the target is no longer reachable, the weak handler attempts to deregister itself.
    /// </summary>
    /// <remarks>
    /// Note that for case TargetMethod.IsStatic the handler never tries to deregister itself.
    /// </remarks>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An object that contains the event data.</param>

    protected void FnHandlerGeneric(object sender, TEventArgs args)
    {
        Action<object, TEventArgs> callback;
        Debug.Assert(!this.IsDeregistered);
        object target = TargetReference.Target;

        if ((target != null) || TargetMethod.IsStatic)
        {
            callback = (Action<object, TEventArgs>)Delegate.CreateDelegate(
              typeof(Action<object, TEventArgs>), target, TargetMethod, true);

            callback?.Invoke(sender, args);
        }
        else
        {
            TrySelfDeregister();
        }
    }

    /// <summary>
    /// Deregisters itself, if the handler is deregister-able and has not been deregistered yet.
    /// </summary>
    /// <returns>True if the call deregistered the handler, false otherwise</returns>
    /// <seealso cref="SelfDeregister"/>
    protected bool TrySelfDeregister()
    {
        // Note: The calling of IsDeregisterAbleNow here is always with arg. false,
        // since for case TargetMethod.IsStatic the handler never should try to deregister itself.
        // See also comments and code for FnHandlerNonGeneric and FnHandlerGeneric
        if (IsSelfDeregisterAble && IsDeregisterAbleNow(false))
        {
            SelfDeregister();
            return true;
        }
        return false;
    }

    /// <summary>
    /// The WeakEventHandler deregisters itself in this method, using EventInfo that has been retrieved in the
    /// constructor.
    /// </summary>
    /// <remarks>
    /// The instance MUST be self-deregisterAble, otherwise the call throws an exception
    /// InvalidOperationException. If you are not sure, call the method TrySelfDeregister.
    /// </remarks>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    /// <seealso cref="TrySelfDeregister"/>
    protected virtual void SelfDeregister()
    {
        if (!IsSelfDeregisterAble)
        {
            throw new InvalidOperationException("This WeakEventHandler cannot be self-deregistered");
        }
        if (_targetReference.Target != null)
        {
            throw new InvalidOperationException("WeakEventHandler must not deregister itself if its target is still reachable");
        }

        if (IsDeregistered) return;  // Nothing to do, deregistered already

        // Originally, I have attempted here to retrieve the invocation list of the event
        // and operate on that, trying to mix the approaches described in
        // http://msdn.microsoft.com/en-us/library/ms228976.aspx
        // and 
        // http://www.expert.tc/topic.php?id=37856
        //
        // In more detail, I have tried:
        // 1. To get get some FieldInfo field for the event
        // 2. For that FieldInfo field get its value (the delegate), like
        //    Delegate eventDelegate = (Delegate)fi.GetValue(EventOwner)
        // 3. Then get the invocation list, like
        //   Delegate[] list = eventDelegate.GetInvocationList();
        // 4. and then enumerate the invocation list, finding the element where the target is this
        //    foreach (Delegate g in list) { if (g.Target == this) ... }
        // 
        // Unfortunately, the very first step could not be done.
        // In quite general, from the outside, an event exposed by a class is just
        // a pair of functions: EventName_Add and EventName_Remove. 
        // Unless you're going to get into decompiling and interpreting the IL of those functions,
        // there's simply no way to locate the delegate whose invocation list 
        // you'd need to enumerate. Worse, there's no requirement that there even be a delegate.
        // The event producer may actually store the callback information 
        // in a structure other than a delegate (for example, it might 
        // actually pass the delegates on to another object if it's simply forwarding 
        // an event from a contained object).
        //
        // Fortunately, there is much simpler way:
        // i/ Get the remove accessor method and create its arguments
        // ii/ invoke the remove method
        // iii/ in the end, the particular WeakEventHandler should be marked as deregistered

        MethodInfo removeHandler = EventInfo.GetRemoveMethod(true);
        object[] removeArgs = (EventInfo.EventHandlerType == typeof(EventHandler))
            ? [new EventHandler(FnHandlerNonGeneric)]
            : [new EventHandler<TEventArgs>(FnHandlerGeneric)];

        removeHandler.Invoke(_eventOwner, removeArgs);
        MarkAsDeregistered();
    }

    /// <summary> Marks this instance as deregistered. </summary>
    /// <seealso cref="IsDeregistered"/>
    protected void MarkAsDeregistered()
    {
        _targetReference = null;
    }
    #endregion // Protected Methods
    #endregion // Methods
}