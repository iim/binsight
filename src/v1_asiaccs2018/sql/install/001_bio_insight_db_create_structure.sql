CREATE TABLE StringValue
(
  strUId INT IDENTITY(1,1),
  strValue NVARCHAR(MAX) NOT NULL,
  strHash VARBINARY(16) NOT NULL,
  strType VARCHAR(4) NOT NULL,
  CONSTRAINT PK_StringValue PRIMARY KEY (strUId)
)
GO

CREATE TABLE StringValueSMLN
(
  strUId INT IDENTITY(1,1),
  strValue NVARCHAR(MAX) NOT NULL,
  strHash VARBINARY(16) NOT NULL,
  CONSTRAINT PK_StringValueSMLN PRIMARY KEY (strUId)
)
GO

CREATE TABLE StringValueSRCN
(
  strUId INT IDENTITY(1,1),
  strValue NVARCHAR(MAX) NOT NULL,
  strHash VARBINARY(16) NOT NULL,
  CONSTRAINT PK_StringValueSRCN PRIMARY KEY (strUId)
)
GO

CREATE TABLE StringValuePKGN
(
  strUId INT IDENTITY(1,1),
  strValue NVARCHAR(MAX) NOT NULL,
  strHash VARBINARY(16) NOT NULL,
  CONSTRAINT PK_StringValuePKGN PRIMARY KEY (strUId)
)
GO

CREATE TABLE StringValuePATH
(
  strUId INT IDENTITY(1,1),
  strValue NVARCHAR(MAX) NOT NULL,
  strHash VARBINARY(16) NOT NULL,
  CONSTRAINT PK_StringValuePATH PRIMARY KEY (strUId)
)
GO

CREATE TABLE StringValueMTHD
(
  strUId INT IDENTITY(1,1),
  strValue NVARCHAR(MAX) NOT NULL,
  strHash VARBINARY(16) NOT NULL,
  CONSTRAINT PK_StringValueMTHD PRIMARY KEY (strUId)
)
GO
CREATE TABLE StringValueFILD
(
  strUId INT IDENTITY(1,1),
  strValue NVARCHAR(MAX) NOT NULL,
  strHash VARBINARY(16) NOT NULL,
  CONSTRAINT PK_StringValueFILD PRIMARY KEY (strUId)
)
GO

-- Creates Application category table
CREATE TABLE ApplicationCategory
(
  apcUId INT IDENTITY(1,1),
  apcName NVARCHAR(256) NOT NULL,
  apcDescription NVARCHAR(256) NOT NULL,
  CONSTRAINT PK_ApplicationCategory PRIMARY KEY (apcUId)
)
GO

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

CREATE TABLE DataSetApplicationCategories
(
  dstcatUId INT NOT NULL IDENTITY(1,1),
  dstcatDstDataSetId INT NOT NULL,
  dstcatApcApplicationCategoryId INT NOT NULL,
  dstcatBioCount INT NOT NULL,
  CONSTRAINT PK_DataSetApplicationCategories PRIMARY KEY (dstcatUId)
)

-- Table that contains all actual binary content.
CREATE TABLE BinaryObjectContent
(
  bocUId INT IDENTITY(1,1),
  bocHash VARBINARY(20) NOT NULL,
  bocContent VARBINARY(MAX) NULL,
  bocLength INT NOT NULL,
  CONSTRAINT PK_BinaryObjectContent PRIMARY KEY (bocUId)
)
GO

CREATE TABLE BinaryObject
(
  bioUId INT IDENTITY(1,1),
  bioDstcatDataSetApplicationCategoryId INT NOT NULL,
  bioBocContentId INT NOT NULL,
  bioBioParentApkId INT NOT NULL,
  bioBopPathId INT NOT NULL,
  bioHash VARBINARY(20) NOT NULL,
  bioFileName NVARCHAR(256) NOT NULL,
  bioRankInCategory INT NOT NULL,
  bioIsRoot INT NOT NULL,
  bioProcessingStage INT NOT NULL,
  CONSTRAINT PK_BinaryObject PRIMARY KEY (bioUId)
)

CREATE TABLE BinaryObjectPath
(
  bopUId INT IDENTITY(1,1),
  bopBopParentId INT NOT NULL,
  bopName NVARCHAR(400) NOT NULL,
  bopParentPath NVARCHAR(2048) NOT NULL,
  CONSTRAINT PK_BinaryObjectPath PRIMARY KEY (bopUId)
)
GO

-- This table stores Processed (i.e., our) version of java type name
CREATE TABLE JavaType
(
  jtypUId INT IDENTITY(1,1),
  jtypBioParentApkId INT NOT NULL,
  jtypBocParentContentId INT NOT NULL,
  jtypAccessControl INT NOT NULL,
  jtypStrPackageNameId INT NOT NULL,
  jtypStrSmaliFullNameId INT NOT NULL,
  jtypStrFileNameId INT NOT NULL,
  jtypStrPathId INT NOT NULL,
  jtypIsClass BIT NOT NULL,
  jtypIsInterface BIT NOT NULL,
  jtypIsFinal BIT NOT NULL,
  jtypIsEnum BIT NOT NULL,
  jtypIsAbstract BIT NOT NULL,
  jtypIsAnnotation BIT NOT NULL,
  jtypIsStatic BIT NOT NULL,
  jtypIsReferenceOnly BIT NOT NULL,
  jtypJtypOuterClassId INT NOT NULL,
  jtypJtypSuperClassId INT NOT NULL,
  jtypDbgSourceNotFound BIT NOT NULL,
  CONSTRAINT PK_JavaType PRIMARY KEY (jtypUId)
)
GO

-- This table stores links between a class and an implemented interface 
CREATE TABLE JavaTypeImplementedInterface
(
  jtiiUId INT NOT NULL IDENTITY(1,1),
  jtiiJtypClassId INT NOT NULL,
  jtiiJtypInterfaceId INT NOT NULL,
  CONSTRAINT PK_JavaTypeImplementedInterface PRIMARY KEY (jtiiUId)
)
GO

CREATE TABLE JavaTypeMethod
(
  jtmUId                  INT IDENTITY(1, 1),
  jtmJtypInTypeId         INT NOT NULL,
  jtmStrSmaliNameId       INT NOT NULL,
  jtmJtypReturnTypeId     INT NOT NULL,
  jtmAccessControl        INT NOT NULL,
  jtmIsAbstract           BIT NOT NULL,
  jtmIsConstructor        BIT NOT NULL,
  jtmIsStatic             BIT NOT NULL,
  jtmSourceCodeIndexBeg   INT NOT NULL,
  jtmSourceCodeIndexEnd   INT NOT NULL
  CONSTRAINT PK_JavaTypeMethod PRIMARY KEY (jtmUId)
)
GO

CREATE TABLE JavaTypeField
(
  jtfUId                  INT IDENTITY(1, 1),
  jtfJtypInTypeId         INT NOT NULL,
  jtfStrSmaliNameId       INT NOT NULL,
  jtfJtypOfTypeId         INT NOT NULL,
  jtfAccessControl        INT NOT NULL,
  jtfIsArray              BIT NOT NULL,
  jtfIsStatic             BIT NOT NULL,
  jtfIsFinal              BIT NOT NULL,
  jtfIsSynthetic          BIT NOT NULL,
  jtfIsEnum               BIT NOT NULL,
  jtfSourceCodeIndex      INT NOT NULL,
  CONSTRAINT PK_JavaTypeField PRIMARY KEY (jtfUId)
)
GO

CREATE TABLE JavaTypeUsedInType
(
  jtuUId  INT IDENTITY(1, 1),
  jtuJtmDestinationMethodId INT NOT NULL,
  jtuStrDestinationMethodSmaliNameId INT NOT NULL,
  jtuJtfDestinationFieldId INT NOT NULL,
  jtuStrDestinationFieldSmaliNameId INT NOT NULL,
  jtuJtmSourceMethodId INT NOT NULL,
  jtuSourceLineIndex INT NOT NULL,
  jtuSourceWithinLineIndex INT NOT NULL,
  jtuIsReturnType BIT NOT NULL,
  jtuIsParameter BIT NOT NULL,
  jtuIsGetFieldAccessor BIT NOT NULL,
  jtuIsPutFieldAccessor BIT NOT NULL,
  CONSTRAINT PK_JavaTypeUsedInType PRIMARY KEY (jtuUId)
)
GO

CREATE TABLE Library
(
  libUId INT IDENTITY(1, 1),
  libStrPackageNameId INT NOT NULL,
  libName NVARCHAR(256) NOT NULL,
  libUrl NVARCHAR(256) NOT NULL,
  libDescription NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_Library PRIMARY KEY (libUId)
)
GO

CREATE TABLE LibraryAliases
(
  lalUId INT IDENTITY(1, 1),
  lalLibLibraryId INT NOT NULL,
  lalStrPackageNameId INT NOT NULL,
  CONSTRAINT PK_LibraryAliases PRIMARY KEY (lalUId)
)
GO

CREATE TABLE LibraryPropertyTypes
(
  lptUId INT IDENTITY(1, 1),
  lptName NVARCHAR(256) NOT NULL,
  lptDescription NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_LibraryPropertyTypes PRIMARY KEY (lptUId)
)
GO

CREATE TABLE LibraryProperties
(
  lprUId INT IDENTITY(1, 1),
  lprLibLibraryId INT NOT NULL,
  lprLptPropertyTypeId INT NOT NULL,
  lprStrValue NVARCHAR(MAX) NOT NULL,
  lprIntValue INT NOT NULL,
  lprBoolValue BIT NOT NULL,
  CONSTRAINT PK_LibraryProperties PRIMARY KEY (lprUId)
)
GO
