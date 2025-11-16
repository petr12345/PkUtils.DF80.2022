param(
    [string]$FolderPath
)

# If no argument is provided, use the script's current directory
if (-not $FolderPath) {
    $FolderPath = Split-Path -Parent $MyInvocation.MyCommand.Path
}

# Resolve to full path
$FolderPath = Resolve-Path $FolderPath

Write-Host "Searching in folder: $FolderPath" -ForegroundColor Cyan

# Define rename rules
$patterns = @{
    "*.DF80.2022.csproj" = "*.DF10.2026.csproj"
    "*.DF80.2022.sln"    = "*.DF10.2026.sln"
}

# Recursively find and rename files
foreach ($pattern in $patterns.Keys) {
    $replacement = $patterns[$pattern]
    Get-ChildItem -Path $FolderPath -Recurse -Filter $pattern | ForEach-Object {
        $oldPath = $_.FullName
        $newPath = $oldPath -replace [regex]::Escape("DF80.2022"), "DF10.2026"
        Write-Host "Renaming:`n  From: $oldPath`n  To:   $newPath" -ForegroundColor Yellow
        Rename-Item -Path $oldPath -NewName (Split-Path $newPath -Leaf)
    }
}

Write-Host "Renaming completed." -ForegroundColor Green
