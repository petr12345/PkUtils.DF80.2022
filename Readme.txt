PKNETUTILS - SOURCE TREE
================================

Repository overview and project inventory

1. PURPOSE AND SCOPE
--------------------

This source tree contains two related C# libraries and their associated tests.
The foundation is the PKNetUtils utility library, whose source code
resides in PkUtils/ and whose compiled assembly is named PK.PkUtils.
The PKCommands library, in PKCommands/, builds on PKNetUtils and provides
a framework for implementing and processing commands.

The repository also contains automated unit tests for both libraries and a
collection of demonstration and test applications showing how the libraries
 are used. The demonstrations are not necessarily automated tests:
most are interactive Windows Forms applications or console programs.

Main directories:

  PkUtils/                 PKNetUtils (PK.PkUtils) reusable utility library
  PKCommands/              PK.Commands command-processing library
  PKCommands.NUnitTests/   Automated NUnit tests for PKCommands
  PKCommands_Tests/        Executable examples for PKCommands
  PkUtils.NUnitTests/      Automated NUnit tests for PKNetUtils
  PkUtils.UnitTests/       Automated MSTest tests for PKNetUtils
  PkUtils_Tests/           Examples, experiments, and sample applications

Supporting directories:

  KeyGen/                  Assembly signing key material and related scripts
  _Doc/                    Architecture diagrams and technical documentation
  _Lic/                    Licensing material
  _Scripts/                Auxiliary and historical project-maintenance scripts

2. BUILD NOTES
--------------

The source tree includes .NET 8 projects (.DF80.2022.csproj) and .NET 10
counterparts (.DF10.2026.csproj). Both configurations use the same C# sources.
Most projects target Windows and may require WinForms or Windows APIs. The
The TestSubstEdit example also includes native C++ projects. 

The test frameworks are intentionally distinct:
  PKCommands.NUnitTests/   NUnit
  PkUtils.NUnitTests/      NUnit
  PkUtils.UnitTests/       MSTest


3. PKUTILS/ - PKNETUTILS FOUNDATION LIBRARY
------------------------------------------

PkUtils/ implements the general-purpose PK.PkUtils library, providing reusable
.NET utilities and Windows-specific functionality. Its project definitions
are PkUtils.DF80.2022.csproj and PkUtils.DF10.2026.csproj.

4. PKCOMMANDS/ - COMMAND-PROCESSING LIBRARY
------------------------------------------

PKCommands/ implements the PK.Commands assembly, which references PK.PkUtils.
It provides abstractions and shared processing code for command-oriented
applications rather than one particular executable command.

5. PKCOMMANDS.NUNITTESTS/ - AUTOMATED COMMAND TESTS
--------------------------------------------------

PKCommands.NUnitTests/ is the NUnit suite for the command-processing library.
It refers to the PKCommands project and exercises command parsing and handling.

6. PKCOMMANDS_TESTS/ - COMMAND APPLICATION EXAMPLES
---------------------------------------------------

PKCommands_Tests/ contains the GZipTest/ sample executable. GZipTest uses the
PKCommands command framework and PKNetUtils services in a practical application.

7. PKUTILS.NUNITTESTS/ - AUTOMATED NUNIT TESTS
---------------------------------------------

PkUtils.NUnitTests/ is an automated NUnit test project referencing the PkUtils
library. Its tests are organized by the library facilities they cover.

8. PKUTILS.UNITTESTS/ - AUTOMATED MSTEST TESTS
---------------------------------------------

PkUtils.UnitTests/ is a separate automated MSTest test project for PKNetUtils.
It overlaps with, but is not identical to, the NUnit suite. These two projects
have different test-framework dependencies and provide separate sources.

9. PKUTILS_TESTS/ - EXAMPLE AND EXPERIMENT INVENTORY
----------------------------------------------------

PkUtils_Tests/ contains 32 immediate subdirectories. Each entry below names
one sample directory and summarizes what its actual projects demonstrate.
These samples are primarily developer-facing console or WinForms programs,
not unit-test suites. Many include local ReadMe.txt documentation; two have
HTML guides. They often exercise functionality from PkUtils, but some also
illustrate independent C# or Win32 programming techniques.

  TestBinding/
    A WinForms demonstration of data binding and BindingSource events,
    including interaction with a form and a singleton-backed data source.

  TestC#DoesShell_1/
    Explores Windows Shell APIs for special-folder paths, PIDLs, and Shell
    folder interfaces through an interactive WinForms application.

  TestC#DoesShell_2/
    Demonstrates ShellExecute and ShellFileOperation operations such as
    opening, editing, locating, copying, and moving files or folders.

  TestC#DoesShell_3/
    Demonstrates an ApplicationDesktopToolbar (Windows AppBar) whose docking
    edge can be adjusted through a WinForms interface.

  TestClipboardMonitor/
    Demonstrates ClipMonitorHook, which observes changes to the Windows
    clipboard and notifies interested subscribers.

  TestCloning/
    Demonstrates deep copying of object graphs, including cyclic references,
    using the JSON-backed CloneHelperBinary API and CloneHelperByContract.

  TestComparers/
    Compares FunctionalEqualityComparer and KeyEqualityComparer in
    case-insensitive LINQ Distinct operations.

  TestCompositeImages/
    Demonstrates a cancelable TPL Dataflow image-processing pipeline that
    loads source images and produces composite images in a WinForms UI.

  TestConcurrency/
    Compares counter updates without synchronization and with Interlocked,
    Monitor, ReaderWriterLock, ReaderWriterLockSlim, and wrapper classes.

  TestConsoleAppWithIcon/
    Demonstrates changing a console application's window icon and taskbar
    icon by combining resources, Win32 APIs, and NotifyIcon.

  TestDumperTextWriter/
    Shows how Trace output can be routed to a WinForms text box using
    DumperTextWriterAndTraceListener and DumperCtrlTextBoxWrapper.

  TestErrorDisplay/
    Demonstrates compile-time binding of optional parameter defaults when
    methods are called through an interface or concrete implementation.

  TestExceptionHelper/
    Exercises ExceptionHelper functionality for handling
    TargetInvocationException while preserving useful stack-trace context.

  TestFileLock/
    Demonstrates locking files and directories using Windows handles and
    inspecting processes associated with file locks.

  TestGetFiles/
    Compares recursive and non-recursive file/directory search
    implementations and reports their results from a console program.

  TestMsgBoxObserver/
    Demonstrates observing the lifecycle of Windows MessageBox dialogs
    through MsgBoxObserver and Windows system hooks.

  TestMsgBoxYesNoAll/
    Exercises MsgBoxYesNoAll and MsgBoxYesNoAllPanelBased custom dialogs,
    including variants of Yes/No/All-style responses.

  TestMultiSelectOwnerDraw/
    Demonstrates owner-drawn visual feedback for a multi-select TreeView,
    including projected selection of descendant nodes.

  TestMultiSelectTreeview/
    Demonstrates an enhanced TreeView with keyboard/mouse multi-selection
    and rendering of nodes with or without associated images.

  TestPropertyGridCustomCollectionEditor/
    Demonstrates custom PropertyGrid collection editors, item validation,
    and CollectionEditorObserver-based observation of collection dialogs.

  TestSerialPortListener/
    Demonstrates serial-port configuration and received-data monitoring
    using SerialPortLib and SerialPortUILib components.

  TestSharedMemory/
    Demonstrates exchanging an object between two processes through a
    named Windows shared-memory segment; contains THREE C# projects:
       TestSharedMemXferObj/ - shared PersonData transfer-type assembly;
       TestSharedMemServer/  - creates the segment and writes PersonData;
       TestSharedMemClient/  - attaches, reads PersonData, and prints data.
    This is a coordinated, multi-process sample, not a standalone form.

  TestSingletonGeneric/
    Demonstrates the generic Singleton<T> pattern through a Highlander
    example class and a WinForms interface.

  TestSplash/
    Demonstrates a customized Windows Forms splash-screen implementation
    whose source originated from an earlier CodeProject example.

  TestSubstEdit/
    Demonstrates editing text containing indivisible embedded fields, with
    TWO implementation branches and their own subprojects:
       TestSubstEdit_cpp/SubstLib/    - native C++ static library;
       TestSubstEdit_cpp/TestSubst/   - native C++ example application;
       TestSubstEdit_CS/SubstEditLib/ - reusable C# editing library;
       TestSubstEdit_CS/TestTgSchema/- C# example application.
    It covers field identifiers, serialization, clipboard handling, and
    undo/redo; this folder contains both .NET and native C++ projects.

  TestSystemHooking/
    Demonstrates Win32 mouse, keyboard, low-level keyboard, and window
    hook installation and logging from a managed application.

  TestTaskDialog/
    Exercises the modern TaskDialogWrapper API and contains a detailed
    experiment for visually aligning three columns using measured spaces,
    including truncation policies and diagnostics; see README.html.

  TestTooltip/
    Demonstrates custom tooltip handlers for list boxes, combo boxes, and
    window titles, including comparisons with/without visual styles.

  TestVistaTaskDialog/
    Demonstrates a Vista-style TaskDialog wrapper/emulator, including
    command links, radio buttons, and message-box variations.

  TestWeakRef/
    Demonstrates weak-event handler patterns and how different event
    subscriptions affect subscriber lifetime and garbage collection.

  TestXMLSerializer/
    Demonstrates serialization/deserialization of sample objects to
    payment.xml and employee.xml using XMLSerializerAdapter.

  Testmemcmp/
    Compares MemUtils.memcmp and Enumerable.SequenceEqual for byte-array
    comparison, with an emphasis on measured execution time.

End of README.
