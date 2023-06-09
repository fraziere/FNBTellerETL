CREATE PROCEDURE [etl].[CashAdvanceReportLogInfo]
	@ReportDate DateTime,
	@RegionId VARCHAR(4),
	@OfficeId VARCHAR(5),
	@OfficeName VARCHAR(40),
	@ProcessingDate DateTime,
	@TotalAmount decimal
AS
BEGIN

SET NOCOUNT ON;

INSERT INTO [etl].[CashAdvanceReportLog]
(
	[ReportDate],
	[RegionId],
	[OfficeId],
	[OfficeName],
	[ProcessingDate],
	[TotalAmount]
)
VALUES (
	@ReportDate,
	@RegionId,
	@OfficeId,
	@OfficeName,
	@ProcessingDate,
	@TotalAmount
);
END
GO