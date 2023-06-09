
CREATE PROCEDURE [etl].[CheckAgainstEJDATADETAIL]
	@RegionID char(4),
	@OfficeID char(5),
	@TransSeq int,
	@MsgSeq int,
	@CashsumId char(4),
	@ProcDate DATETIME
AS
BEGIN

SET NOCOUNT ON;

SELECT [PROCDATE]
      ,[REGIONID]
      ,[OFFICEID]
      ,[CASHSUMID]
      ,[TRANSEQNUM]
      ,[MSGSEQNUM]
      ,[PROCLISTNAME]
	  ,[REVERSAL]
      ,[REVERSED]
      ,[APPLSUBFUNC]
      ,[SOURCEREGIONID]
      ,[SOURCEOFFICEID]
      ,[SOURCEOPERID]
      ,[AMOUNT1]
      ,[CASHIN]
      ,[CASHINOFFSET]
      ,[CASHOUT]
      ,[CASHOUTOFFSET]
FROM [ARGOENT].[dbo].[TLR_EJDATADETAIL] WITH (NOLOCK)
WHERE REGIONID = @RegionID
  AND OFFICEID = @OfficeID
  AND TRANSEQNUM = @TransSeq
  AND MSGSEQNUM = @MsgSeq
  AND CASHSUMID = @CashsumId
  AND procdate = @ProcDate
  AND REVERSAL = 0
  AND REVERSED = 0
END
GO