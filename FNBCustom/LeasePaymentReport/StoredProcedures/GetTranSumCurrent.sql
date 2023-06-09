CREATE PROCEDURE [etl].[LPRTranSumCurrent]
	@RegionID char(4),
	@OfficeID char(5),
	@TransSeq int,
	@CashsumId char(4),
	@ProcDate DATETIME
AS
BEGIN

SET NOCOUNT ON;

SELECT TOP (1) [TRANSEQNUM]
      ,[ITEMCOUNT]
      ,[CREDITAMOUNT]
      ,[CREDITCOUNT]
      ,[DEBITAMOUNT]
      ,[DEBITCOUNT]
  FROM [ARGOENT].[dbo].[TLR_EJTRAN] WITH (NOLOCK)
  WHERE [PROCDATE] = @ProcDate
  AND [REGIONID] = @RegionID
  AND [OFFICEID] = @OfficeID 
  AND [CASHSUMID] = @CashsumId  
  AND TRANSEQNUM > @TransSeq
  ORDER BY TRANSEQNUM ASC

END
GO
GO