// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary> Interface defining a general 'data-keeper' (data repository) functionality.<br/>
/// The repository can distinguish two exclusive states of the data:<br/>
/// - data are attached. In this case this object is not an owner of the data, and methods
/// <see cref="Forfeit"/> and <see cref="IDisposable.Dispose"/>(which usually should just calls Forfeit ) will call
/// <see cref="Detach"/>.  <br/>
/// - data are owned by this object. In this case methods <see cref="Forfeit"/> and <see cref="IDisposable.Dispose"/>(which
/// just calls Forfeit ) should call some protected or private method that disposes these data.
/// </summary>
///
/// <remarks> You are likely to use some Repository  implementation, if some of your object  "A"
/// manipulates some other data  "B" ( for instance a Form working with a connection), and in some
/// scenarios "A" becomes an owner of "B" , while in other scenarios it is not an owner.  <br/>
///  With this rather common usability pattern, the object  "A" could use prepared implementation for IRepository{B},
///  thus saving an additional boolean variable and without a need to re-implement the logic.
/// </remarks>
///
/// <typeparam name="T"> The type of the data that is held by or attached to this Repository. </typeparam>
[CLSCompliant(true)]
public interface IRepository<T> : IDisposable
{
    /// <summary> Gets a value indicating whether this object has any data of type  T (either attached  or owned).</summary>
    ///
    /// <value> true if this object has data, false if not. </value>
    bool HasData { get; }

    /// <summary> Gets a value indicating whether this object has attached data. </summary>
    ///
    /// <value> true if this object has attached data, false if not. </value>
    bool IsAttached { get; }

    /// <summary> Get currently owned or attached data. Returns null  if there are no such data.</summary>
    ///
    /// <value> The data. </value>
    T Data { get; }

    /// <summary> Keep the data ( that means became an owner of it ). <br/>
    /// Throws InvalidOperationException if there are already any data owned or attached. </summary>
    ///
    /// <remarks> If the data are owned, the methods <see cref="Forfeit"/> and <see cref="IDisposable.Dispose"/>
    /// (which just calls Forfeit ) should call some protected or private method that disposes these data.  <br/>
    /// Otherwise, if the data are attached, these methods call <see cref="Detach"/>. </remarks>
    ///
    /// <seealso cref="Attach"/>
    ///
    /// <param name="data"> The data that will be owned by this repository object. </param>
    void Keep(T data);

    /// <summary> Attach the data  ( that means do NOT became an owner of it ). <br/>  <br/>
    /// Throws InvalidOperationException if there are already any data owned or attached. </summary>
    ///
    /// <remarks> If the data are just attached, the methods <see cref="Forfeit"/> and
    /// <see cref="IDisposable.Dispose"/>( which just calls Forfeit ) will call <see cref="Detach"/>.
    /// <br/> </remarks>
    ///
    /// <seealso cref="Keep"/>
    ///
    /// <param name="data"> The data being attached to this repository object. </param>
    void Attach(T data);

    /// <summary>
    /// Detach the data previously attached and return these data. <br/>
    /// Should not make any change and return null in case there are no data attached 
    /// ( which means either there are no data at all, or the instance is actually the owner of the data ).
    /// </summary>
    /// <returns> Previously attached data or null.. </returns>
    T Detach();

    /// <summary>
    /// Forfeit the data ( get rid of the data ). <br/>
    /// In case the data were attached, calls <see cref="Detach"/>. <br/>
    /// In case the data were not attached but this object was the owner of the data, 
    /// should call some protected or private method that disposes these data.<br/>
    /// </summary>
    /// <seealso cref="Detach"/>
    void Forfeit();
};
