CREATE PROCEDURE [etl].[SetAmlAccountsCtrFlag]
	@AcctType VARCHAR(50),
	@AcctNum VARCHAR(50)
AS
BEGIN

	IF @AcctType = 'DDA'
		BEGIN
		UPDATE [FRAUD].[dbo].[AML_ACCOUNTS]
		   SET [ctr_exemption] = 'Y'
		 WHERE [acct] = @AcctNum
		   AND acct_type = '2'
	END
	IF @AcctType = 'SAV'
	   BEGIN
	   UPDATE [FRAUD].[dbo].[AML_ACCOUNTS]
	      SET [ctr_exemption] = 'Y'
	    WHERE [acct] = @AcctNum
	      AND acct_type = '4'
	  END
END
GO