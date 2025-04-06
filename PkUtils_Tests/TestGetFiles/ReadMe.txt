The project TestGetFiles is a console application, demonstrating the usage
of a FileSearch class; to be more exact of two derived classes 
FileSearchRecursive and FileSearchNonRecursive 

During the runtime, the program searches all the files with provided extension in the specified directory.

--- Supported command-line arguments list:

1. Given path to parse, specified by command-line option -d, like 
     TestGetFiles.exe -d c:\Tmp3
   If none specified, program will use Environment.GetFolderPath(Environment.SpecialFolder.System),
   which is usually evaluated to "C:\windows\system32"

2. The search pattern, specified by command-line option -p, like
     TestGetFiles.exe -p *.txt
   If none is specified, program will use "*.dll" string

3. The boolean switch that decides whether the caught SystemException(s) ( like UnauthorizedAccessException 
   or IOException ) will be dumped to the output or not. 
   Presence of switch implies means that caught exceptions will be dumped; absence means the opposite.
   Example of usage:

   TestGetFiles.exe DumpExceptions

---

Remark: Apparently, the FileSearcher that relies on Win32 interop does not work quite reliably,
as it sometimes produces LESS results that the other two.  For that reason, it is not included
in PkUtils assembly project, but just in this testing project as a link, 
