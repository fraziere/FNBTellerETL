CREATE PROCEDURE [etl].[GetOfficalCheckTran]
	@ProcDateLow DATETIME,
    @ProcDateHigh DATETIME
AS
BEGIN


  SELECT A.[procdate]
      ,A.[region_id]
      ,A.[office_id]
      ,A.[cashsum_id]
      ,A.[transeq]
      ,A.[sequence]
      ,A.[ISN]
      ,A.[chgDateTime]
      ,A.[source]
      ,A.[oper_id]
      ,A.[CRDR]
      ,A.[itemnumber]
      ,A.[serial]
      ,A.[field6]
      ,A.[abanumber]
      ,A.[field4]
      ,A.[account]
      ,A.[trancode]
      ,A.[amount]
	  ,B.OVERRIDETYPE
	  ,B.OVERRIDEID
	  ,C2.[NAME] AS OverriderName
	  ,B.TRANID
	  ,B.SOURCEOPERID
	  ,C.[NAME] AS EmployeeName
	  ,D.[NAME] AS OfficeName
	  ,A2.[REGIONID]
      ,A2.[OFFICEID]
      ,A2.[ID]
      ,A2.[EMPLOYEEID]
      ,A2.[OPERTYPEID]
      ,A2.[OPERSECPROFID]
      ,A2.[LIMITSPROFID]
      ,A2.[SKILLSPROFID]
      ,A2.[ALTOPERID]
      ,A2.[DATALEN]
      ,A2.[USERDATAAREA]
	  ,[SYS_LIMITSPROF].[DESCRIPTION] AS LimitsProfDesc
	  ,[SYS_OPERSECPROF].[DESCRIPTION] AS OperSecProfDesc
	  ,C2.[NAME] AS OverriderName2
	  ,A3.LIMITSPROFID
	  ,A31.[DESCRIPTION] AS OverriderLimits
  FROM (SELECT A0.*
		FROM (SELECT * FROM [ARGOENT].[dbo].[TLR_ITEM_MICR] WHERE amount >= 5000 and procdate >= @ProcDateLow and procdate <= @ProcDateHigh) as A0
				INNER JOIN (SELECT [GUID], MAX([sequence]) as maxSeq FROM [ARGOENT].[dbo].[TLR_ITEM_MICR] WHERE procdate >= @ProcDateLow and procdate <= @ProcDateHigh
							GROUP BY [GUID]) AS B1
							ON B1.[GUID] = A0.[GUID]
							AND B1.maxSeq = A0.[sequence]) AS A
  LEFT JOIN [ARGOENT].[dbo].[TLR_EJDATADETAIL] as B
  ON A.[procdate] = B.[procdate]
  AND A.region_id = B.REGIONID
  AND A.office_id = B.OFFICEID
  AND A.cashsum_id = B.CASHSUMID
  AND A.transeq = B.TRANSEQNUM
  AND A.[sequence] = b.MSGSEQNUM
  LEFT JOIN [ARGOENT].[dbo].[SYS_EMPLOYEE] as C
  ON A.oper_id = c.ID
  AND A.region_id = c.REGIONID
  LEFT JOIN [ARGOENT].[dbo].[SYS_EMPLOYEE] as C2
  ON B.OVERRIDEID = c2.ID
  AND B.REGIONID = c2.REGIONID
  LEFT JOIN [ARGOENT].[dbo].[SYS_OFFICE] as D
  ON A.region_id = D.REGIONID
  AND A.office_id = D.ID
  LEFT JOIN [ARGOENT].[dbo].[SYS_OPER] as A2
  ON A2.REGIONID = A.region_id
  AND A2.OFFICEID = A.office_id
  AND A2.EMPLOYEEID = A.oper_id
  LEFT JOIN [ARGOENT].[dbo].[SYS_LIMITSPROF]
  ON A2.LIMITSPROFID = [SYS_LIMITSPROF].ID
  LEFT JOIN [ARGOENT].[dbo].[SYS_OPERSECPROF]
  ON A2.OPERSECPROFID = [SYS_OPERSECPROF].ID
  LEFT JOIN [ARGOENT].[dbo].[SYS_OPER] as A3
  ON A3.REGIONID = B.REGIONID
  AND A3.OFFICEID = B.OFFICEID
  AND A3.EMPLOYEEID = B.OVERRIDEID
  LEFT JOIN [ARGOENT].[dbo].[SYS_LIMITSPROF] AS A31
  ON A3.LIMITSPROFID = A31.ID
  LEFT JOIN [ARGOENT].[dbo].[SYS_OPERSECPROF] AS A32
  ON A3.OPERSECPROFID = A32.ID
  WHERE PROCLISTNAME = 'PURCHASE'
  AND itemnumber = 11
  AND CRDR = 'Cr'
  AND REVERSED != 1
  ORDER BY amount DESC;

END
GO
