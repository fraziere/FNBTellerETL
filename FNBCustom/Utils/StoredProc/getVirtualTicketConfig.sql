
CREATE PROCEDURE [etl].[getVirtualTicketConfig]
	@RoutingNumber VARCHAR(10),
	@AccountNumber VARCHAR(12),
	@TranCode VARCHAR(10),
	@Serial VARCHAR(10),
	@ItemType VARCHAR(10)
AS
BEGIN

SET NOCOUNT ON;

SELECT TOP (1) [Region]
      ,[Office]
      ,[ItemName]
      ,[Description]
      ,[RoutingNumber]
      ,[AccountNumber]
      ,[TranCode]
      ,[Serial]
      ,[ItemType]
  FROM [FNBCustom].[dbo].[DE_VirtualConfigTickets_CheatSheet]
  WHERE RoutingNumber = @RoutingNumber
  AND AccountNumber = @AccountNumber
  AND TranCode = @TranCode
  AND Serial = @Serial
  AND ItemType = @ItemType

END
GO
GO