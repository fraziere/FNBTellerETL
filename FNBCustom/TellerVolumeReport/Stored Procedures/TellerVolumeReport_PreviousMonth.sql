CREATE PROCEDURE [etl].[TellerVolumeReport_PreviousMonth]
AS

BEGIN

SET NOCOUNT ON;

SELECT
	reg.[NAME] AS [Region],
	reg.[ID] AS [Region ID],
	ejd.[OFFICEID] AS [Branch Number],
	sof.[NAME] AS [Branch Name],
	ejd.[CASHSUMID] AS [Cashbox Number],
	CASE
		WHEN ejd.[CASHSUMID] = '98' OR ejd.[CASHSUMID] = '99' THEN (SELECT CONCAT(RTRIM(LTRIM((SELECT DISTINCT [NAME] FROM [ARGOENT].[dbo].[SYS_EMPLOYEE] WHERE [id] = ejd.[SOURCEOPERID]))), ' / Vault'))
		WHEN ejd.[CASHSUMID] = '91' OR ejd.[CASHSUMID] = '92' OR ejd.[CASHSUMID] = '93' THEN (SELECT CONCAT(RTRIM(LTRIM((SELECT DISTINCT [NAME] FROM [ARGOENT].[dbo].[SYS_EMPLOYEE] WHERE [id] = ejd.[SOURCEOPERID]))), ' / Recycler'))
		ELSE
			(SELECT DISTINCT [NAME] FROM [ARGOENT].[dbo].[SYS_EMPLOYEE] WHERE [id] = ejd.[SOURCEOPERID])
	END AS [Teller Name],

	count(case WHEN ejd.[LOC_CODE] LIKE 'L%' then 1 else null end) AS [Lobby Transactions],
	count(case WHEN ejd.[LOC_CODE] LIKE 'D%' then 1 else null end) AS [Drive Up Transactions],
	count(case WHEN ejd.[LOC_CODE] NOT LIKE 'D%' AND ejd.[LOC_CODE] NOT LIKE 'L%' then 1 else null end) AS [UnSpecified Transactions],

	COUNT(*) AS [Total Transaction Count]
FROM [ARGOENT].[dbo].[TLR_EJDATADETAIL] AS ejd WITH (NOLOCK)

INNER JOIN [ARGOENT].[dbo].[SYS_REGION] AS reg WITH (NOLOCK)
	ON ejd.[REGIONID] = reg.[ID]

INNER JOIN [ARGOENT].[dbo].[SYS_OFFICE] AS sof WITH (NOLOCK)
	ON ejd.[OFFICEID] = sof.[ID] and ejd.[REGIONID] = sof.[REGIONID]

--between first day of previous month and first day of this month
WHERE ejd.[PROCDATE] between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0) and DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)
	AND ([APPLSUBFUNC] = 'DEP' OR [APPLSUBFUNC] = 'PMT' OR [APPLSUBFUNC] = 'WDL' OR [APPLSUBFUNC] = 'PUR' 
		OR [APPLSUBFUNC] = 'CCKO' OR [APPLSUBFUNC] = 'CCKT' OR [APPLSUBFUNC] = 'CCOR' OR [APPLSUBFUNC] = 'TCLS' 
		OR [APPLSUBFUNC] = 'TRAN' OR [APPLSUBFUNC] = 'ADV' OR [APPLSUBFUNC] = 'FREX' OR [APPLSUBFUNC] = 'SBR' OR [APPLSUBFUNC] = 'DV1'
		OR [APPLSUBFUNC] = 'GLA' OR [APPLSUBFUNC] = 'BKOB' OR [APPLSUBFUNC] = 'BYSL' OR [APPLSUBFUNC] = 'SPCH')
    AND [REVERSED] != 1 AND REVERSAL != 1

GROUP BY reg.[NAME], reg.[ID], ejd.[OFFICEID], sof.[NAME], ejd.[CASHSUMID], ejd.[SOURCEOPERID]
ORDER BY [Branch Number], reg.[NAME]

END
GO