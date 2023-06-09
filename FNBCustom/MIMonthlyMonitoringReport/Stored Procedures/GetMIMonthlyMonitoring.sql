CREATE PROCEDURE [etl].[GetMIMonthlyMonitoring]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select transaction_date,transaction_code,transaction_description,serial_number,amount,funding_transaction_type,funding_account_type_code,
	funding_account_number,payee_name,purchaser as urchaser_name,pur_customer_type,pur_internal_type,funding_transaction_number,transaction_number,
	fee_amount,fee_funded_by_cash,tin_type,tin_descr,tin_number,recourse_acct_type_code,recourse_acct_number,recourse_transit,teller_id,
	EMP.NAME as teller_name,branch_code
	from [FRAUD].[dbo].[vw_BSA_monetary_instruments_and_conductors] WITH(NOLOCK) 
	left outer join [ARGOENT].[dbo].[SYS_EMPLOYEE] as EMP WITH(NOLOCK)
	on teller_id = id
	where transaction_date >= cast(getdate()-90 as date)
	order by transaction_date
END
GO