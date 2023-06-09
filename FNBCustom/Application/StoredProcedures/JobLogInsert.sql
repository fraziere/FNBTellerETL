CREATE Procedure [etl].[JobLogInsert]
	@IsError bit,
	@Severity varchar(20),
	@Environment varchar(20),
	@Command varchar(50) = NULL,
	@CommandName varchar(50) = NULL,
	@Mode varchar(50) = NULL,
	@ModeName varchar(50) = NULL,
	@Arguments varchar(50) = NULL,
	@ShortMsg varchar(250) = NULL,
	@LongMsg varchar(max) = NULL,
	@RecordCount int = NULL,
	@RoundTripMs float = NULL,
	@Output varchar(max) = NULL,
	@ServerName varchar(50),
	@SessionID uniqueidentifier
AS
BEGIN

SET NOCOUNT ON;

	INSERT INTO [etl].[JobLog]
	(IsError
	,Severity
	,Environment
	,CreateDate
	,Command
	,CommandName
	,Mode
	,ModeName
	,Arguments
	,ShortMsg
	,LongMsg
	,RecordCount
	,RoundTripMs
	,[Output]
	,ServerName
	,SessionID)
	VALUES 
	(@IsError
	,@Severity
	,@Environment
	,GetDate()
	,@Command
	,@CommandName
	,@Mode
	,@ModeName
	,@Arguments
	,@ShortMsg
	,@LongMsg
	,@RecordCount
	,@RoundTripMs
	,@Output
	,@ServerName
	,@SessionID)
	
END
GO


