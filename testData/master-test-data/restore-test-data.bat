@echo off

:: Cleanup any existing zip and/or log files.

del ..\*.zip
del ..\*.log

:: Copy the zip files and logs from our master area.

copy *.zip ..\
copy *.log ..\
