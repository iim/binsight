PRINT 'Create a New compressed StringValue table'

-- Switch to compressed table
USE ApkTest
GO

-- Create the compressed table
CREATE TABLE StringValueCompressed
(
  strcUId INT IDENTITY(1,1),
  strcStrMinUId INT NOT NULL,
  CONSTRAINT PK_StringValueCompressed PRIMARY KEY (strcUId)
)
GO

-- Allow to insert identity, since we need to save IDs
SET IDENTITY_INSERT StringValueCompressed ON
GO

-- Insert the null table, since it is still needed
INSERT INTO StringValueCompressed(strcUId, strcStrMinUId)
VALUES (0, 0)
GO

-- Fill the compression table
INSERT INTO StringValueCompressed(strcUId, strcStrMinUId)
SELECT 
	strcUId = strUId, 
	strcStrMinUId = 
		(
			SELECT MIN(strUId) 
			FROM [ApkTest].[dbo].StringValue AS MinStringValue 
			WHERE 1=1
				AND MinStringValue.strType = SourceStringValue.strType
				AND MinStringValue.strHash = SourceStringValue.strHash
		)
FROM [ApkTest].[dbo].StringValue AS SourceStringValue
WHERE strUId > 0
GO

SET IDENTITY_INSERT StringValueCompressed OFF
GO

-- Now create a new JavaType and populate it properly
UPDATE [ApkTest].[dbo].JavaType
SET jtypStrFileNameId = TmpTable.newFileNameId
FROM 
(
	SELECT 
		jtypUId,
		newFileNameId = StringValueCompressed.strcStrMinUId
	FROM [ApkTest].[dbo].JavaType
		INNER JOIN StringValueCompressed ON (StringValueCompressed.strcUId = jtypStrFileNameId)
	WHERE StringValueCompressed.strcStrMinUId <> StringValueCompressed.strcUId
) as TmpTable
WHERE [ApkTest].[dbo].JavaType.jtypUId = TmpTable.jtypUId
GO

UPDATE [ApkTest].[dbo].JavaType
SET jtypStrPackageNameId = TmpTable.newFileNameId
FROM 
(
	SELECT 
		jtypUId,
		newFileNameId = StringValueCompressed.strcStrMinUId
	FROM [ApkTest].[dbo].JavaType
		INNER JOIN StringValueCompressed ON (StringValueCompressed.strcUId = jtypStrPackageNameId)
	WHERE StringValueCompressed.strcStrMinUId <> StringValueCompressed.strcUId
) as TmpTable
WHERE [ApkTest].[dbo].JavaType.jtypUId = TmpTable.jtypUId
GO

UPDATE [ApkTest].[dbo].JavaType
SET jtypStrPathId = TmpTable.newFileNameId
FROM 
(
	SELECT 
		jtypUId,
		newFileNameId = StringValueCompressed.strcStrMinUId
	FROM [ApkTest].[dbo].JavaType
		INNER JOIN StringValueCompressed ON (StringValueCompressed.strcUId = jtypStrPathId)
	WHERE StringValueCompressed.strcStrMinUId <> StringValueCompressed.strcUId
) as TmpTable
WHERE [ApkTest].[dbo].JavaType.jtypUId = TmpTable.jtypUId
GO

UPDATE [ApkTest].[dbo].JavaType
SET jtypStrProcessedFullNameId = TmpTable.newFileNameId
FROM 
(
	SELECT 
		jtypUId,
		newFileNameId = StringValueCompressed.strcStrMinUId
	FROM [ApkTest].[dbo].JavaType
		INNER JOIN StringValueCompressed ON (StringValueCompressed.strcUId = jtypStrProcessedFullNameId)
	WHERE StringValueCompressed.strcStrMinUId <> StringValueCompressed.strcUId
) as TmpTable
WHERE [ApkTest].[dbo].JavaType.jtypUId = TmpTable.jtypUId
GO

UPDATE [ApkTest].[dbo].JavaType
SET jtypStrShortNameId = TmpTable.newFileNameId
FROM 
(
	SELECT 
		jtypUId,
		newFileNameId = StringValueCompressed.strcStrMinUId
	FROM [ApkTest].[dbo].JavaType
		INNER JOIN StringValueCompressed ON (StringValueCompressed.strcUId = jtypStrShortNameId)
	WHERE StringValueCompressed.strcStrMinUId <> StringValueCompressed.strcUId
) as TmpTable
WHERE [ApkTest].[dbo].JavaType.jtypUId = TmpTable.jtypUId
GO

UPDATE [ApkTest].[dbo].JavaType
SET jtypStrShortShortNameId = TmpTable.newFileNameId
FROM 
(
	SELECT 
		jtypUId,
		newFileNameId = StringValueCompressed.strcStrMinUId
	FROM [ApkTest].[dbo].JavaType
		INNER JOIN StringValueCompressed ON (StringValueCompressed.strcUId = jtypStrShortShortNameId)
	WHERE StringValueCompressed.strcStrMinUId <> StringValueCompressed.strcUId
) as TmpTable
WHERE [ApkTest].[dbo].JavaType.jtypUId = TmpTable.jtypUId
GO

UPDATE [ApkTest].[dbo].JavaType
SET jtypStrSmaliFullNameId = TmpTable.newFileNameId
FROM 
(
	SELECT 
		jtypUId,
		newFileNameId = StringValueCompressed.strcStrMinUId
	FROM [ApkTest].[dbo].JavaType
		INNER JOIN StringValueCompressed ON (StringValueCompressed.strcUId = jtypStrSmaliFullNameId)
	WHERE StringValueCompressed.strcStrMinUId <> StringValueCompressed.strcUId
) as TmpTable
WHERE [ApkTest].[dbo].JavaType.jtypUId = TmpTable.jtypUId
GO

DROP INDEX IDX_strType_strHash ON StringValue
GO

-- Clearup the String Value
DELETE
FROM [ApkTest].[dbo].StringValue
WHERE strUId IN
(
	SELECT strcUId
	FROM StringValueCompressed
	WHERE strcStrMinUId <> strUId
)
GO

DROP TABLE StringValueCompressed
GO