CREATE TABLE [etl].[LeasePaymentReportLog]
	(
		[UtilCustAccountNumber] VARCHAR(255),
		[UtilLeaCustomerName] VARCHAR(255),
		[PROCDATE] [datetime],
		[Amount] decimal,
		[Office] char(5),
		[NAME] char(40),
		[Cashbox] char(4),
		[ReportDate] datetime NOT NULL Default GETDATE(),
		[CheckNum] VARCHAR(255)
	);