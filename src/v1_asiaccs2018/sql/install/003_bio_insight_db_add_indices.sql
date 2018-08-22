CREATE UNIQUE INDEX UQ_bocHash ON BinaryObjectContent (bocHash)
GO

CREATE NONCLUSTERED INDEX IDX_bioHash ON BinaryObject (bioHash)
GO

CREATE UNIQUE INDEX UQ_bopBopParentId_bopName ON BinaryObjectPath (bopBopParentId, bopName)
GO

CREATE UNIQUE INDEX UQ_JavaTypeImplementedInterface
ON JavaTypeImplementedInterface (jtiiJtypClassId, jtiiJtypInterfaceId)
GO

-- JavaTypeMethod table
CREATE NONCLUSTERED INDEX IDX_JavaTypeMethod
ON JavaTypeMethod (jtmJtypInTypeId, jtmStrSmaliNameId)
GO

-- JavaTypeField table
CREATE NONCLUSTERED INDEX IDX_JavaTypeField
ON JavaTypeField (jtfJtypInTypeId, jtfStrSmaliNameId)
GO

-- JavaTypeUsedInType table
CREATE NONCLUSTERED INDEX IDX_JavaTypeUsedInType_Destination
ON JavaTypeUsedInType (jtuJtmDestinationMethodId)
GO
CREATE NONCLUSTERED INDEX IDX_JavaTypeUsedInType_Source
ON JavaTypeUsedInType (jtuJtmSourceMethodId)
GO

-- Library
CREATE NONCLUSTERED INDEX IDX_Library_libStrPackageNameId
ON Library (libStrPackageNameId)
GO

-- Library Aliases
CREATE NONCLUSTERED INDEX IDX_LibraryAliases
ON LibraryAliases (lalLibLibraryId, lalStrPackageNameId)
GO

-- Library properties
CREATE NONCLUSTERED INDEX IDX_LibraryProperties
ON LibraryProperties (lprLibLibraryId, lprLptPropertyTypeId)
GO

-- JavaType
CREATE UNIQUE INDEX UQ_jtypBioParentApkId_jtypStrSmaliFullNameId ON JavaType (jtypBioParentApkId, jtypStrSmaliFullNameId)
GO