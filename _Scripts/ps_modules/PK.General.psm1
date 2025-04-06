#  FileName: PK.General.psm1
#---------------------------------------------------------------------------------------
#  Script Name: PK.General
#  Created: 03/08/2011
#
#  Purpose: 
#    PowerShell module contaning various functions used in other powershell scripts.
#    For more info about PowerShell modules, see for instance
#    http://tfl09.blogspot.com/2009/01/modules-in-powershell-v2.html
#---------------------------------------------------------------------------------------

#---------------------------------------------------------------------------------------
#  REVISION HISTORY
#---------------------------------------------------------------------------------------
#  Date:
#  Author of Revision:
#  Change:
#---------------------------------------------------------------------------------------

#---------------------------------------------------------------------------------------
#  FUNCTION LISTINGS
#---------------------------------------------------------------------------------------
#  Purpose: Tests the directory existence; the directory is specified by input argument
#           Returns 1 if such directory exists
#           Returns 0 if such directory does not exists
#           Returns -1 if 'strPath' actually refers to the file in the file system
#  Arguments: strPath - input argument specifying the directory ( should be rooted path )
#---------------------------------------------------------------------------------------
function CheckDirectoryExistence([string]$strPath)
{
    # Note: besides of Test-Path -path $strPath, must use DirectoryInfo here.
    # The reason is that "Test-Path -path $strPath" returns true even if strPath value is actually 
    # a file name of the existing file, not a directory name.
    [int]$nResult = 0
    
    if ((Test-Path -path $strPath))
    {
        [System.IO.DirectoryInfo] $dirInfo = ([System.IO.DirectoryInfo]$strPath)
        if ($dirInfo.Exists)
        {
            if (($dirInfo.Attributes -band [System.IO.FileAttributes]::Directory))     
            {
                $nResult = 1
            }
            else
            {
                $nResult = -1			
            }
        }
        elseif (([System.IO.FileInfo]$strPath).Exists)
        {
            $nResult = -1			
        }
    }
        
    return $nResult
}

#---------------------------------------------------------------------------------------
#  FUNCTION LISTINGS
#---------------------------------------------------------------------------------------
#  Purpose: Combine two strings - the rooted path and subdirectory - into one rooted path
#  Arguments: root - the rooted directory
#             subdir - subdirectory
#---------------------------------------------------------------------------------------
function PathCombine([string]$root, [string]$subdir) 
{
    $left = PathRemoveSeparator($root)
    $right = $subdir.TrimStart([System.IO.Path]::DirectorySeparatorChar)
    $right = $right.TrimStart([System.IO.Path]::AltDirectorySeparatorChar)    
    $fullPath = [System.IO.Path]::Combine($left, $right)
    return $fullPath
}

#---------------------------------------------------------------------------------------
#  FUNCTION LISTINGS
#---------------------------------------------------------------------------------------
#  Removes ending backslash from the given path ( input argument ), 
#  if the path ends with backslash and if the path is not disk root specification.
#  Resulting string is returned as a function result.
#  Examples: string "C:\Dokumenty\Abc\" will be changed to "C:\Dokumenty\Abc"
#            string "C:\Dokumenty" will not be changed 
#            string "C:\" will not be changed 
#            string "\" will not be changed 
#  Arguments: The input path
#---------------------------------------------------------------------------------------
function PathRemoveSeparator([string]$strPath)
{
    [string]$result = $strPath;
    
     if ($strPath -ne $null)
     {
        [int]$nLength = $strPath.Length 
        if ($nLength -gt 0)     
        {
           [char]$chLast = $strPath[$nLength - 1]
           if (($chLast -ne [System.IO.Path]::DirectorySeparatorChar) -and
               ($chLast -ne [System.IO.Path]::AltDirectorySeparatorChar) -and
               ($chLast -ne [System.IO.Path]::VolumeSeparatorChar))
               {
                    $result = $strPath + [System.IO.Path]::DirectorySeparatorChar;
               }
        }
     }
     
    return $result
}

#---------------------------------------------------------------------------------------
#  Purpose: Returns true for 64bit process, otherwise false
#---------------------------------------------------------------------------------------
function IsX64Process
{
    [bool]$isX64proc = $true
    
    if ([System.IntPtr]::Size -eq 4)
    {
        $isX64proc = $false
    }
    else
    {
        $isX64proc = $true
    }
    
    return $isX64proc
}

#---------------------------------------------------------------------------------------
#  Purpose: Returns true for 64bit Windows, false otherwise 
#           Remark: the returned value does NOT depend on the fact whether current 
#           runnning process is 32-bit or 64-bit 
#---------------------------------------------------------------------------------------
function IsX64System
{
    [bool]$isX64 = $true
    
    if(${env:ProgramFiles(x86)} -eq $null)
    {
        $isX64 = $false
    }
    return $isX64
}

#---------------------------------------------------------------------------------------
#  Purpose: Writes up (dumps) all script arguments to the console
#  Arguments: arrArgs - script arguments
#
#  Example:
#	PrintScriptArguments $script:args
#---------------------------------------------------------------------------------------
function PrintScriptArguments([Array]$arrArgs)
{
    [int]$nArgs = $arrArgs.Length
    [string]$strNotice = [System.String]::Format("Printing {0} script arguments...", $nArgs)
    
    Write-Host $strNotice    
    if ($nArgs -gt 0)
    {
        for($i = 0; $i -lt $nArgs; $i++)
        {
            Write-Host "args[$i] = $($arrArgs[$i])"
        }
    }
    else
    {
        Write-Host "No script arguments have been provided"
    }
}

#---------------------------------------------------------------------------------------
#  Purpose: Filters arguments and returns Hashtable of parameter arguments, where
#			key is name of the variable and value is the value of the variable.
#  Arguments: arrArgs - array of variables
#
#  Example:
#	[System.Collections.Hashtable]$paramVariables = GetNamedArgs (Get-Variable)
#---------------------------------------------------------------------------------------
function GetNamedArgs([Array]$arrArgs)
{
    [System.Collections.Hashtable]$paramVariables = new-object System.Collections.Hashtable
    $arrArgs | Where-Object {$_.Attributes | ForEach-Object {$_ -is [System.Management.Automation.ParameterAttribute]} } | ForEach-Object {$paramVariables.Add($_.Name,$_.Value)}
    
    return $paramVariables
}

#---------------------------------------------------------------------------------------
#  Purpose: Writes up (dumps) all named parameter arguments to the console
#  Arguments: arrArgs - array of variables
#
#  Example:
#	PrintNamedArgs (Get-Variable)
#---------------------------------------------------------------------------------------
function PrintNamedArgs([Array]$arrArgs)
{
    [System.Collections.Hashtable]$paramVariables = GetNamedArgs $arrArgs
    
    if($paramVariables -ne $null)
    {
        $nArgs = $paramVariables.Count
        [string]$strNotice = [System.String]::Format("Printing {0} script named arguments...", $nArgs)
        Write-Host $strNotice
        
        if ($nArgs -gt 0)
        {
            foreach ($key in $paramVariables.Keys)
            {
                Write-Host "$key = $($paramVariables[$key])"
            }
        }
        else
        {
            Write-Host "No script named arguments have been provided"
        }
    }
}

#---------------------------------------------------------------------------------------
#  Purpose: Show message box
#           Returns true if succeeded, false on error
#  Arguments: title - messagebox title
#  Arguments: msg   - messagebox message
#---------------------------------------------------------------------------------------
function ShowMessageBox ([string]$title, [string]$msg) 
{
    [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") | Out-Null  
    [Windows.Forms.MessageBox]::Show($msg, $title, [Windows.Forms.MessageBoxButtons]::OK, [System.Windows.Forms.MessageBoxIcon]::Warning, [System.Windows.Forms.MessageBoxDefaultButton]::Button1, [System.Windows.Forms.MessageBoxOptions]::DefaultDesktopOnly) | Out-Null 
}

#---------------------------------------------------------------------------------------
#  EXPORT SECTION
#---------------------------------------------------------------------------------------
Export-ModuleMember CheckDirectoryExistence, PathCombine, PathRemoveSeparator, IsX64Process, IsX64System, PrintScriptArguments, ShowMessageBox, GetNamedArgs, PrintNamedArgs