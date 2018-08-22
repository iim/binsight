  -- BinaryObject
ALTER TABLE BinaryObject ADD CONSTRAINT SK_bioBioParentApkId
FOREIGN KEY (bioBioParentApkId)
REFERENCES BinaryObject(bioUId)
GO

ALTER TABLE BinaryObject ADD CONSTRAINT FK_bioBopPathId
FOREIGN KEY (bioBopPathId)
REFERENCES BinaryObjectPath(bopUId)
GO

ALTER TABLE BinaryObject ADD CONSTRAINT FK_bioBocContentId
FOREIGN KEY (bioBocContentId)
REFERENCES BinaryObjectContent(bocUId)
GO

ALTER TABLE BinaryObject ADD CONSTRAINT FK_bioDstcatDataSetApplicationCategoryId
FOREIGN KEY (bioDstcatDataSetApplicationCategoryId)
REFERENCES DataSetApplicationCategories(dstcatUId)
GO

-- BinaryObjectPath
ALTER TABLE BinaryObjectPath ADD CONSTRAINT SK_bopBopParentId
FOREIGN KEY (bopBopParentId)
REFERENCES BinaryObjectPath(bopUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypBioParentApkId
FOREIGN KEY (jtypBioParentApkId)
REFERENCES BinaryObject(bioUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypBocParentContentId
FOREIGN KEY (jtypBocParentContentId)
REFERENCES BinaryObjectContent(bocUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypStrPackageNameId 
FOREIGN KEY (jtypStrPackageNameId)
REFERENCES StringValuePKGN(strUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypStrSmaliFullNameId
FOREIGN KEY (jtypStrSmaliFullNameId)
REFERENCES StringValueSMLN(strUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypStrPathId
FOREIGN KEY (jtypStrPathId)
REFERENCES StringValuePATH(strUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT FK_jtypStrFileNameId
FOREIGN KEY (jtypStrFileNameId)
REFERENCES StringValueSRCN(strUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT SK_jtypJtypSuperClassId
FOREIGN KEY (jtypJtypSuperClassId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaType ADD CONSTRAINT SK_jtypJtypOuterClassId
FOREIGN KEY (jtypJtypOuterClassId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeImplementedInterface ADD CONSTRAINT FK_jtiiJtypClassId
FOREIGN KEY (jtiiJtypClassId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeImplementedInterface ADD CONSTRAINT FK_jtiiJtypInterfaceId
FOREIGN KEY (jtiiJtypInterfaceId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE DataSetApplicationCategories ADD CONSTRAINT FK_dstcatDstDataSetId
FOREIGN KEY (dstcatDstDataSetId)
REFERENCES DataSet(dstUId)
GO

ALTER TABLE DataSetApplicationCategories ADD CONSTRAINT FK_dstcatApcApplicationCategoryId
FOREIGN KEY (dstcatApcApplicationCategoryId)
REFERENCES ApplicationCategory(apcUId)
GO

ALTER TABLE JavaTypeMethod ADD CONSTRAINT FK_jtmJtypInTypeId
FOREIGN KEY (jtmJtypInTypeId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeMethod ADD CONSTRAINT FK_jtmJtypReturnTypeId
FOREIGN KEY (jtmJtypReturnTypeId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeMethod ADD CONSTRAINT FK_jtmStrSmaliNameId
FOREIGN KEY (jtmStrSmaliNameId)
REFERENCES StringValueMTHD(strUId)
GO

ALTER TABLE JavaTypeField ADD CONSTRAINT FK_jtfJtypInTypeId
FOREIGN KEY (jtfJtypInTypeId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeField ADD CONSTRAINT FK_jtfJtypOfTypeId
FOREIGN KEY (jtfJtypOfTypeId)
REFERENCES JavaType(jtypUId)
GO

ALTER TABLE JavaTypeField ADD CONSTRAINT FK_jtfStrSmaliNameId
FOREIGN KEY (jtfStrSmaliNameId)
REFERENCES StringValueFILD(strUId)
GO

ALTER TABLE JavaTypeUsedInType ADD CONSTRAINT FK_jtuJtmDestinationMethodId
FOREIGN KEY (jtuJtmDestinationMethodId)
REFERENCES JavaTypeMethod(jtmUId)
GO

ALTER TABLE JavaTypeUsedInType ADD CONSTRAINT FK_jtuJtmSourceMethodId
FOREIGN KEY (jtuJtmSourceMethodId)
REFERENCES JavaTypeMethod(jtmUId)
GO

ALTER TABLE JavaTypeUsedInType ADD CONSTRAINT FK_jtuJtfDestinationFieldId
FOREIGN KEY (jtuJtfDestinationFieldId)
REFERENCES JavaTypeField(jtfUId)
GO

ALTER TABLE JavaTypeUsedInType ADD CONSTRAINT FK_jtuStrDestinationMethodSmaliNameId
FOREIGN KEY (jtuStrDestinationMethodSmaliNameId)
REFERENCES StringValueMTHD(strUId)
GO

ALTER TABLE JavaTypeUsedInType ADD CONSTRAINT FK_jtuStrDestinationFieldSmaliNameId
FOREIGN KEY (jtuStrDestinationFieldSmaliNameId)
REFERENCES StringValueFILD(strUId)
GO

-- Library
-- This is a special case of rarely added LBPK
ALTER TABLE Library ADD CONSTRAINT FK_libStrPackageNameId
FOREIGN KEY (libStrPackageNameId)
REFERENCES StringValue(strUId)
GO

-- Library Aliases
ALTER TABLE LibraryAliases ADD CONSTRAINT FK_lalLibLibraryId
FOREIGN KEY (lalLibLibraryId)
REFERENCES Library(libUId)
GO

ALTER TABLE LibraryAliases ADD CONSTRAINT FK_lalStrPackageNameId
FOREIGN KEY (lalStrPackageNameId)
REFERENCES StringValuePKGN(strUId)
GO

-- Library Properties
ALTER TABLE LibraryProperties ADD CONSTRAINT FK_lprLibLibraryId
FOREIGN KEY (lprLibLibraryId)
REFERENCES Library(libUId)
GO

ALTER TABLE LibraryProperties ADD CONSTRAINT FK_lprLptPropertyTypeId
FOREIGN KEY (lprLptPropertyTypeId)
REFERENCES LibraryPropertyTypes(lptUId)
GO
