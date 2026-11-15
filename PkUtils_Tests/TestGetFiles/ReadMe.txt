TestGetFiles is a console application that compares two implementations of
FileSearchBase: FileSearchRecursive and FileSearchNonRecursive.

The program searches the specified directory tree for files matching a pattern,
compares the results from the two implementations, and then compares their
directory-search results.

Command-line options:

  -d <directory>     Root directory to search. If omitted, the program uses
                     Environment.SpecialFolder.System (usually C:\Windows\System32).

  -p <pattern>       File search pattern; defaults to *.dll.
                     Example: -p *.txt

  DumpExceptions     Optional switch that enables reporting of directory
                     access/parsing errors encountered by the search routines.

Example:
  TestGetFiles.exe -d C:\Tmp3 -p *.txt DumpExceptions

Only FileSearchRecursive and FileSearchNonRecursive are present in the
current comparison. For details, see Program.cs and SearchTester.cs.
