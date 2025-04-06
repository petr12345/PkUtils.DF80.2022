/***************************************************************************************************************
*
* FILE NAME:   .\Interfaces\ISuspendable.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of interface ISuspendable
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Suspendable, Haltable
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// ISuspendable defines a functionality used for suspending or resuming some object activity.<br/>
/// It is assumed the object DOES count the amount of suspend calls,
/// and one has to call Revive the same times in order to revive successfully.
/// </summary>
[CLSCompliant(true)]
public interface ISuspendable
{
    /// <summary>
    /// Is the entity suspended ?
    /// </summary>
    bool IsSuspended { get; }

    /// <summary>
    /// Suspend the entity.  
    /// </summary>
    /// <returns>The amount of total suspends acquired</returns>
    int Suspend();

    /// <summary>
    /// Revive the entity.
    /// </summary>
    /// <returns>The amount of remaining suspends </returns>
    int Revive();
}

/// <summary>
/// IHaltable defines a functionality used for halting or resuming some object activity.<br/>
/// Unlike with <see cref="ISuspendable "/> interface, it is assumed the object does NOT count the halt calls,
/// and just one Resume call is sufficient to continue the activity.
/// </summary>
[CLSCompliant(true)]
public interface IHaltable
{
    /// <summary> Is the entity suspended (halted) ? </summary>
    ///
    /// <value> true if this object is halted, false if not. </value>
    /// <seealso cref="Halt"/>
    bool IsHalted { get; }

    /// <summary>
    /// Halt the activity.  <br/>
    /// If the entity was not in halted state before the call, returns true on successful halt, or false on failure.<br/>
    /// If the entity was in a halted state already, just return true with no changes. <br/>
    /// </summary>
    /// <returns>True if the object is halted upon returning, false otherwise.</returns>
    /// <seealso cref="IsHalted"/>
    bool Halt();

    /// <summary>
    /// Resume the activity. <br/>
    /// If the entity was in halted state before the call, returns true on successful resuming, or
    /// false on failure.<br/>
    /// If the entity was not in a halted state already, just return true with no changes.<br/> </summary>
    ///
    /// <returns> true if the object could get into the resumed state; false if not. </returns>
    /// <seealso cref="IsHalted"/>
    bool Resume();
}
