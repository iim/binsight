-- This table stores Smali version of java type name
CREATE TABLE JavaTypeNameShort
(
  jtnsUId INT IDENTITY(1,1) PRIMARY KEY,
  jtnsName NVARCHAR(448) NOT NULL,
)
GO

SET IDENTITY_INSERT JavaTypeNameShort ON
GO

INSERT INTO JavaTypeNameShort (jtnsUId, jtnsName) VALUES(0,'')
GO

SET IDENTITY_INSERT JavaTypeNameShort OFF
GO

ALTER TABLE JavaTypeNameShort
ADD CONSTRAINT UQ_jtnsName UNIQUE (jtnsName)
GO

-- This table stores Smali version of java type name
CREATE TABLE JavaTypeNameShortShort
(
  jtnssUId INT IDENTITY(1,1) PRIMARY KEY,
  jtnssName NVARCHAR(448) NOT NULL,
)
GO

SET IDENTITY_INSERT JavaTypeNameShortShort ON
GO

INSERT INTO JavaTypeNameShortShort (jtnssUId, jtnssName) VALUES(0,'')
GO

SET IDENTITY_INSERT JavaTypeNameShortShort OFF
GO

ALTER TABLE JavaTypeNameShortShort
ADD CONSTRAINT UQ_jtnssName UNIQUE (jtnssName)
GO

ALTER TABLE JavaType ADD jtypJtnsShortNameId INT NULL
GO

ALTER TABLE JavaType ADD jtypJtnssShortShortNameId INT NULL
GO

UPDATE JavaType
SET jtypJtnsShortNameId = 0, jtypJtnssShortShortNameId = 0
WHERE 1=1
GO

ALTER TABLE JavaType ALTER COLUMN jtypJtnsShortNameId INT NOT NULL
GO

ALTER TABLE JavaType ALTER COLUMN jtypJtnssShortShortNameId INT NOT NULL
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypJtnsShortNameId
FOREIGN KEY (jtypJtnsShortNameId)
REFERENCES JavaTypeNameShort(jtnsUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypJtnssShortShortNameId
FOREIGN KEY (jtypJtnssShortShortNameId)
REFERENCES JavaTypeNameShortShort(jtnssUId)
GO

DROP INDEX IDX_JavaType_InserSelectSuperClass_Case1 ON JavaType
GO

DROP INDEX IDX_JavaType_InserSelectSuperClass_Case2And3 ON JavaType
GO

DROP INDEX IDX_JavaType_InserSelectOuterClass ON JavaType
GO

ALTER TABLE JavaType DROP COLUMN jtypNameShort
GO

CREATE NONCLUSTERED INDEX IDX_JavaType_InserSelectSuperClass_Case1
ON JavaType (jtypJtpnPackageNameId, jtypJtnfpProcessedFullNameId, jtypJtnfsSmaliFullNameId, jtypJtnsShortNameId,jtypJtnssShortShortNameId, jtypIsReferenceOnly)
GO

-- Also works for SelectTypeIdForNotInApkShallowReference query
CREATE NONCLUSTERED INDEX IDX_JavaType_InserSelectSuperClass_Case2And3
ON JavaType (jtypBioParentApkId, jtypBocParentContentId, jtypJtpnPackageNameId, jtypJtnfpProcessedFullNameId, jtypJtnfsSmaliFullNameId, jtypJtnsShortNameId,jtypJtnssShortShortNameId, jtypIsReferenceOnly)
GO

CREATE NONCLUSTERED INDEX IDX_JavaType_InserSelectOuterClass
ON JavaType (jtypJtpnPackageNameId, jtypJtnfpProcessedFullNameId, jtypJtnfsSmaliFullNameId, jtypJtnsShortNameId,jtypJtnssShortShortNameId, jtypJtypOuterClassId)
GO

DROP INDEX UQ_EnumOptionInClass ON EnumOptionInClass 
GO
ALTER TABLE EnumOptionInClass ALTER COLUMN encStrValue NVARCHAR(MAX)
GO
ALTER TABLE EnumOptionInClass ADD encStrValueShort NVARCHAR(400) NULL
GO

UPDATE EnumOptionInClass
SET encStrValueShort = encStrValue
GO

CREATE UNIQUE INDEX UQ_EnumOptionInClass
ON EnumOptionInClass(encJtypEnumId, encEnoOptionId, encIntValue, encStrValueShort)
GO

PRINT 'Issue #66: Adding tables to store information about methods in a type - ...'

CREATE TABLE JavaTypeMethodNameSmali
(
  jtmnsUId INT IDENTITY(1,1),
  jtmnsName NVARCHAR(448) NOT NULL,
  jtmnsLongName NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_JavaTypeMethodNameSmali PRIMARY KEY (jtmnsUId)
)
GO

CREATE TABLE JavaTypeMethodSignature
(
  jtmsigUId INT IDENTITY(1,1),
  jtmsigSignature NVARCHAR(448) NOT NULL,
  jtmsigFullSignature NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_JavaTypeMethodSignature PRIMARY KEY (jtmsigUId)
)
GO

CREATE TABLE JavaTypeMethod
(
  jtmUId INT IDENTITY(1, 1),
  jtmJtypInTypeId INT NOT NULL,
  jtmJtmnsSmaliNameId INT NOT NULL,
  jtmJtmsigSignatureId INT NOT NULL,
  jtmAccessControl INT NOT NULL,
  jtmIsAbstract BIT NOT NULL,
  jtmIsConstructor BIT NOT NULL,
  jtmIsStatic BIT NOT NULL,
  jtmJtypReturnType INT NOT NULL,
  jtmSourceCodeIndexBeg INT NOT NULL,
  jtmSourceCodeIndexEnd INT NOT NULL
  CONSTRAINT PK_JavaTypeMethod PRIMARY KEY (jtmUId)
)

CREATE TABLE JavaTypeParameterName
(
  jtpnUId INT IDENTITY(1,1),
  jtpnName NVARCHAR(448) NOT NULL,
  CONSTRAINT PK_JavaTypeParameterName PRIMARY KEY (jtpnUId)
)
GO

CREATE TABLE JavaTypeMethodParameters
(
  jtmpUId INT IDENTITY(1,1),
  jtmpJtmMethodId INT NOT NULL,
  jtmpJtypOfTypeId INT NOT NULL,
  jtmpJtmpnNameId INT NOT NULL,
  jtmpIndex INT NOT NULL,
  CONSTRAINT PK_JavaTypeMethodParameters PRIMARY KEY (jtmpUId)
)
GO

-- JavaTypeMethodNameSmali (not unique since jtmsigSignature is truncated)
CREATE NONCLUSTERED INDEX IDX_jtmnsName
ON JavaTypeMethodNameSmali (jtmnsName)
GO

-- JavaTypeMethod table
CREATE NONCLUSTERED INDEX IDX_JavaTypeMethod_NameAndSignature
ON JavaTypeMethod (jtmJtypInTypeId, jtmJtmnsSmaliNameId, jtmJtmsigSignatureId)
GO

CREATE NONCLUSTERED INDEX IDX_jtmJtypInTypeId
ON JavaTypeMethod (jtmJtypInTypeId)
GO

CREATE NONCLUSTERED INDEX IDX_jtmJtypReturnType
ON JavaTypeMethod (jtmJtypReturnType)
GO

-- JavaTypeMethodSignature (not unique since jtmsigSignature is truncated)
CREATE NONCLUSTERED INDEX IDX_jtmsigSignature
ON JavaTypeMethodSignature (jtmsigSignature)
GO

-- JavaTypeParameterName table
CREATE UNIQUE INDEX UQ_JavaTypeParameterName
ON JavaTypeParameterName(jtpnName)
GO

-- JavaTypeMethodParameters table
CREATE NONCLUSTERED INDEX IDX_JavaTypeMethodParameters
ON JavaTypeMethodParameters (jtmpJtmMethodId, jtmpJtypOfTypeId, jtmpJtmpnNameId)
GO

ALTER TABLE JavaTypeMethod ADD CONSTRAINT FK_jtmJtypInTypeId
FOREIGN KEY (jtmJtypInTypeId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeMethod ADD CONSTRAINT FK_jtmJtypReturnType
FOREIGN KEY (jtmJtypReturnType)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeMethod ADD CONSTRAINT FK_jtmJtmnsSmaliNameId
FOREIGN KEY (jtmJtmnsSmaliNameId)
REFERENCES JavaTypeMethodNameSmali(jtmnsUId)
GO

ALTER TABLE JavaTypeMethod ADD CONSTRAINT FK_jtmJtmsigSignatureId
FOREIGN KEY (jtmJtmsigSignatureId)
REFERENCES JavaTypeMethodSignature(jtmsigUId)
GO

ALTER TABLE JavaTypeMethodParameters ADD CONSTRAINT FK_jtmpJtmMethodId
FOREIGN KEY (jtmpJtmMethodId)
REFERENCES JavaTypeMethod(jtmUId)
GO

ALTER TABLE JavaTypeMethodParameters ADD CONSTRAINT FK_jtmpJtypOfTypeId
FOREIGN KEY (jtmpJtypOfTypeId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeMethodParameters ADD CONSTRAINT FK_jtmpJtmpnNameId
FOREIGN KEY (jtmpJtmpnNameId)
REFERENCES JavaTypeParameterName(jtpnUId)
GO
PRINT 'Issue #66: Adding tables to store information about methods in a type - DONE'
GO

PRINT 'Issue #67: Saving methods methods info and bug fixing '
ALTER TABLE EnumOption ALTER COLUMN enoNameShort NVARCHAR(444) NOT NULL
GO

SET IDENTITY_INSERT JavaTypeMethodSignature ON
GO

INSERT INTO JavaTypeMethodSignature (jtmsigUId, jtmsigSignature, jtmsigFullSignature) VALUES(0, '', '')
GO

SET IDENTITY_INSERT JavaTypeMethodSignature OFF
GO

PRINT 'Issue #67: Saving methods methods info and bug fixing - DONE'


PRINT 'Issue #73: Creating UQ indices'

ALTER TABLE JavaTypeFileName
ADD CONSTRAINT UQ_jtfnName UNIQUE (jtfnName)
GO

ALTER TABLE JavaTypeMethodNameSmali
ADD CONSTRAINT UQ_jtmnsName UNIQUE (jtmnsName)
GO

ALTER TABLE JavaTypeMethodSignature
ADD CONSTRAINT UQ_jtmsigSignature UNIQUE (jtmsigSignature)
GO

ALTER TABLE JavaTypePath
ADD CONSTRAINT UQ_jtpName UNIQUE (jtpName)
GO

PRINT 'Issue #73: Creating UQ indices - DONE'
