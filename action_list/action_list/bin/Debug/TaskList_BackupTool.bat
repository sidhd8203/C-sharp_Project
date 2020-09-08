@echo off
set CURPATH=%cd%\%date:~0,4%
set hour=%time:~0,2%
if "%time:~0,1%"==" " set hour=0%time:~1,1%
set folder_save=%CURPATH%\%date:~5,2%\%date:~0,4%%date:~5,2%%date:~8,2%
set logfolder_save=%CURPATH%\%date:~5,2%\%date:~0,4%%date:~5,2%%date:~8,2%log
md %folder_save%
md %logfolder_save%
set filename=%CURPATH%\%date:~5,2%\%date:~0,4%%date:~5,2%%date:~8,2%\%hour%%time:~3,2%
tasklist > %filename%.txt
