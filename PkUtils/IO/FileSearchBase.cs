/***************************************************************************************************************
*
* FILE NAME:   .\IO\FileSearchBase.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:  The file contains interface IFileSearch and abstract class FileSearchBase
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using PK.PkUtils.DataStructures;
using ParseStage = PK.PkUtils.IO.FileSearchBase.ParseStageEventArgs.ParseStage;

#pragma warning disable IDE0290     // Use primary constructor

namespace PK.PkUtils.IO;

/// <summary> Defines basic interface of class with file-search capabilities. </summary>
[CLSCompliant(true)]
public interface IFileSearch
{
    /// <summary>
    /// Search all files under the given directory. Will search file system recursively, if searchOption
    /// requires that.
    /// </summary>
    /// <param name="dirRoot"> The directory to begin the search with. </param>
    /// <param name="pattern"> A filename which can include wildcard characters. For instance "*.dll". 
    /// If this argument is null or empty, pattern "*.*" will be used instead. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// 
    /// <returns> Resulting sequence of files that can be iterated. </returns>
    IEnumerable<FileInfo> SearchFiles(DirectoryInfo dirRoot, string pattern, SearchOption searchOption);

    /// <summary>
    /// Search for given file pattern all directories under the given directory (including that root).
    /// Will search file system recursively if searchOption is FileSearchOption.AllDirectories.
    /// </summary>
    /// <param name="dirRoot"> The directory to begin the search with. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// <param name="includeTop"> true to include top directory in returned enumeration. </param>
    ///
    /// <returns> Resulting sequence of directories that can be iterated. </returns>
    IEnumerable<DirectoryInfo> SearchDirectories(DirectoryInfo dirRoot, SearchOption searchOption,
      bool includeTop);
}

/// <summary> A file-search class supporting events. </summary>
[CLSCompliant(true)]
public abstract class FileSearchBase : NotifyPropertyChanged, IFileSearch
{
    #region Public Interface
    #region Typedefs

    /// <summary> An abstract class used as base of other event-args-like classes. </summary>
    public abstract class ParseFolderEventArgs : CancelEventArgs
    {
        #region Public Interface
        #region Properties

        /// <summary> Gets or sets the folder information. </summary>
        public DirectoryInfo DirInfo { get; private set; }

        /// <summary> The parsed directory level. </summary>
        public int Level { get; private set; }
        #endregion // Properties
        #endregion // Public Interface

        #region Protected Interface
        #region Constructor(s)

        /// <summary> Constructor initializing essential properties. </summary>
        /// <param name="di"> The folder information. </param>
        /// <param name="nParseLevel"> The parsed directory level. Zero means top folder,
        ///  one its immediate sub-folder etc. </param>
        protected ParseFolderEventArgs(DirectoryInfo di, int nParseLevel)
        {
            this.DirInfo = di;
            this.Level = nParseLevel;
        }
        #endregion // Constructor(s)
        #endregion // Protected Interface
    }

    /// <summary> EventArguments used with event that is being raised during processing the folder.
    /// Contains additional information regarding folder processing stage. </summary>
    public class ParseStageEventArgs : ParseFolderEventArgs
    {
        #region Public Interface
        #region Typedefs(s)

        /// <summary>
        /// The enum is used as an argument of event raised when processing folder
        /// ( upon entering/exiting the specific folder ).
        /// </summary>
        public enum ParseStage
        {
            /// <summary>
            /// Used when entering the folder
            /// </summary>
            Enter = 0,

            /// <summary>
            /// Used when exiting the folder
            /// </summary>
            Exit = 1,
        }
        #endregion // Typedefs(s)

        #region Constructor(s)

        /// <summary> Constructor initializing essential properties. </summary>
        /// <param name="di"> The folder information. </param>
        /// <param name="nParseLevel"> The parsed directory level. Zero means top folder,
        ///  one its immediate sub-folder etc. </param>
        /// <param name="aStage"> The stage of folder processing. </param>
        public ParseStageEventArgs(DirectoryInfo di, int nParseLevel, ParseStage aStage)
          : base(di, nParseLevel)
        {
            this.Stage = aStage;
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> Gets or sets the stage of folder processing. </summary>
        public ParseStage Stage { get; private set; }
        #endregion // Properties
        #endregion // Public Interface
    }

    /// <summary> EventArguments used with event that is being raised if an error occurred
    /// during processing (parsing) the folder.
    /// Contains additional information regarding folder processing stage. </summary>
    public class ParseErrorEventArgs : ParseFolderEventArgs
    {
        #region Public Interface

        #region Constructor(s)

        /// <summary> Constructor initializing essential properties. </summary>
        /// <param name="di"> The folder information. </param>
        /// <param name="nParseLevel"> The parsed directory level. Zero means top folder,
        ///  one its immediate sub-folder etc. </param>
        /// <param name="ex"> The exception that occurred when accessing (parsing) the folder. </param>
        public ParseErrorEventArgs(DirectoryInfo di, int nParseLevel, SystemException ex)
          : base(di, nParseLevel)
        {
            this.EncounteredException = ex;
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> Gets or sets a value indicating whether the caller should retry the attempt. </summary>
        public bool Retry { get; set; }

        /// <summary> Gets or sets the encountered exception. </summary>
        public SystemException EncounteredException { get; private set; }

        #endregion // Properties
        #endregion // Public Interface
    }
    #endregion // Typedefs

    #region Public Properties

    /// <summary> Gets or sets a value indicating whether current parsing is canceled. </summary>
    /// <remarks> Instead of assigning the property value, derived classes should use 
    ///  virtual method <see cref="Cancel"/> to cancel current search ( with setting the property) and
    ///  virtual method <see cref="ClearCanceled"/> to clear IsCanceled flag;
    ///  </remarks>
    /// <seealso cref="Cancel()"/>
    public bool IsCanceled
    {
        get { return _canceled; }
        protected set { SetField(ref _canceled, value, nameof(IsCanceled)); }
    }
    #endregion // Public Properties

    #region Public Events

    /// <summary> Event fired for all listeners interested in Directory Parse Stage change. </summary>
    public event EventHandler<ParseStageEventArgs> ParseStageEvent
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add { _parseStageHandler += value; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        remove { _parseStageHandler -= value; }
    }

    /// <summary> Event fired for all listeners interested in Directory Parse Error,
    ///           caused by SystemException. </summary>
    public event EventHandler<ParseErrorEventArgs> ParseErrorEvent
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add { _parseErrorHandler += value; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        remove { _parseErrorHandler -= value; }
    }
    #endregion // Public Events

    #region Public Methods

    /// <summary> Cancels the search. </summary>
    /// <remarks>Derived class may need to override this. In such case, either it should call base.Cancel()
    /// or to assign <see cref="IsCanceled"/> on its own.</remarks>
    /// <seealso cref="ClearCanceled"/>
    /// <seealso cref="IsCanceled"/>
    public virtual void Cancel()
    {
        IsCanceled = true;
    }

    /// <summary> Virtual method to be used or overwritten in derive classes.
    /// Current implementation assigns the <see cref="IsCanceled"/> property to false. </summary>
    /// <seealso cref="Cancel"/>
    public virtual void ClearCanceled()
    {
        IsCanceled = false;
    }
    #endregion // Public Methods
    #endregion // Public Interface

    #region IFileSearch members

    /// <summary>
    /// Search all files under the given directory. Will search file system recursively, if searchOption
    /// requires that.
    /// </summary>
    /// <param name="dirRoot"> The directory to begin the search with. </param>
    /// <param name="pattern"> A filename which can include wildcard characters. For instance "*.dll". 
    /// If this argument is null or empty, pattern "*.*" will be used instead. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// 
    /// <returns> Resulting sequence of files that can be iterated. </returns>
    public abstract IEnumerable<FileInfo> SearchFiles(DirectoryInfo dirRoot, string pattern,
      SearchOption searchOption);

    /// <summary>
    /// Search for given file pattern all directories under the given directory (including that root). 
    /// Will search file system recursively if searchOption is FileSearchOption.AllDirectories.
    /// </summary>
    /// <remarks>
    /// Just to avoid confusion: the <paramref name="searchOption"/> argument relates to the fact 
    /// 'sub-directories of what' will be returned. For instance, if dirRoot is 'c:\Tmp3', and
    /// searchOption has a value SearchOption.TopDirectoryOnly, the method will return all
    /// direct sub-directories of c:\Tmp3' ( and 'c:\Tmp3' itself if includeTop is true ).
    /// </remarks>
    /// <param name="dirRoot"> The directory to begin the search with. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// <param name="includeTop"> true to include top directory in returned enumeration. </param>
    /// 
    /// <returns> Resulting sequence of directories that can be iterated. </returns>
    public abstract IEnumerable<DirectoryInfo> SearchDirectories(DirectoryInfo dirRoot,
      SearchOption searchOption, bool includeTop);
    #endregion // IFileSearch members

    #region Protected Interface

    #region Protected Methods

    /// <summary> Raises the directory stage event <see cref="ParseStageEvent"/>. </summary>
    /// <remarks>
    /// Derived class can override method OnParseStage to be informed about such event, without
    /// subscribing event itself.
    /// </remarks>
    /// <param name="args"> Event information to send to registered event handlers. </param>
    protected virtual void OnParseStage(ParseStageEventArgs args)
    {
        EventHandler<ParseStageEventArgs> handler = this._parseStageHandler;
        handler?.Invoke(this, args);
    }

    /// <summary> Virtual method called the folder parse enter action. Calls <see cref="OnParseStage"/>
    /// thus raising the event ParseStageEvent. Assigns _Canceled if parsing should be canceled. <br/>
    /// When overriding OnFolderEnter in a derived class, be sure to call the base class's OnFolderEnter
    /// so that registered delegates receive the event.
    /// </summary>
    /// <param name="di"> The folder information. </param>
    /// <param name="nParseLevel"> The parsed directory level. Zero means top folder,
    ///  one its immediate sub-folder etc. </param>
    /// <returns> Returns true if parsing should continue, false if should be canceled. </returns>
    protected bool OnFolderEnter(DirectoryInfo di, int nParseLevel)
    {
        Debug.Assert(!IsCanceled);
        var args = new ParseStageEventArgs(di, nParseLevel, ParseStage.Enter);

        OnParseStage(args);
        if (args.Cancel)
        {
            Cancel();
        }

        return !IsCanceled;
    }

    /// <summary> Virtual method called the folder parse exit action. Calls <see cref="OnParseStage"/>
    /// thus raising the event ParseStageEvent. Assigns _Canceled if parsing should be canceled. <br/>
    /// When overriding OnFolderExit in a derived class, be sure to call the base class's OnFolderExit
    /// so that registered delegates receive the event.
    /// </summary>
    /// <param name="di"> The folder information. </param>
    /// <param name="nParseLevel"> The parsed directory level. Zero means top folder,
    ///  one its immediate sub-folder etc. </param>
    /// <returns> Returns true if parsing should continue, false if should be canceled. </returns>
    protected bool OnFolderExit(DirectoryInfo di, int nParseLevel)
    {
        Debug.Assert(!IsCanceled);
        var args = new ParseStageEventArgs(di, nParseLevel, ParseStage.Exit);

        OnParseStage(args);
        if (args.Cancel)
        {
            Cancel();
        }

        return !IsCanceled;
    }

    /// <summary> Raises the event <see cref="ParseErrorEvent"/>. </summary>
    /// <param name="args"> Event information to send to registered event handlers. </param>
    protected virtual void OnParseError(ParseErrorEventArgs args)
    {
        EventHandler<ParseErrorEventArgs> handler = this._parseErrorHandler;
        handler?.Invoke(this, args);
    }

    /// <summary>
    /// Constructs new <see cref="ParseErrorEventArgs"/> instance and raises the event
    /// <see cref="ParseErrorEvent"/>. If upon returning from raising the event these ParseErrorEventArgs have
    /// property Cancel set to true, it calls <see cref="Cancel"/>. </summary>
    /// 
    /// <param name="di"> The folder information. </param>
    /// <param name="nParseLevel"> The parsed directory level. Zero means top folder, one its immediate
    ///  sub-folder etc. </param>
    /// <param name="ex"> Details of the exception. </param>
    /// <param name="keepRetry"> [out] Will be true if the calling code should keep retry. </param>
    /// <returns> Returns true if parsing should continue, false if should be canceled. </returns>
    protected virtual bool OnParseError(DirectoryInfo di, int nParseLevel, SystemException ex, out bool keepRetry)
    {
        Debug.Assert(!IsCanceled);
        var args = new ParseErrorEventArgs(di, nParseLevel, ex);

        OnParseError(args);
        if (args.Cancel)
        {
            Cancel();
            keepRetry = false;
        }
        else
        {
            keepRetry = args.Retry;
        }

        return !IsCanceled;
    }
    #endregion // Protected Methods
    #endregion // Protected Interface

    #region Private Members
    #region Private Fields

    /// <summary> Backing field of property <see cref="IsCanceled"/>. </summary>
    private bool _canceled;

    /// <summary>
    /// The event handler representing the event <see cref="ParseStageEvent"/>, which is raised 
    /// to subscribers when parsing goes through specific Parse Stage.
    /// </summary>
    private EventHandler<ParseStageEventArgs> _parseStageHandler;

    /// <summary>
    /// The event handler the event <see cref="ParseErrorEvent"/>, which is raised
    /// to subscribers when SystemException has occurred.
    /// </summary>
    private EventHandler<ParseErrorEventArgs> _parseErrorHandler;
    #endregion // Private Fields
    #endregion // Private Members
}

#pragma warning restore IDE0290     // Use primary constructor