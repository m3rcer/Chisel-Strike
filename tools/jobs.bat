@echo off
for /F "tokens=2" %%K in ('
   tasklist /FI "ImageName eq chisel64.exe" /FO LIST ^| findstr /B "PID:"
') do (
   echo %%K 
)
