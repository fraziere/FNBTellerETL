CREATE PROCEDURE [etl].[LPRGetSessionAllTrans]
	@RegionID char(4),
	@OfficeID char(5),
	@TransSeqHigh int,
	@TransSeqLow int,
	@CashsumId char(4),
	@ProcDate DATETIME
AS
BEGIN

SET NOCOUNT ON;

SELECT A.[procdate]
      ,A.[region_id]
      ,A.[office_id]
      ,A.[cashsum_id]
      ,A.[transeq]
      ,A.[GUID]
      ,A.[sequence]
      ,A.[ISN]
      ,A.[chgDateTime]
      ,A.[source]
      ,A.[oper_id]
      ,A.[CRDR]
      ,A.[itemnumber]
      ,A.[serial]
      ,A.[field6]
      ,A.[abanumber]
      ,A.[field4]
      ,A.[account]
      ,A.[trancode]
      ,A.[amount], B.REVERSAL, B.REVERSED 
FROM [ARGOENT].[dbo].[TLR_ITEM_MICR] AS A
LEFT JOIN (SELECT *
			FROM [ARGOENT].[dbo].[TLR_EJDATADETAIL] WITH (NOLOCK)
			WHERE MSGSEQNUM = 1) AS B
ON A.procdate = b.PROCDATE
	AND A.region_id = b.REGIONID
	AND A.office_id = b.OFFICEID
	AND A.cashsum_id = b.CASHSUMID
	AND A.transeq = b.TRANSEQNUM
WHERE a.procdate = @ProcDate
  AND region_id = @RegionID
  AND office_id = @OfficeID 
  AND cashsum_id = @CashsumId  
  AND transeq >= @TransSeqLow
  AND transeq <= @TransSeqHigh

END
GO