CREATE PROCEDURE [etl].[CheckMICRInfo]
	@RegionID VARCHAR,
	@OfficeID VARCHAR,
	@TransSeq VARCHAR,
	@CashsumId VARCHAR,
	@ProcDate DATETIME
AS
BEGIN

SET NOCOUNT ON;

SELECT [sequence],[ISN],[serial],[field6],[trancode],[amount]
FROM [ARGOENT].[dbo].[TLR_ITEM_MICR] WITH (NOLOCK)
WHERE region_id = @RegionID
  AND office_id = @OfficeID
  AND transeq = @TransSeq
  AND cashsum_id = @CashsumId
  AND CRDR = 'Dr'
  AND procdate = @ProcDate

END
GO