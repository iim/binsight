SELECT 
  jtypUId,
  JavaTypeSmaliName = SmaliName.strValue,
  JavaTypeShortName = ShortName.strValue,
  JavaTypeShortShortName = ShortShortName.strValue,
  JavaTypeFileNameId = SourceFileName.strUId,
  JavaTypeSourceNotFound = jtypDbgSourceNotFound,
  JavaTypeFileName = SourceFileName.strValue
FROM JavaType
  INNER JOIN StringValue AS SmaliName ON jtypStrSmaliFullNameId = SmaliName.strUId
  INNER JOIN StringValue AS ShortName ON jtypStrShortNameId = ShortName.strUId
  INNER JOIN StringValue AS ShortShortName ON jtypStrShortShortNameId = ShortShortName.strUId
  INNER JOIN StringValue AS SourceFileName ON jtypStrFileNameId = SourceFileName.strUId
WHERE jtypUId = 112656