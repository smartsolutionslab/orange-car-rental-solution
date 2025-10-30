# Stop all Orange Car Rental related processes
Get-Process | Where-Object {$_.ProcessName -like '*Orange*'} | ForEach-Object {
    Write-Host "Stopping: $($_.ProcessName) (PID: $($_.Id))"
    Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
}

# Stop dotnet processes running APIs
Get-Process -Name "dotnet" | Where-Object {
    $_.Modules | Where-Object {$_.FileName -like '*OrangeCarRental*'}
} | ForEach-Object {
    Write-Host "Stopping dotnet process: PID $($_.Id)"
    Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
}

Write-Host "All processes stopped."
