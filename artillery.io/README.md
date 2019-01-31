## Setting up

Set registry value `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\MaxUserPort` to something like `60000`.

Add `--max-old-space-size=4096` parameter to `C:\Users\{Username}\AppData\Roaming\npm\artillery.cmd`, so it looks like `node --max-old-space-size=4096 "%~dp0\node_modules\artillery\bin\artillery" %*`. The value is specified in megabytes.

Expose garbage collection: add `--expose_gc` parameter to `C:\Users\{Username}\AppData\Roaming\npm\artillery.cmd`, so it looks like `node --expose_gc --max-old-space-size=8192 "%~dp0\node_modules\artillery\bin\artillery" %*`.

## Troubleshooting

It seems like NodeJs only uses `49152 – 65535` ports range (16 thousands) for TCP connections.
Execute `taskkill /im node.exe /f` to free up those ports.