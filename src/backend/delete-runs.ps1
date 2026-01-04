$runs = Get-Content runs.json | ConvertFrom-Json
foreach ($run in $runs) {
    Write-Host "Deleting $($run.databaseId)"
    gh run delete $run.databaseId --confirm 2>&1 | Out-Null
}
Write-Host "Done! Deleted $($runs.Count) runs"