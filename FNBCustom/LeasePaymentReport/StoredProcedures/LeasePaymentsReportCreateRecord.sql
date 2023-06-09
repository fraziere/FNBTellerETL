CREATE PROCEDURE [etl].[LeasePaymentsReportCreateRecord]
	@UtilCustAccountNumber VARCHAR(255),
	@UtilLeaCustomerName VARCHAR(255),
	@PROCDATE DateTime,
	@Amount decimal,
	@Office char(5),
	@Name char(40),
	@Cashbox char(4),
	@CheckNum VARCHAR(255)
AS
BEGIN

SET NOCOUNT ON;



	INSERT INTO [etl].[LeasePaymentReportLog]
	(
		[UtilCustAccountNumber],
		[UtilLeaCustomerName],
		[PROCDATE],
		[Amount],
		[Office],
		[NAME],
		[Cashbox],
		[CheckNum]
	)
	VALUES (
		@UtilCustAccountNumber,
		@UtilLeaCustomerName,
		@PROCDATE,
		@Amount,
		@Office,
		@Name,
		@Cashbox,
		@CheckNum
		);

END
GO
