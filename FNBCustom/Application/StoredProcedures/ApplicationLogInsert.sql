CREATE Procedure [etl].[ApplicationLogInsert]
	@State varchar(20),
	@IsError bit,
	@Severity varchar(20),
	@Environment varchar(20),
	@Command varchar(50) = NULL,
	@CommandName varchar(50) = NULL,
	@IsChainedCommand bit = NULL,
	@Mode varchar(50) = NULL,
	@ModeName varchar(50) = NULL,
	@Arguments varchar(50) = NULL,
	@ShortMsg varchar(250) = NULL,
	@LongMsg varchar(max) = NULL,
	@RecordCount int = NULL,
	@RoundTripSec float = NULL,
	@ServerName varchar(50),
	@SessionID uniqueidentifier
    --@AppLogID bigint OUTPUT
AS
BEGIN

SET NOCOUNT ON;

	INSERT INTO [etl].[ApplicationLog]
	([State]
	,IsError
	,Severity
	,Environment
	,CreateDate
	,Command
	,CommandName
	,IsChainedCommand
	,Mode
	,ModeName
	,Arguments
	,ShortMsg
	,LongMsg
	,RecordCount
	,RoundTripSec
	,ServerName
	,SessionID)
	VALUES 
	(@State
	,@IsError
	,@Severity
	,@Environment
	,GetDate()
	,@Command
	,@CommandName
	,@IsChainedCommand
	,@Mode
	,@ModeName
	,@Arguments
	,@ShortMsg
	,@LongMsg
	,@RecordCount
	,@RoundTripSec
	,@ServerName
	,@SessionID)
	
  --SET @AppLogID = SCOPE_IDENTITY();
END
GO