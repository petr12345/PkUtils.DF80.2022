#  FileName: _rename.ps1
#---------------------------------------------------------------------------------------


#---------------------------------------------------------------------------------------
#  ARGUMENTS & MODE DECLARATION
#---------------------------------------------------------------------------------------
Param()
Set-StrictMode -Version 3.0

#---------------------------------------------------------------------------------------
#  FUNCTION LISTINGS
#---------------------------------------------------------------------------------------
#  Purpose: Checks the value of errorlevel. If it is nonzero it sets global exitcode 
#           to that value and returns false. Otherwise it returns true.
#  Arguments: None
#---------------------------------------------------------------------------------------
function CheckErrorLevel
{
  $bErrorLevelOK = $true
  if (($env:ERRORLEVEL -ne $null) -and ($env:ERRORLEVEL -ne 0))
  {
    $script:exitCode = $env:ERRORLEVEL
    $bErrorLevelOK = $false
  }
    
  return $bErrorLevelOK
}

#---------------------------------------------------------------------------------------
#  Purpose: Checks for existence of used powershell modules ( now just PK.General.psm1),
#	 and loads these modules. Returns true for success, otherwise  false
#  Arguments: None
#---------------------------------------------------------------------------------------
function CheckAndLoadModules
{
  $bOK = $true

  [string]$strModuleGeneral = (Join-Path  $script:strScriptDir "..\ps_modules\PK.General.psm1")
  [string[]]$arrModules = ($strModuleGeneral)
    
  foreach($strModule in $arrModules)
  {
    if (!(Test-Path -Path $strModule -PathType Leaf))
    {
      EchoError "Script '$strModule' does not exist"
      $bOK = $false
    }
  }
  if ($bOK)
  {
    foreach($strModule in $arrModules) { Import-Module $strModule } 
  }
    
  return $bOK
}

#---------------------------------------------------------------------------------------
#  Purpose: Display the given error message text
#  Arguments: [string]$strErrMsg - the error message
#---------------------------------------------------------------------------------------
function EchoError([string]$strErrMsg)
{
  [string]$strFinalText = "Error: " + $strErrMsg
  Write-Host $strFinalText -ForegroundColor Red
}

#---------------------------------------------------------------------------------------
#  Purpose: Assuming the argument $item is a file in the file system,
#    replaces in the filename the string $strOriginalFileNamePart by $strNewFileNamePart
#     
#  Arguments:   [System.IO.FileSystemInfo]$item the file that will be copiedd
#   [string]$strOriginalFileNamePart The string being replaced in new file name
#   [string]$strNewFileNamePart The string replacing in the original string in file name
#---------------------------------------------------------------------------------------
function CopyFileTo(
  [System.IO.FileSystemInfo]$item, 
  [string]$strOriginalFileNamePart, 
  [string]$strNewFileNamePart)
{
  [string]$strFullItemName = $item.FullName
  [string]$strNewFullItemName = $strFullItemName.Replace($strOriginalFileNamePart, $strNewFileNamePart)    
  
  if (Test-Path $item.FullName -PathType Container)
  {
    [string]$strErr = "Wrong attempt to copy a directory : " + $strFullItemName
    EchoError $strErr
  }
  else
  {
    if (-Not (Test-Path -Path $strNewFullItemName -PathType Leaf))
    {
      Write-Host $strNewFullItemName -ForegroundColor Blue    
      Copy-Item $strFullItemName $strNewFullItemName
    }
  }
}

#---------------------------------------------------------------------------------------
#  Purpose: renames projects and solutions from  "DF46.2015" to "DF48.2019", 
#     Uses recursive searching the include the given directory $path and its subdirectories.
#  Arguments: [string]$path  The root folder where all file searching begins
#---------------------------------------------------------------------------------------
function Rename2015To2019([string]$path)
{
  foreach ($item in Get-ChildItem $path)
  {
    if (Test-Path $item.FullName -PathType Container)
    {
      Rename2015To2019 $item.FullName
    }
    else
    {
      [string]$strItemName = $item.FullName
      [string]$baseName = [System.IO.Path]::GetFileName($item.FullName)
      [string]$newBaseName2017 = $baseName.Replace(".DF46.2015", ".DF48.2019")
      [string]$newBaseNameConfig2017 = $baseName.Replace("app.df46.config", "app.df48.config")	  
        
      if ([string]::Compare($baseName, $newBaseName2017, $True) -ne 0)
      {
        Rename-Item $strItemName $newBaseName2017
      }
	  elseif ([string]::Compare($baseName, $newBaseNameConfig2017, $True) -ne 0)
	  {
        Rename-Item $strItemName $newBaseNameConfig2017
	  }
    }
  }
} 


#---------------------------------------------------------------------------------------
#  Purpose: Should copy VS 2015 project files and solutions to VS 2019 files.
#  Arguments: [string]$path  The root folder where all file searching begins
#---------------------------------------------------------------------------------------
function Copy2015ToVS2019([string]$path)
{
  foreach ($item in Get-ChildItem $path)
  {
    if (Test-Path $item.FullName -PathType Container)
    {
      Copy2015ToVS2019 $item.FullName
    }
    else
    {
      [string]$strFullItemName = $item.FullName 
      [string]$str2015Sol  = ".DF46.2015.sln"
      [string]$str2015Proj = ".DF46.2013.csproj"
      [string]$str2019Sol  = ".DF48.2019.sln"
      [string]$str2019Proj = ".DF48.2019.csproj"

      if ($strFullItemName.EndsWith($str2013Sol))
      {
        CopyFileTo $item $str2013Sol $str2017Sol
      }
      elseif ($strFullItemName.EndsWith($str2013Proj))
      {
        CopyFileTo $item $str2013Proj $str2017Proj
      }
    }
  }
} 

#---------------------------------------------------------------------------------------
#  Purpose: Replaces in the contents of  project files
#     Uses recursive searching, that inlude the given directory $path and its subdirectories.
#  Arguments: [string]$path  The root folder where all file searching begins
#
#	  See How can I replace every occurrence of a String in a file with PowerShell?
#	  https://stackoverflow.com/questions/17144355/how-can-i-replace-every-occurrence-of-a-string-in-a-file-with-powershell
#---------------------------------------------------------------------------------------
function ReplaceInFiles([string]$path)
{
  foreach ($item in Get-ChildItem $path)
  {
    if (Test-Path $item.FullName -PathType Container)
    {
      ReplaceInFiles $item.FullName
    }
    else
    {
	  [string]$content = ""
      [string]$strItemName = $item.FullName
      [string]$baseName = [System.IO.Path]::GetFileName($item.FullName)
      [bool]$b2010 = $false
      [bool]$b2012 = $baseName.Contains("2012")
      [bool]$bcsproj = $baseName.Contains("2019.csproj")
	  
      if ($bcsproj)
      {
#       Write-Host $baseName  -ForegroundColor Blue
#       (Get-Content $strItemName).replace('bin_df46','bin_df48') | Set-Content $strItemName
#       (Get-Content $strItemName).replace('obj_df46','obj_df48') | Set-Content $strItemName
		
		$content = [System.IO.File]::ReadAllText($strItemName).Replace("bin_df46","bin_df48")
		$content = $content.Replace("obj_df46","obj_df48")
		$content = $content.Replace("DF46.2015","DF48.2019")
		$content = $content.Replace("df46.config","df48.config")						
		[System.IO.File]::WriteAllText($strItemName, $content)		
	  }
      if ($b2010)
      {
#       Write-Host $baseName  -ForegroundColor Blue
#       (gc $strItemName).replace('.VS0.','.2010.') | Set-Content $strItemName
      }
      if ($b2012)
      {
#       Write-Host $baseName  -ForegroundColor Blue
#       (gc $strItemName).replace('.VS1.','.2012.') | Set-Content $strItemName
      }
    }
  }
}

#---------------------------------------------------------------------------------------
#  SCRIPT BODY
#---------------------------------------------------------------------------------------
Write-Host running $MyInvocation.MyCommand.Path -ForegroundColor DarkGreen
[string]$strScriptDir = Split-Path $MyInvocation.MyCommand.Path	
[string]$strAboveAboveScriptDir = $strScriptDir + "\..\..";
[string]$strScriptFileName = Split-Path -Leaf $MyInvocation.MyCommand.Path
[int]$exitCode = 0

[System.IO.DirectoryInfo] $dirInfoAboveAbove = ([System.IO.DirectoryInfo]$strAboveAboveScriptDir )
$strAboveAboveScriptDir = $dirInfoAboveAbove.FullName

if (!(CheckErrorLevel))
{
  EchoError "Premature end of script '$strScriptFileName' because errorlevel is nonzero"
  $exitCode = $env:ERRORLEVEL
}
elseif (!(CheckAndLoadModules))
{
  EchoError "The script '$strScriptFileName' has failed loading its modules"
  $exitCode = 1
}
else
{
#  Rename2015To2019 $strAboveAboveScriptDir
#  Copy2015ToVS2019 $strAboveAboveScriptDir
  ReplaceInFiles $strAboveAboveScriptDir
}

if ($exitCode -ne 0)
{
  EchoError "The script '$strScriptFileName' has failed `r`n"
  Start-Sleep -s 30
  $env:ERRORLEVEL = $exitCode
}
else
{
  Write-Host "The script '$strScriptFileName' succeeded `r`n" -ForegroundColor DarkGreen
  Start-Sleep -s 5	
}

exit $exitCode
