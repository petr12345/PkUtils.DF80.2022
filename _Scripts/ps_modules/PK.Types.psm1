#  FileName: PK.Types.psm1
#---------------------------------------------------------------------------------------
#  Script Name: PK.Types
#  Created: 03/08/2011
#
#  Purpose:
#  Functionality for defining new custon enum type
#
#  Example: 
#  Add-Enum -nameSpace "BuildEngine" -name "BuildTypes" -values "None = 0", "Major", "Minor", "Build"
#  [BuildEngine.BuildTypes]$buildEngineType = [BuildEngine.BuildTypes]::Build
#  switch([BuildEngine.BuildTypes]$buildEngineType)
#  {
#	 $([BuildEngine.BuildTypes]::Major)	{ Write-Host "Selected: Major"}
#	 $([BuildEngine.BuildTypes]::Minor)	{ Write-Host "Selected: Major"}
#	 $([BuildEngine.BuildTypes]::Build)	{ Write-Host "Selected: Build"}
#	 default {Write-Host "Build could not be determined"}
#  }
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
#  Purpose: Adds a new enum type
#  Arguments: nameSpace - list of enum values
#             typeName - the name of the enum type
#             values - comma-separated list of individual values
#---------------------------------------------------------------------------------------
function Add-Enum([String]$nameSpace, [String]$typeName, [String[]]$values) 
{
    if ($nameSpace) 
    {
$code = @"
    namespace $nameSpace
    {
        public enum $typeName : int 
        {
            $($values -join ",`n")
        }
    }
"@
    }
    else 
    {
$code = @"
  public enum $typeName : int 
  {
      $($values -join ",`n")
  }
"@
    }
    
  Add-Type $code
}

#---------------------------------------------------------------------------------------
#  Purpose: Gets enum members
#  Arguments: The actual enum type
#  Example:
#    Add-Enum -name "BuildTypes" -nameSpace "BuildEngine" -values "None = 0", "Major", "Minor", "Build"
#    Get-EnumMembers([BuildEngine.BuildTypes])
#---------------------------------------------------------------------------------------
function Get-EnumMembers([Type]$enum)
{
    [Type]$enumType = [Enum]::GetUnderlyingType($enum)
    [Enum]::getNames($enum) | Select-Object -Property @{Name = "Name" ; Expression = {$_} }, @{ Name = "Value"; Expression = { $_}}
}

#---------------------------------------------------------------------------------------
#  EXPORT SECTION
#---------------------------------------------------------------------------------------
Export-ModuleMember Add-Enum, Get-EnumMembers