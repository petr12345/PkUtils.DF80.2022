param (
    [string]$FolderPath
)

# Check if the folder path is provided
if (-not $FolderPath) {
    Write-Host "Please provide a folder path as an argument."
    exit
}

# Check if the folder exists
if (-not (Test-Path $FolderPath -PathType Container)) {
    Write-Host "Folder not found: $FolderPath"
    exit
}

# Recursively unblock files in the specified folder
function Recursively-Unblock-Files ($folderPath) {
    $files = Get-ChildItem $folderPath -Recurse | Where-Object { $_.PSIsContainer -eq $false }

    foreach ($file in $files) {
        Unblock-File -Path $file.FullName
        Write-Host "Unblocked: $($file.FullName)"
    }
}

# Call the function to unblock files recursively
Recursively-Unblock-Files $FolderPath
