The project TestWeakRef tests WeakReference Event Handlers implementation.

Internally, this is done by generating lots of 'TargetShortLiving' per timer tick;
with these object subscribing to several events:
i/ event EventHandler evResize
  This event is declared in the TestForm and raised every time the form is resized.
ii/ event EventHandler<EventArgs> evSingleClick
  This event is declared in the TestForm and raised every time the form is single-clicked.
iii/ EventHandler<EventArgs> evDoubleClick
  This event is declared in the TestForm and raised every time the form is double-clicked.
iv/ SourceStatic.evStaticEvent 
  This event is declared in the static class SourceStatic and raised every time 
  the user manually pushes a specific button in the TestForm.
  
Currently, the UI supports following four scenarios:

a/ "None" subscription of created objects.  
    All new objects have no relation to listed events.
    Any time you invoke GC.Collect from UI by "Enforce GC.Collect" button,
    all these objects are collected.
    
b/ "Strong references" subscription of created objects.
    In that case all new objects are in the Invocation List of listed events.
    Therefore, invoking GC.Collect just itself does not collect these objects.
	Further details:
    - If you destroy invocation list of the related event (by "Destroy InvocationLists" button).
      all 'TargetShortLiving' become unreachable and next GC.Collect will collect them.
	- The button "Clear DeregisterAble Handlers" cannot make any change in this case.
    
c/ "Weak simple references" subscription of created objects.
    In this case, the invocation list of the events contains instances
    of delegates (EventHandlers), each of them has strong references to 
    related WeakEventHandler, which in turn just indirectly (via WeakReference) points 
    to TargetShortLiving.
    Hence, GC.Collect anytime can collect all instances of TargetShortLiving.
    However, this approach is far from ideal, since the invocation list
    of event still contains all instances of EventHandler that refer to
    WeakEventHandler, and these instances are never cleared by GC.Collect.
    Further details:
    - If you destroy invocation list of the related event (by "Destroy InvocationLists" button).
      all 'TargetShortLiving' become unreachable and next GC.Collect will collect them.
	- The button "Clear DeregisterAble Handlers" calls WeakEventHandler.InvocationListPurify,
	  which remove from the invocation list all delegates whose target is WeakEventHandler,
      and that target returns IsDeregisterAbleNow (which means that target has been collected).
	  Hence, the invocation lists and related WeakEventHandlers could be cleared in several steps:
	  i/ first call of GC.Collect collects just TargetShortLiving objects 
	    (but their WeakEventHandlers remain still reachable).
	  ii/ subsequent call of "Clear DeregisterAble Handlers" will remove from invocation list 
	   the related delegates, and their target WeakEventHandler will became unreachable
	  iii/ subsequent call of GC.Collect can collect WeakEventHandlers that were cleared that way.
    
d/ "Weak self-deregisterAble references" subscription of of created objects.
    This case is similar to c/, however, now all created WeakEventHandlers
    are self-deregisterAble.
    When handling the event in WeakEventHandler with self-deregistering capability, 
    the method can recognize that the original event target has been garbage-collected, 
    and will attempt to self-deregister the WeakEventHandler,
    which includes removing the 'regular' delegate object from the invocation list of the event.
    See more details in WeakEventHandler source code.
    Further details: otherwise this is similar with the case c/


---- Reference:  ------------------------------------------------

* WeakReference Event Handlers by Paul Stovell
  http://www.paulstovell.com/weakevents
  
* Weak Events in C# by Daniel Grunwald
  http://www.codeproject.com/KB/cs/WeakEvents.aspx
  
* Weak References
  http://msdn.microsoft.com/en-us/library/ms404247.aspx
  
* How to hook up an event 
  http://msdn.microsoft.com/en-us/library/ms228976.aspx
  
* Multicast Delegate Internals
  http://msdn.microsoft.com/en-us/vcsharp/bb508935
  
* Garbage Collection: Automatic Memory Management in the Microsoft .NET Framework, Part 1 and 2
  by Jeffrey Richter
  http://msdn.microsoft.com/en-us/magazine/bb985010.aspx
  http://msdn.microsoft.com/en-us/magazine/bb985011.aspx
  
* ICLRGCManager Interface
  http://msdn.microsoft.com/en-us/library/ms164371(v=VS.80).aspx
  