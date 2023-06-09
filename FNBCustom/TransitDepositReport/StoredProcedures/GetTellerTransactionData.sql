﻿CREATE PROCEDURE [etl].[GetTellerTransactionData]
	@ProcDateLow DATETIME,
    @ProcDateHigh DATETIME
AS
BEGIN



SELECT A1.procdate,A1.region_id,A1.office_id,C1.[NAME] as officeName,
A1.cashsum_id,A1.transeq,A1.[GUID],A1.[sequence],A1.ISN,
A1.[source],A1.oper_id,A1.CRDR,A1.itemnumber,A1.serial,A1.field6,
A1.abanumber,A1.field4,A1.account,A1.trancode,A1.amount,
B1.TransactionSumNum,B1.ITEMCOUNT,B1.CREDITAMOUNT,B1.CREDITCOUNT,B1.DEBITAMOUNT,B1.DEBITCOUNT,
B1.TRANID,B1.PROCLISTNAME,
B1.ACCTNUM,B1.ACCTYPE1,B1.AMOUNT1
FROM (SELECT A.*, B.REVERSAL
	  FROM [ARGOENT].[dbo].[TLR_ITEM_MICR] AS A
	  LEFT JOIN [ARGOENT].[dbo].[TLR_EJDATADETAIL] AS B
	  ON A.procdate = B.PROCDATE
	  AND A.region_id = B.REGIONID
	  AND A.office_id = B.OFFICEID
	  AND A.cashsum_id = B.CASHSUMID
	  AND A.transeq = B.TRANSEQNUM
	  AND A.[sequence] = B.MSGSEQNUM
	  INNER JOIN (SELECT MAX([sequence]) as maxSeq, [GUID] as GUID_2
				  FROM [ARGOENT].[dbo].[TLR_ITEM_MICR]
				  GROUP BY [GUID]) AS C
		ON A.[GUID] = C.GUID_2
		AND A.[sequence] = maxSeq) AS A1

LEFT JOIN (SELECT B.TransactionSumNum, C.ITEMCOUNT, C.CREDITAMOUNT, C.CREDITCOUNT, C.DEBITAMOUNT, C.DEBITCOUNT, A.*
		  FROM [ARGOENT].[dbo].[TLR_EJDATADETAIL] AS A 
		  LEFT JOIN (SELECT MIN(B.TRANSEQNUM) AS TransactionSumNum, A.PROCDATE, A.REGIONID, A.OFFICEID, A.CASHSUMID, A.TRANSEQNUM, A.MSGSEQNUM
					  FROM [ARGOENT].[dbo].[TLR_EJDATADETAIL] AS A
					  LEFT JOIN [ARGOENT].[dbo].[TLR_EJTRAN] AS B
					  ON A.PROCDATE = B.PROCDATE
					  AND A.REGIONID = B.REGIONID
					  AND A.OFFICEID = B.OFFICEID
					  AND A.CASHSUMID = B.CASHSUMID
					  AND B.TRANSEQNUM >= A.TRANSEQNUM  
					  GROUP BY A.PROCDATE, A.REGIONID, A.OFFICEID, A.CASHSUMID, A.TRANSEQNUM, A.MSGSEQNUM) AS B
			ON A.PROCDATE = B.PROCDATE
			AND A.REGIONID = B.REGIONID
			AND A.OFFICEID = B.OFFICEID
			AND A.CASHSUMID = B.CASHSUMID
			AND A.TRANSEQNUM = B.TRANSEQNUM
			AND A.MSGSEQNUM = B.MSGSEQNUM
			LEFT JOIN [ARGOENT].[dbo].[TLR_EJTRAN] AS C
			ON  A.PROCDATE = C.PROCDATE
			AND B.TransactionSumNum = C.TRANSEQNUM
			AND A.REGIONID = C.REGIONID
			AND A.OFFICEID = C.OFFICEID
			AND A.CASHSUMID = C.CASHSUMID) AS B1
ON B1.PROCDATE = A1.procdate
AND B1.REGIONID = A1.region_id
AND B1.OFFICEID = A1.office_id
AND B1.CASHSUMID = A1.cashsum_id
AND B1.TRANSEQNUM = A1.transeq
--AND B1.MSGSEQNUM = A1.[sequence]
LEFT JOIN [ARGOENT].[dbo].[SYS_OFFICE] AS C1
ON A1.office_id = C1.ID
AND A1.region_id = C1.REGIONID
WHERE A1.procdate >= @ProcDateLow
AND A1.procdate < @ProcDateHigh

AND B1.TRANID = 'TDEP3PRC'
AND A1.CRDR = 'Dr'
AND (CREDITAMOUNT >= 500000 OR DEBITAMOUNT >= 500000)

END
GO
