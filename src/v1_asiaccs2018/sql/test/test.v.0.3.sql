--SELECT COUNT(*)
--FROM JavaTypePackageName

--SELECT COUNT(*)
--FROM JavaTypeFileName

--SELECT COUNT(*)
--FROM JavaTypePath

SELECT LEN(JavaTypeOuter.jtypNameShort),*
FROM JavaType AS JavaTypeOuter
  INNER JOIN JavaTypeNameFullSmali ON JavaTypeOuter.jtypJtnfsSmaliFullNameId = jtnfsUId
  INNER JOIN JavaType AS JavaTypeConsum ON JavaTypeConsum.jtypJtypOuterClassId = JavaTypeOuter.jtypUId
WHERE 1=1
  AND LEN(JavaTypeOuter.jtypNameShort) = 0 
  AND JavaTypeOuter.jtypUId >0
ORDER BY JavaTypeOuter.jtypNameShort, JavaTypeOuter.jtypUId