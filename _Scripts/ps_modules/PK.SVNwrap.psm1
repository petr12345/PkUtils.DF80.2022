#  FileName: PK.SVNwrap.psm1
#---------------------------------------------------------------------------------------
#  Script Name: PK.SVNwrap
#  Created: 03/09/2011
#
#  Purpose:
#  PowerShell module contaning various functions used for SVN access.
#  Script addes a new file into the subversion storage
#  Note: It is assumed that svn.exe ( CollabNetSubversion-client ) is installed.
#  For more info regarding svn.exe, see for instance
#  http://stackoverflow.com/questions/1071857/how-do-i-svn-add-all-unversioned-files-to-svn
#  For more info about PowerShell modules, see for instance
#  http://tfl09.blogspot.com/2009/01/modules-in-powershell-v2.html
#  
#---------------------------------------------------------------------------------------

#---------------------------------------------------------------------------------------
#  REVISION HISTORY
#---------------------------------------------------------------------------------------
#  Date:
#  Author of Revision:
#  Change:
#---------------------------------------------------------------------------------------

#---------------------------------------------------------------------------------------
#  GLOBAL VARIABLES
#---------------------------------------------------------------------------------------

#---------------------------------------------------------------------------------------
#  FUNCTION LISTINGS
#---------------------------------------------------------------------------------------

#---------------------------------------------------------------------------------------
#  Purpose: Verifies correctness of the input arguments
#           Returns true if verification succeeded, otherwise returns false
#  Arguments: strPathName - file path&name
#---------------------------------------------------------------------------------------
function CheckAddedFile([string]$strPathName)
{
    $bCorrectInput = $true
    
    # verify file existence
    #
    if ((Test-Path $strPathName -pathtype leaf) -eq $false)
    { 
        Write-Host "Error - the specified file $strPathName does not exist" -ForegroundColor Red
        Start-Sleep -s 6        
        $bCorrectInput = $false
    }
    
    return $bCorrectInput
}

#---------------------------------------------------------------------------------------
#  Purpose: Adds the given file to SVN storage
#           Returns true if succeeded, otherwise returns falss
#  Arguments: strPathName - file path&nam
#---------------------------------------------------------------------------------------
function AddToSVN([string]$strPathName)
{
    [int]$nErr = 0
    [bool]$bOk = $true
    
    if ((CheckAddedFile $strPathName) -eq $false)
    {
        $bOk = $false
    }
    else
    {
        $strExeName = "svn.exe"
        $strArg1 = "add"
        $strArg2 = $strPathName
        $strArg3 = "-q"
        & $strExeName $strArg1 $strArg2 $strArg3
        $nErr = $LastExitCode
		if ($nErr -ne 0)
		{ 
			Write-Host "Error in AddToSVN - the last exit code is $nErr" -ForegroundColor Red
			Start-Sleep -s 6        
			$bOk = $false
		}
    }
        
    return $bOk
}

#---------------------------------------------------------------------------------------
#  EXPORT SECTION
#---------------------------------------------------------------------------------------
Export-ModuleMember AddToSVN, CheckAddedFile