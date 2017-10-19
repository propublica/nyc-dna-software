@ECHO OFF
@ECHO OFF
REM This creates the debugOut directory and gives everyone permissions
IF EXIST DebugOut GOTO SKIPDebugOut
	MKDIR DebugOut
:SKIPDebugOut
cacls DebugOut /t /e /p Everyone:f

REM This creates the FSTReport directory (where reports get generated) and gives everyone permissions
IF EXIST FSTReport GOTO SKIPFSTReport
	MKDIR FSTReport
:SKIPFSTReport
cacls FSTReport /t /e /p Everyone:f

REM This creates Admin\Upload directory (where almost everything that needs to be saved temporarily gets saved) and gives everyone permissions
IF EXIST Admin\Upload GOTO SKIPAdminUpload
	MKDIR Admin\Upload
:SKIPAdminUpload
cacls Admin\Upload /t /e /g Everyone:f
