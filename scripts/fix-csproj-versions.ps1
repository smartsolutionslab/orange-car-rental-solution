# Script to remove Version attributes from PackageReference elements for Central Package Management

$projectFiles = Get-ChildItem -Path "src/backend" -Filter "*.csproj" -Recurse

foreach ($file in $projectFiles) {
    Write-Host "Processing: $($file.FullName)"

    [xml]$xml = Get-Content $file.FullName

    $modified = $false

    # Find all PackageReference elements with Version attribute
    $packageRefs = $xml.SelectNodes("//PackageReference[@Version]")

    foreach ($ref in $packageRefs) {
        $packageName = $ref.GetAttribute("Include")
        Write-Host "  Removing version from: $packageName"
        $ref.RemoveAttribute("Version")
        $modified = $true
    }

    if ($modified) {
        $xml.Save($file.FullName)
        Write-Host "  Saved changes to: $($file.FullName)" -ForegroundColor Green
    }
}

Write-Host "`nDone! All project files have been fixed." -ForegroundColor Cyan
