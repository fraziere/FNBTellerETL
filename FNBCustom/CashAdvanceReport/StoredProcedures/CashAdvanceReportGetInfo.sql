CREATE PROCEDURE [etl].[CashAdvanceReport]
	@From DATETIME,
	@To DATETIME
AS
BEGIN

SET NOCOUNT ON;

SELECT region_id AS RegionId, office_id AS OfficeId, office.[name] AS OfficeName, cast(procdate AS DATE) AS ProcessingDate, sum(amount) AS TotalAmount 
FROM [ARGOENT].[dbo].[TLR_ITEM_MICR] AS trans WITH (NOLOCK)
--left join should be unnecessary, but leaving it in case there are somehow trans associated
--with a deleted office, in that case region_id, office_id, and name will be blank/null
LEFT JOIN [ARGOENT].[dbo].[SYS_OFFICE] AS office WITH(NOLOCK) 
ON office.REGIONID = trans.region_id AND office.ID = trans.office_id
WHERE trans.[account] = '154456' -- 154456 is the account used to deposit the cash advance funds to from the customer account
AND trans.[crdr] = 'Dr' -- Dr is the Debit Indicator
AND trans.[procdate] between @From and @To
GROUP BY region_id, office_id, cast(procdate AS DATE), office.[name]
ORDER BY office.[name]

END
GO
