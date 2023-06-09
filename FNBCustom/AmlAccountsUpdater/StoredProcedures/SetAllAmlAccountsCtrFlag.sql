CREATE PROCEDURE [etl].[SetAllAmlAccountsCtrFlag]
AS
BEGIN

UPDATE [FRAUD].[dbo].[AML_ACCOUNTS]
SET [ctr_exemption] = 'N'

END
GO