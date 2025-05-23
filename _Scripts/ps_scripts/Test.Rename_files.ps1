#  FileName: Test.AddNewFiles.ps1
#---------------------------------------------------------------------------------------
#  Script Name: Test.AddNewFiles
#  Created: 03/08/2011
#  
#  Purpose:
#  For debugging purpose only!
#  The script is calling AddNewFiles.ps1 with several arguments
# 
#---------------------------------------------------------------------------------------

#---------------------------------------------------------------------------------------
#  ARGUMENTS & MODE DECLARATION
#---------------------------------------------------------------------------------------
Set-StrictMode -Version 3.0


#---------------------------------------------------------------------------------------
#  SCRIPT BODY
#---------------------------------------------------------------------------------------
Write-Host running $MyInvocation.MyCommand.Path
[int]$exitCode = 0

# --- when using hard-coded paths
# [string]$myPathOfCmd = "e:\PK.Projects\PkNetUtils\_Scripts\ps_scripts"
# [string]$myPathOfRoot = "e:\PK.Projects\PkNetUtils\_Scripts"
[string]$myPathOfCmd = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path)
[string]$myPathOfRoot = (Get-Item $myPathOfCmd).Parent.Parent.FullName

# --- now complete the whole command and run it
[string]$myCmd = $myPathOfCmd + [System.IO.Path]::DirectorySeparatorChar + "Rename_files.ps1"

& $myCmd $myPathOfRoot
if ($LastExitCode -ne $null)
{
   $exitCode = $LastExitCode
}

exit $exitCode
