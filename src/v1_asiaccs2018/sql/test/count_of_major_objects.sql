SELECT
MethodCnt = (SELECT COUNT (*) FROM StringValueMTHD),
FieldCnt = (SELECT COUNT (*) FROM StringValueFILD),
PathCnt = (SELECT COUNT (*) FROM StringValuePATH),
PackageCnt = (SELECT COUNT (*) FROM StringValuePKGN),
SNamesCnt = (SELECT COUNT (*) FROM StringValueSMLN),
SourceCnt = (SELECT COUNT (*) FROM StringValueSRCN),
JavaTypeCnt = (SELECT COUNT (*) FROM JavaType),
JavaTypeFieldCnt = (SELECT COUNT (*) FROM JavaTypeField),
JavaTypeMethodCnt = (SELECT COUNT (*) FROM JavaTypeMethod),
JavaTypeUseCnt = (SELECT COUNT (*) FROM JavaTypeUsedInType)