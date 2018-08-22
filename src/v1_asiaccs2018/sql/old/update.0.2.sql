ALTER TABLE BinaryObject ADD bioProcessingStage INT NULL
GO

UPDATE BinaryObject SET bioProcessingStage = 0
GO

ALTER TABLE BinaryObject ALTER COLUMN bioProcessingStage INT NOT NULL
GO

CREATE TABLE BinaryObjectPath
(
  bopUId INT IDENTITY(1,1) PRIMARY KEY,
  bopBopParentId INT NOT NULL,
  bopName NVARCHAR(400) NOT NULL,
  bopParentPath NVARCHAR(2048) NOT NULL,
)
GO

CREATE NONCLUSTERED INDEX IDX_bopName_bopBopParentId ON BinaryObjectPath (bopBopParentId, bopName)
GO

CREATE NONCLUSTERED INDEX IDX_bopBopParentId ON BinaryObjectPath (bopBopParentId)
GO

SET IDENTITY_INSERT BinaryObjectPath ON
INSERT BinaryObjectPath(bopUId, bopBopParentId, bopName, bopParentPath) VALUES (0, 0, '', '')
SET IDENTITY_INSERT BinaryObjectPath OFF
GO

ALTER TABLE BinaryObjectPath ADD CONSTRAINT SK_bopBopParentId
FOREIGN KEY (bopBopParentId) REFERENCES BinaryObjectPath(bopUId)

ALTER TABLE BinaryObject ADD bioBopPathId INT NULL
GO
UPDATE BinaryObject SET bioBopPathId = 0 WHERE 1=1
GO

ALTER TABLE BinaryObject ADD CONSTRAINT FK_bioBopPathId
FOREIGN KEY (bioBopPathId) REFERENCES BinaryObjectPath(bopUId)

ALTER TABLE BinaryObject ALTER COLUMN bioBopPathId INT NOT NULL
GO

SET IDENTITY_INSERT BinaryObjectContent ON
INSERT BinaryObjectContent(bocUId, bocHash, bocContent, bocLength) VALUES (0, '', 0x, 0)
SET IDENTITY_INSERT BinaryObjectContent OFF
GO

ALTER TABLE BinaryObject ADD bioBioParentApkId INT NULL
GO

SET IDENTITY_INSERT BinaryObject ON
INSERT BinaryObject(bioUId, bioBocContentId, bioApcCategoryId, bioHash, bioFileName, bioRankInCategory, bioIsRoot, bioProcessingStage, bioBopPathId, bioBioParentApkId) 
VALUES (0, 0, 1, '', '', 0, 0, 0, 0, 0)
SET IDENTITY_INSERT BinaryObject OFF
GO

UPDATE BinaryObject SET bioBioParentApkId = 0
GO

ALTER TABLE BinaryObject ALTER COLUMN bioBioParentApkId INT NOT NULL
GO

ALTER TABLE BinaryObject ADD CONSTRAINT SK_bioBioParentApkId
FOREIGN KEY (bioBioParentApkId) REFERENCES BinaryObject(bioUId)
