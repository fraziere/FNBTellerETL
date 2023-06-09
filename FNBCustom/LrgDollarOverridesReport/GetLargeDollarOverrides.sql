--SEE Word Document "Teller Application - The Electronic Journal.docx" for more information on the columns in TLR_EJ* tables
--ACCTNUM column stores the primary account number for the EJ record
--AMOUNT1 column stores the total transaction amt for the EJ record
--AMOUNT2 column stores the total check amount for the EJ record

CREATE PROCEDURE [etl].[GetLargeDollarOverrides]
	@From DATETIME,
	@To DATETIME
AS BEGIN

SET NOCOUNT ON;

SELECT 
	[PROCDATE] AS [Procdate],
	ejdd.[REGIONID] AS [RegionId],
	reg.[NAME] AS [RegionName],
	[OFFICEID] AS [OfficeId],
	sof.[NAME] AS [OfficeName],
	[CASHSUMID] AS [CashboxNum],
	[TRANSEQNUM] AS [TranSeqNum],
	[MSGSEQNUM] AS [MsgSeqNum],
	[OVERRIDEID] AS [OverrideId],
	(SELECT [NAME] FROM [ARGOENT].[dbo].[SYS_EMPLOYEE] WHERE [ID] = ejdd.[OVERRIDEID] AND [REGIONID] = ejdd.[REGIONID]) AS [OverrideName],
	[SOURCEOPERID] AS [SourceOperId],
	(SELECT [NAME] FROM [ARGOENT].[dbo].[SYS_EMPLOYEE] WHERE [ID] = ejdd.[SOURCEOPERID] AND [REGIONID] = ejdd.[REGIONID]) AS [TellerName],
	[ACCTNUM] AS [AcctNum],
	[AMOUNT1] AS [AmountOne],
	[AMOUNT2] AS [AmountTwo]
	
FROM [ARGOENT].[dbo].[TLR_EJDATADETAIL] AS ejdd WITH (NOLOCK)

--join to get region name
INNER JOIN [ARGOENT].[dbo].[SYS_REGION] AS reg WITH (NOLOCK)
	ON ejdd.[REGIONID] = reg.[ID]

--join to get branch name
INNER JOIN [ARGOENT].[dbo].[SYS_OFFICE] AS sof WITH (NOLOCK)
	ON ejdd.[OFFICEID] = sof.[ID] and ejdd.[REGIONID] = sof.[REGIONID]

WHERE [OVERRIDETYPE] = 'S' AND [APPLSUBFUNC] = 'DEP' AND [AMOUNT2] >= 25000 AND [REVERSED] != 1
	AND [PROCDATE] BETWEEN @From and @To

ORDER BY ejdd.[PROCDATE] ASC

END 
GO