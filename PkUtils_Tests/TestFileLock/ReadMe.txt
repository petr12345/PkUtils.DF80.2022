The project TestFileLock demonstrates file and folder locking from C#.
For native-code-related reference, see:

i/ LockFile function
http://msdn.microsoft.com/en-us/library/windows/desktop/aa365202(v=vs.85).aspx

ii/ MSDN - Directory Handles
http://msdn.microsoft.com/en-us/library/windows/desktop/aa365258(v=vs.85).aspx

iii/ code example of CreateFile() API for Directory, thus returning directory handle
http://www.cplusplus.com/forum/windows/46852/

#include<windows.h>
#include<stdio.h>
#include<WCHAR.h>
 
int main()
{
 WCHAR userPath[] = L"E:\\Demo"; /* Demo is read Only Folder*/
 HANDLE hFile = NULL;
 
 // To open a directory you must specify FILE_FLAG_BACKUP_SEMANTICS flag to CreateFile(). 
 // To clear read-only flag, use SetFileAttributes().
 hFile = ::CreateFile(
  userPath, 
  GENERIC_WRITE,
  FILE_SHARE_WRITE, 
  0,
  OPEN_EXISTING,
  FILE_FLAG_BACKUP_SEMANTICS,
  NULL);
 
 if(hFile == INVALID_HANDLE_VALUE)
 {
   wprintf(L"Error HANDLE = 0x%x \n",hFile);
 }
 else
 {
  wprintf(L"Suceess HANDLE = 0x%x \n",hFile);
  ::CloseHandle(hFile);
 }
}
 
iv/ StackOverflow - Using LockFileEx in C#
http://stackoverflow.com/questions/1784195/using-lockfileex-in-c-sharp