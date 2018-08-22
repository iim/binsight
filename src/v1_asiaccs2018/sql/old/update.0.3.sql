-- This table stores Smali version of java type name
CREATE TABLE JavaTypeNameFullSmali
(
  jtnfsUId INT IDENTITY(1,1) PRIMARY KEY,
  jtnfsName NVARCHAR(448) NOT NULL,
)
GO

SET IDENTITY_INSERT JavaTypeNameFullSmali ON
GO

INSERT INTO JavaTypeNameFullSmali (jtnfsUId, jtnfsName) VALUES(0,'')
GO

SET IDENTITY_INSERT JavaTypeNameFullSmali OFF
GO

ALTER TABLE JavaTypeNameFullSmali
ADD CONSTRAINT UQ_jtnfsName UNIQUE (jtnfsName)
GO

-- This table stores Processed (i.e., our) version of java type name
CREATE TABLE JavaTypeNameFullProcessed
(
  jtnfpUId INT IDENTITY(1,1) PRIMARY KEY,
  jtnfpName NVARCHAR(448) NOT NULL,
)
GO

ALTER TABLE JavaTypeNameFullProcessed
ADD CONSTRAINT UQ_jtnfpName UNIQUE (jtnfpName)
GO

SET IDENTITY_INSERT JavaTypeNameFullProcessed ON
GO

INSERT INTO JavaTypeNameFullProcessed (jtnfpUId, jtnfpName) VALUES(0,'')
GO

SET IDENTITY_INSERT JavaTypeNameFullProcessed OFF
GO

-- This table file path for a java type object
CREATE TABLE JavaTypePath
(
  jtpUId INT IDENTITY(1,1) PRIMARY KEY,
  jtpName NVARCHAR(448) NOT NULL,
)
GO

CREATE NONCLUSTERED INDEX IDX_jtpName ON JavaTypePath (jtpName)
GO

SET IDENTITY_INSERT JavaTypePath ON
GO

INSERT INTO JavaTypePath (jtpUId, jtpName) VALUES(0,'')
GO

SET IDENTITY_INSERT JavaTypePath OFF
GO

-- This table file path for a java type object
CREATE TABLE JavaTypePackageName
(
  jtpnUId INT IDENTITY(1,1) PRIMARY KEY,
  jtpnName NVARCHAR(448) NOT NULL,
)
GO

CREATE NONCLUSTERED INDEX IDX_jtpnName ON JavaTypePackageName (jtpnName)
GO

SET IDENTITY_INSERT JavaTypePackageName ON
GO

ALTER TABLE JavaTypePackageName
ADD CONSTRAINT UQ_jtpnName UNIQUE (jtpnName)
GO

INSERT INTO JavaTypePackageName (jtpnUId, jtpnName) VALUES(0,'')
GO

SET IDENTITY_INSERT JavaTypePackageName OFF
GO

-- This table file name for a java type object
CREATE TABLE JavaTypeFileName
(
  jtfnUId INT IDENTITY(1,1) PRIMARY KEY,
  jtfnName NVARCHAR(448) NOT NULL,
)
GO

CREATE NONCLUSTERED INDEX IDX_jtfnName ON JavaTypeFileName (jtfnName)
GO

SET IDENTITY_INSERT JavaTypeFileName ON
GO

INSERT INTO JavaTypeFileName (jtfnUId, jtfnName) VALUES(0,'')
GO

SET IDENTITY_INSERT JavaTypeFileName OFF
GO

-- This table stores Processed (i.e., our) version of java type name
CREATE TABLE JavaType
(
  jtypUId INT IDENTITY(1,1) PRIMARY KEY,
  jtypBioParentApkId INT NOT NULL,
  jtypBocParentContentId INT NOT NULL,
  jtypAccessControl INT NOT NULL,
  jtypNameShort NVARCHAR(400) NOT NULL,
  jtypJtpnPackageNameId INT NOT NULL,
  jtypJtnfpProcessedFullNameId INT NOT NULL,
  jtypJtnfsSmaliFullNameId INT NOT NULL,
  jtypJtpPathId INT NOT NULL,
  jtypJtfnFileNameId INT NOT NULL,
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
)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypBioParentApkId
FOREIGN KEY (jtypBioParentApkId)
REFERENCES BinaryObject(bioUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypBocParentContentId
FOREIGN KEY (jtypBocParentContentId)
REFERENCES BinaryObjectContent(bocUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypJtpnPackageNameId 
FOREIGN KEY (jtypJtpnPackageNameId)
REFERENCES JavaTypePackageName(jtpnUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypJtnfpProcessedFullNameId
FOREIGN KEY (jtypJtnfpProcessedFullNameId)
REFERENCES JavaTypeNameFullProcessed(jtnfpUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypJtnfsSmaliFullNameId
FOREIGN KEY (jtypJtnfsSmaliFullNameId)
REFERENCES JavaTypeNameFullSmali(jtnfsUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypJtpPathId
FOREIGN KEY (jtypJtpPathId)
REFERENCES JavaTypePath(jtpUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypJtfnFileNameId
FOREIGN KEY (jtypJtfnFileNameId)
REFERENCES JavaTypeFileName(jtfnUId)
GO

SET IDENTITY_INSERT JavaType ON
GO

INSERT INTO JavaType (
  jtypUId, jtypBioParentApkId, jtypBocParentContentId, jtypAccessControl, jtypNameShort,
  jtypJtpnPackageNameId, jtypJtnfpProcessedFullNameId, jtypJtnfsSmaliFullNameId, jtypJtpPathId, jtypJtfnFileNameId,
  jtypIsClass, jtypIsInterface, jtypIsFinal, jtypIsEnum, jtypIsAbstract,
  jtypIsAnnotation, jtypIsStatic, jtypIsReferenceOnly ,
  jtypJtypOuterClassId, jtypJtypSuperClassId, jtypDbgSourceNotFound)
VALUES(
  0, 0, 0, 0, '',
  0, 0, 0, 0, 0,
  0, 0, 0, 0, 0,
  0, 0, 1, 
  0, 0, 0
)
GO

SET IDENTITY_INSERT JavaType OFF
GO

ALTER TABLE JavaType ADD CONSTRAINT SK_jtypJtypSuperClassId
FOREIGN KEY (jtypJtypSuperClassId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT SK_jtypJtypOuterClassId
FOREIGN KEY (jtypJtypOuterClassId)
REFERENCES JavaType(jtypUId)
GO