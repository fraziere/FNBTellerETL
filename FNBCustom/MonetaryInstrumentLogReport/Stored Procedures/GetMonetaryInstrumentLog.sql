CREATE PROCEDURE [etl].[GetMonetaryInstrumentLog]

AS
BEGIN
SET NOCOUNT ON;

DECLARE @date AS DATETIME
SET @date =  DATEADD(m, -6, GETDATE())

SELECT 
	[funding_transaction_number],
	[transaction_number],
	[transaction_date],
	[transaction_time],
	[cr_dr_ind],
	[amount],
	[fee_amount],
	[fee_funded_by_cash],
	[transaction_code],
	[transaction_description],
	[channel],
	[teller_id],
	[office_id],
	[branch_code],
	[branch_state],
	[branch_zip_code],
	[serial_number],
	[funding_transaction_type],
	[funding_account_type_code],
	[funding_account_number],
	[funding_check_number],
	[payee_name],
	[purchaser],
	[pur_customer_type],
	[pur_internal_type],
	[pur_occupation],
	[pur_address],
	[pur_city],
	[pur_state],
	[pur_zip_code],
	[pur_country],
	[date_of_birth],
	[id_type],
	[id_descr],
	[id_number],
	[id_state],
	[id_issuer],
	[tin_type],
	[tin_descr],
	[tin_number],
	[recourse_acct_number],
	[recourse_transit],
	[recourse_acct_type_code],
	[recourse_host_app_code],
	[recourse_host_region_code] 
FROM [FRAUD].[dbo].[vw_BSA_monetary_instruments_and_conductors] WITH (NOLOCK)

--go back 6 months
WHERE [transaction_date] BETWEEN CAST(DATEADD(d, -1, @date) AS DATE) AND CONVERT(DATE, GETDATE())

ORDER BY [transaction_time]

END
GO