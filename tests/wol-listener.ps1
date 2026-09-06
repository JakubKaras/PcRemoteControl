# WOL Listener Script
# Listens for WOL (Wake-on-LAN) magic packets on UDP port 7

$RemoteEndpoint = New-Object System.Net.IPEndPoint([System.Net.IPAddress]::Any, 0)
$UdpClient = New-Object System.Net.Sockets.UdpClient
$UdpClient.Client.SetSocketOption([System.Net.Sockets.SocketOptionLevel]::Socket, [System.Net.Sockets.SocketOptionName]::ReuseAddress, $true)
$UdpClient.ExclusiveAddressUse = $false
$UdpClient.Client.ReceiveTimeout = 1000  # 1 second timeout to allow interrupt

try {
    # Bind to port 7 (standard WOL port) on all interfaces
    $UdpClient.Client.Bind([System.Net.IPEndPoint]::new([System.Net.IPAddress]::Any, 7))
    
    Write-Host "Listening for WOL requests on port 7..." -ForegroundColor Green
    Write-Host "Press Ctrl+C to stop`n" -ForegroundColor Yellow
    
    while ($true) {
        
        try {
            $ReceiveBytes = $UdpClient.Receive([ref]$RemoteEndpoint)
            $ReceiveText = [System.Text.Encoding]::ASCII.GetString($ReceiveBytes)
            
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] WOL Request received from $($RemoteEndpoint.Address)" -ForegroundColor Cyan
            Write-Host "Packet size: $($ReceiveBytes.Length) bytes"
            Write-Host "Hex dump: $([System.BitConverter]::ToString($ReceiveBytes))`n"
        }
		catch [System.Net.Sockets.SocketException] {
			# Timeout occurred, silently continue
		}
        catch {
            Write-Error "Error receiving data: $_"
        }
    }
}
finally {
    $UdpClient.Close()
    Write-Host "Listener stopped." -ForegroundColor Yellow
}