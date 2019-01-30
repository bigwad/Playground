Set registry value `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\MaxUserPort` to something like `60000`.

Add `--max-old-space-size=4096` parameter to `C:\Users\{Username}\AppData\Roaming\npm\artillery.cmd`, so it looks like `node --max-old-space-size=8192 "%~dp0\node_modules\artillery\bin\artillery" %*`.