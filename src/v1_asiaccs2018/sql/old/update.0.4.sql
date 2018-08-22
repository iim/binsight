-- This table stores links between a class and an implemented interface 
CREATE TABLE JavaTypeImplementedInterface
(
  jtiiUId INT NOT NULL IDENTITY(1,1),
  jtiiJtypClassId INT NOT NULL,
  jtiiJtypInterfaceId INT NOT NULL,
  CONSTRAINT PK_JavaTypeImplementedInterface PRIMARY KEY (jtiiUId)
)
GO

ALTER TABLE JavaTypeImplementedInterface ADD CONSTRAINT FK_jtiiJtypClassId
FOREIGN KEY (jtiiJtypClassId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeImplementedInterface ADD CONSTRAINT FK_jtiiJtypInterfaceId
FOREIGN KEY (jtiiJtypInterfaceId)
REFERENCES JavaType(jtypUId)
GO

CREATE UNIQUE INDEX IDX_UQ_JavaTypeImplementedInterface
ON JavaTypeImplementedInterface (jtiiJtypClassId, jtiiJtypInterfaceId)
GO

PRINT 'Issue #55: Table to store DataSet'

-- Table contains all datasets that we have
CREATE TABLE DataSet
(
  dstUId INT NOT NULL IDENTITY(1,1),
  dstName NVARCHAR(1024) NOT NULL,
  dstSource NVARCHAR(128) NOT NULL,
  dstBioCount INT NOT NULL,
  dstDownloadDateBeg DATETIME NOT NULL,
  dstDownloadDateEnd DATETIME NOT NULL,
  CONSTRAINT PK_DataSet PRIMARY KEY (dstUId)
)
GO

SET IDENTITY_INSERT DataSet ON
GO
INSERT INTO DataSet (dstUId, dstName, dstSource, dstBioCount, dstDownloadDateBeg, dstDownloadDateEnd) 
VALUES (0, '0', '0', 0, '01-01-1900', '01-01-1900')
GO
INSERT INTO DataSet (dstUId, dstName, dstSource, dstBioCount, dstDownloadDateBeg, dstDownloadDateEnd) 
VALUES (1, 'TOP 100 PerCategory', 'LERSSE/UBC', 0, '06-01-2015', '06-30-2015')
GO
SET IDENTITY_INSERT DataSet OFF
GO

PRINT 'Issue #55: Updating Application Count for DataSet #1'
UPDATE DataSet
SET dstBioCount = 
    (
      SELECT COUNT(*)
      FROM BinaryObject
      WHERE bioIsRoot = 1
    )
WHERE dstUId = 1
GO

PRINT 'Issue #55: Adding a connection table between DataSets and Application Categories'
-- This table stores connection between ApplicationCategories and DataSet, it also includes some stats for category for a dataset
CREATE TABLE DataSetApplicationCategories
(
  dstcatUId INT NOT NULL IDENTITY(1,1),
  dstcatDstDataSetId INT NOT NULL,
  dstcatApcApplicationCategoryId INT NOT NULL,
  dstcatBioCount INT NOT NULL,
  CONSTRAINT PK_DataSetApplicationCategories PRIMARY KEY (dstcatUId)
)
GO

PRINT 'Issue #55: Adding FKs to DataSetApplicationCategories'

ALTER TABLE DataSetApplicationCategories ADD CONSTRAINT FK_dstcatDstDataSetId
FOREIGN KEY (dstcatDstDataSetId)
REFERENCES DataSet(dstUId)
GO

ALTER TABLE DataSetApplicationCategories ADD CONSTRAINT FK_dstcatApcApplicationCategoryId
FOREIGN KEY (dstcatApcApplicationCategoryId)
REFERENCES ApplicationCategory(apcUId)
GO

PRINT 'Issue #55: Adding FKs to DataSetApplicationCategories - DONE'

PRINT 'Issue #55: Adding NULL row'

SET IDENTITY_INSERT DataSetApplicationCategories ON
GO
INSERT INTO DataSetApplicationCategories (dstcatUId, dstcatDstDataSetId, dstcatApcApplicationCategoryId, dstcatBioCount) 
VALUES (0, 0, 1, 0)
GO
SET IDENTITY_INSERT DataSetApplicationCategories OFF
GO

PRINT 'Issue #55: Adding NULL row - DONE'

PRINT 'Issue #55: Adding all Android Categories to DataSet 1'
INSERT INTO DataSetApplicationCategories (dstcatDstDataSetId, dstcatApcApplicationCategoryId, dstcatBioCount)
SELECT 1, apcUId, 0
FROM ApplicationCategory
GO

PRINT 'Issue #55: Adding all Android Categories to DataSet 1 - DONE'


PRINT 'Issue #55: Adding reference column to DataSetApplicationCategories table from BinaryObject'

-- Now we need to replace the old apc link with the new one.
ALTER TABLE BinaryObject ADD bioDstcatDataSetApplicationCategoryId INT NULL
GO

PRINT 'Issue #55: Adding reference column to DataSetApplicationCategories table from BinaryObject - DONE'

PRINT 'Issue #55: Updating bioDstcatDataSetApplicationCategoryId column value (this part might take a while)'

UPDATE BinaryObject
SET bioDstcatDataSetApplicationCategoryId = 0
WHERE bioUId = 0
GO

UPDATE BinaryObjectNew
SET bioDstcatDataSetApplicationCategoryId = dstcatUId
FROM BinaryObject AS BinaryObjectNew
  INNER JOIN DataSetApplicationCategories ON dstcatApcApplicationCategoryId = bioApcCategoryId
WHERE 1=1
  AND dstcatApcApplicationCategoryId = bioApcCategoryId
  AND dstcatDstDataSetId = 1
  AND bioDstcatDataSetApplicationCategoryId IS NULL
GO

PRINT 'Issue #55: Updating bioDstcatDataSetApplicationCategoryId column value - DONE'

ALTER TABLE BinaryObject ALTER COLUMN bioDstcatDataSetApplicationCategoryId INT NOT NULL
GO

PRINT 'Issue #55: Update counts [dbo].[DataSetApplicationCategories] table'
UPDATE DataSetApplicationCategories
SET DataSetApplicationCategories.dstcatBioCount = 
  (
    SELECT COUNT(*) 
    FROM BinaryObject 
    WHERE BinaryObject.bioDstcatDataSetApplicationCategoryId = dstcatUId
  )
WHERE dstcatUId > 0
GO
PRINT 'Issue #55: Update counts [dbo].[DataSetApplicationCategories] table - DONE'

PRINT 'Issue #55: Adding FK on the new column in Binary Object'
ALTER TABLE BinaryObject ADD CONSTRAINT FK_bioDstcatDataSetApplicationCategoryId
FOREIGN KEY (bioDstcatDataSetApplicationCategoryId)
REFERENCES DataSetApplicationCategories(dstcatUId)
GO
PRINT 'Issue #55: Adding FK on the new column in Binary Object - DONE'

PRINT 'Issue #55: Dropping FK_bioApcCategoryId and bioApcCategoryId'
ALTER TABLE BinaryObject
DROP CONSTRAINT FK_bioApcCategoryId
GO
ALTER TABLE BinaryObject DROP COLUMN bioApcCategoryId
GO
PRINT 'Issue #55: Dropping FK_bioApcCategoryId and bioApcCategoryId - DONE'

PRINT 'Issue #43: Adding table to store enum otion values'
CREATE TABLE EnumOption
(
  enoUId INT NOT NULL IDENTITY(1,1),
  enoSmaliFullNameId INT NOT NULL,
  enoProcessedFullNameId INT NOT NULL,
  enoNameShort NVARCHAR(128) NOT NULL,
  CONSTRAINT PK_EnumOption PRIMARY KEY (enoUId)
)
GO

ALTER TABLE EnumOption ADD CONSTRAINT FK_enoSmaliFullNameId
FOREIGN KEY (enoSmaliFullNameId)
REFERENCES JavaTypeNameFullSmali(jtnfsUId)
GO

ALTER TABLE EnumOption ADD CONSTRAINT FK_enoProcessedFullNameId
FOREIGN KEY (enoProcessedFullNameId)
REFERENCES JavaTypeNameFullProcessed(jtnfpUId)
GO

CREATE UNIQUE INDEX UQ_EnumOption
ON EnumOption(enoSmaliFullNameId, enoProcessedFullNameId, enoNameShort)
GO

CREATE TABLE EnumOptionInClass
(
  encUId INT NOT NULL IDENTITY(1,1),
  encJtypEnumId INT NOT NULL,
  encEnoOptionId INT NOT NULL,
  encIntValue INT NULL,
  encStrValue NVARCHAR(256) NULL,
  CONSTRAINT PK_EnumOptionInClass PRIMARY KEY (encUId)
)
GO

ALTER TABLE EnumOptionInClass ADD CONSTRAINT FK_encJtypEnumId
FOREIGN KEY (encJtypEnumId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE EnumOptionInClass ADD CONSTRAINT FK_encEnoOptionId
FOREIGN KEY (encEnoOptionId)
REFERENCES EnumOption(enoUId)
GO

CREATE UNIQUE INDEX UQ_EnumOptionInClass
ON EnumOptionInClass(encJtypEnumId, encEnoOptionId, encIntValue, encStrValue)
GO

PRINT 'Issue #43: Adding table to store enum otion values - DONE'

PRINT 'Issue #39: Creating all required indexes on JavaType table'

CREATE NONCLUSTERED INDEX IDX_JavaType_InserSelectSuperClass_Case1
ON JavaType (jtypJtpnPackageNameId, jtypJtnfpProcessedFullNameId, jtypJtnfsSmaliFullNameId, jtypNameShort, jtypIsReferenceOnly)
GO

-- Also works for SelectTypeIdForNotInApkShallowReference query
CREATE NONCLUSTERED INDEX IDX_JavaType_InserSelectSuperClass_Case2And3
ON JavaType (jtypBioParentApkId, jtypBocParentContentId, jtypJtpnPackageNameId, jtypJtnfpProcessedFullNameId, jtypJtnfsSmaliFullNameId, jtypNameShort, jtypIsReferenceOnly)
GO

CREATE NONCLUSTERED INDEX IDX_JavaType_InserSelectOuterClass
ON JavaType (jtypJtpnPackageNameId, jtypJtnfpProcessedFullNameId, jtypJtnfsSmaliFullNameId, jtypNameShort, jtypJtypOuterClassId)
GO

CREATE NONCLUSTERED INDEX IDX_JavaType_jtypBioParentApkId
ON JavaType (jtypBioParentApkId)
GO

CREATE NONCLUSTERED INDEX IDX_JavaType_ReReferenceSuperClassInApkToNewType
ON JavaType (jtypBioParentApkId, jtypJtypSuperClassId)
GO

CREATE NONCLUSTERED INDEX IDX_JavaType_ReReferenceOutterClassInApkToNewType
ON JavaType (jtypBioParentApkId, jtypJtypOuterClassId)
GO

CREATE NONCLUSTERED INDEX IDX_JavaTypeImplementedInterface_jtiiJtypClassId
ON JavaTypeImplementedInterface (jtiiJtypClassId)
GO

CREATE NONCLUSTERED INDEX IDX_JavaTypeImplementedInterface_jtiiJtypInterfaceId
ON JavaTypeImplementedInterface (jtiiJtypInterfaceId)
GO

PRINT 'Issue #39: Creating all required indexes on JavaType table - DONE'