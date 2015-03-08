@ECHO OFF
SETLOCAL enabledelayedexpansion
CLS

SET outputRoot=/App_Plugins/BackofficeTweaking
SET outputFile=%cd%\files.txt
IF EXIST "%outputFile%" DEL "%outputFile%"
CD ..
CD "App_Plugins"
SET outputRemoveRoot=%cd%


REM -- Get all files 
SET /A COUNT=1
FOR /R %%f IN (*) DO (
		CALL :processFile "%%f" "%%~nf" "%%~xf" >> "%outputFile%"
		SET /A COUNT+=1
		ECHO !COUNT!
	)
GOTO :EOF

:processFile
	SET filepath=%~1
	SET filename=%~2%~3
	SET processFileResult=

	REM -- Remove absolute path
	CALL :stringLength "%outputRemoveRoot%"
	CALL :subString "%filepath%" %stringLengthResult% 999
	SET processFileResult=%subStringResult%
	
	REM -- Remove filename
	CALL :stringLength "%filename%"
	SET filenameLength=%stringLengthResult%
	CALL :stringLength "%processFileResult%"
	SET /A relativePathLength = %stringLengthResult%-%filenameLength%
	CALL :subString "%processFileResult%" 0 %relativePathLength%
	SET processFileResult=%subStringResult%

	REM -- Replace backslashs with slashs
	CALL :stringReplace "%processFileResult%" "\" "/"
	SET processFileResult=%stringReplaceResult%

	REM -- Remove trailing slash
	IF %processFileResult:~-1%==/ SET processFileResult=%processFileResult:~0,-1%
	REM SET processFileResult=%processFileResult:~0,-1%	
	
	REM -- Add root
	SET processFileResult=%outputRoot%%processFileResult%
	
	REM -- Output filenames
	ECHO ^<file^>
    ECHO  ^<guid^>%filename%^</guid^>
    ECHO  ^<orgPath^>%processFileResult%^</orgPath^>
    ECHO  ^<orgName^>%filename%^</orgName^>
    ECHO ^</file^>
GOTO :EOF

:stringReplace
	SET str=%~1
	SET strSearch=%~2
	SET strReplace=%~3
	SET stringReplaceResult=
	CALL SET str=%%str:%strSearch%=%strReplace%%%
	SET stringReplaceResult=%str%
GOTO :EOF


:stringLength
	set #=%~1
	set stringLengthResult=0
	:loop
	if defined # (
		set #=%#:~1%
		set /A stringLengthResult += 1
		goto loop
	)
GOTO :EOF

:subString
	SET subStringResult=
	SET str=%~1
	SET startchar=%~2
	SET length=%~3
	CALL SET subStringResult=%%str:~%startchar%,%length%%%
GOTO :EOF

