PRINT 'Issue #96: Reworking UQ for special tables'

ALTER TABLE JavaTypeMethodNameSmali
DROP CONSTRAINT UQ_jtmnsName
GO

ALTER TABLE JavaTypeMethodSignature
DROP CONSTRAINT UQ_jtmsigSignature
GO

PRINT 'Issue #96: Creating UQ indices - DONE'


PRINT 'Issue #98: Adding index on JavaTypeMethod table'

CREATE NONCLUSTERED INDEX IDX_JavaTypeMethod_UQIndex
ON JavaTypeMethod (jtmJtypInTypeId, jtmJtmnsSmaliNameId, jtmJtmsigSignatureId, jtmAccessControl, jtmIsAbstract, jtmIsConstructor, jtmIsStatic, jtmSourceCodeIndexBeg, jtmSourceCodeIndexEnd)
GO

PRINT 'Issue #98: Adding index on JavaTypeMethod table - DONE'

PRINT 'Issue #99: Adding new table to store all text values'

CREATE TABLE StringValue
(
  strUId INT IDENTITY(1,1),
  strValue NVARCHAR(MAX) NOT NULL,
  strHash VARCHAR(64) NOT NULL,
  strType VARCHAR(4) NOT NULL,
  CONSTRAINT PK_StringValue PRIMARY KEY (strUId)
)
GO

CREATE UNIQUE INDEX UQX_strHash_strType ON StringValue (strHash, strType)
GO

SET IDENTITY_INSERT StringValue ON
INSERT StringValue(strUId, strValue, strHash, strType)
VALUES (0, '', 0x, 0)
SET IDENTITY_INSERT StringValue OFF
GO

PRINT 'Issue #99: Adding new table to store all text values - DONE'