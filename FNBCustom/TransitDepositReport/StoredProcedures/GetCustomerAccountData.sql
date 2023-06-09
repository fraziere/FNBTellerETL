CREATE PROCEDURE [etl].[GetCustomerAccountData]
     @AccountList AccountsShortList READONLY
AS
BEGIN



SELECT A.[transit]
      ,A.[acct]
	  ,D.[description]
      ,C.customer_id
      ,C.full_name
	  ,C.entity_type
	  ,B.contypeid
  FROM [FRAUD].[dbo].[AML_ACCOUNTS] AS A
 left outer Join (SELECT [contypeid],[left_obj],[right_obj]
				   FROM [FRAUD].[dbo].[AML_CONNECTIONS]
				   WHERE [is_current] = 1) AS B on A.acctndx = B.right_obj
 Left outer Join [FRAUD].dbo.AML_ENTITIES AS C on B.left_obj = C.entity_id
 LEFT JOIN [FRAUD].[dbo].[AML_ACCT_TYPES] AS D
  ON A.acct_type = D.acct_type
  WHERE acct IN (Select * from @AccountList)

END
GO
