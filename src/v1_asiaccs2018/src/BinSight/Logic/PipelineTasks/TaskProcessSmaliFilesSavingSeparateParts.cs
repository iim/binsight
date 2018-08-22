using System;
using APKInsight.Enums;
using APKInsight.Models.Custom;
using APKInsight.Models.DataBase;

namespace APKInsight.Logic.PipelineTasks
{
    internal partial class TaskProcessSmaliFiles
    {
        private static JavaType GetShallowRefJavaType(SmaliParsingDbContext dbContext, string smaliName, string packageName)
        {
            return new JavaType
            {
                ParentApkId = 0,
                ParentContentId = 0,
                AccessControl = 0,
                DbgSourceNotFound = true,
                FileNameId = 0,
                IsAbstract = false,
                IsAnnotation = false,
                IsClass = false,
                IsEnum = false,
                IsFinal = false,
                IsReferenceOnly = true,
                IsStatic = false,
                PackageNameId =
                    StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, packageName,
                        StringValueType.JavaPackageName),
                SmaliFullNameId =
                    StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, smaliName,
                        StringValueType.JavaTypeSmaliFullName),
                OuterClassId = 0,
                IsInterface = false,
                PathId = 0,
                SuperClassId = 0
            };
        }

        private static int SaveSuperClass(SmaliParsingDbContext dbContext, ContentParsing.JavaObjects.JavaType javaType)
        {
            var jt = GetShallowRefJavaType(dbContext, javaType.SuperClass.NameFullSmali, javaType.SuperClass.PackageName);
            return  dbContext.jtypQuery.InsertObjectIntoCache(jt);
        }

        /// <summary>
        /// Saves the chain of outer classes and returns the first outer class Id
        /// </summary>
        /// <param name="bio">Binary Object that we are processing</param>
        /// <param name="javaType">Java type that we are saving</param>
        /// <returns>Id of the first outer class</returns>
        private static int SaveOuterClasses(SmaliParsingDbContext dbContext, BinaryObjectWithContent bio, ContentParsing.JavaObjects.JavaType javaType)
        {
            if (javaType.OuterType == null)
                return 0;

            // First, go up chain and find the first outer class
            var outerType = javaType.OuterType;
            var lastOuterType = javaType.OuterType;
            while (outerType.OuterType != null)
            {
                lastOuterType = outerType.OuterType;
                outerType = outerType.OuterType;
            }

            // Now go backwards and save everything
            int previousOuterTypeId = 0;
            while (lastOuterType != null)
            {
                if (lastOuterType.InnerType != null)
                {
                   
                    // InnerType is null for the final type, we are not saving it here.
                    var jtModel = GetShallowRefJavaType(dbContext, lastOuterType.NameFullSmali, lastOuterType.PackageName);
                    jtModel.ParentApkId = bio.ParentApkId;
                    jtModel.OuterClassId = previousOuterTypeId;
                    previousOuterTypeId = dbContext.jtypQuery.InsertObjectIntoCache(jtModel);
                }
                lastOuterType = lastOuterType.InnerType;
            }

            return previousOuterTypeId;
        }

        private static void SaveImplementedInterfaces(SmaliParsingDbContext dbContext, BinaryObjectWithContent bio, ContentParsing.JavaObjects.JavaType javaType, int javaTypeId)
        {
            foreach (var implementedInterface in javaType.ImplementedInterfaces)
            {
                var interfaceModel = GetShallowRefJavaType(
                            dbContext,
                            implementedInterface.NameFullSmali,
                            implementedInterface.PackageName);
                interfaceModel.ParentApkId = bio.ParentApkId;
                interfaceModel .IsInterface= true;

                interfaceModel.UId = dbContext.jtypQuery.InsertObjectIntoCache(interfaceModel);
                //Save link
                if (interfaceModel.UId.HasValue)
                {
                    var link = new JavaTypeImplementedInterface
                    {
                        ClassId = javaTypeId,
                        InterfaceId = interfaceModel.UId.Value
                    };
                    dbContext.jtypQuery.InsertObjectIntoCache(link);
                }
            }

        }

        // Saves fields of the parsed type.
        private void SaveFields(SmaliParsingDbContext dbContext, BinaryObjectWithContent bio, ContentParsing.JavaObjects.JavaType parsedJavaType, int javaTypeId)
        {
            foreach (var javaTypeField in parsedJavaType.Fields)
            {
                SaveField(dbContext, bio, javaTypeField, javaTypeId);
            }
        }

        // Saves specific fields
        private void SaveField(SmaliParsingDbContext dbContext, BinaryObjectWithContent bio, JavaTypeField javaTypeField, int javaTypeId)
        {
            // Save return type
            var type = GetShallowRefJavaType(
                        dbContext, 
                        javaTypeField.JavaType.NameFullSmali,
                        javaTypeField.JavaType.PackageName);
            type.ParentApkId = bio.ParentApkId;

            type.UId = dbContext.jtypQuery.InsertObjectIntoCache(type);

            var field = new Models.DataBase.JavaTypeField
            {
                InTypeId = javaTypeId,
                AccessControl = (int)javaTypeField.AccessControl,
                SmaliNameId = StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, javaTypeField.SmaliName, StringValueType.JavaTypeFieldSmaliFullName),
                IsArray = javaTypeField.IsArray,
                IsStatic = javaTypeField.IsStatic,
                IsFinal = javaTypeField.IsFinal,
                IsSynthetic = javaTypeField.IsSynthetic,
                IsIsEnum = javaTypeField.IsEnum,
                OfTypeId = type.UId,
                SourceCodeIndex = javaTypeField.SourceLineIndex,
            };

            // Save the method itself
            dbContext.jtypQuery.InsertObjectIntoCache(field);
        }

        // Saves methods of the parsed type.
        private void SaveMethods(SmaliParsingDbContext dbContext, BinaryObjectWithContent bio, ContentParsing.JavaObjects.JavaType parsedJavaType, int javaTypeId)
        {
            foreach (var javaTypeMethod in parsedJavaType.Methods)
            {
                SaveMethod(dbContext, bio, javaTypeMethod, javaTypeId);
            }
        }

        // Saves specific method
        private void SaveMethod(SmaliParsingDbContext dbContext, BinaryObjectWithContent bio, JavaTypeMethod javaTypeMethod, int javaTypeId)
        {
            // Save return type
            var type = GetShallowRefJavaType(dbContext, javaTypeMethod.ReturnType.NameFullSmali,
                javaTypeMethod.ReturnType.PackageName);
            type.ParentApkId = bio.ParentApkId;

            type.UId = dbContext.jtypQuery.InsertObjectIntoCache(type);

            var method = new Models.DataBase.JavaTypeMethod
            {
                InTypeId = javaTypeId,
                AccessControl = (int)javaTypeMethod.AccessControl,
                SmaliNameId = StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, javaTypeMethod.SmaliName, StringValueType.JavaTypeMethodSmaliFullName),
                IsAbstract = javaTypeMethod.IsAbstract,
                IsConstructor = javaTypeMethod.IsConstructor,
                IsStatic = javaTypeMethod.IsStatic,
                ReturnTypeId = type.UId,
                SourceCodeIndexBeg = javaTypeMethod.SourceCodeIndexBeg,
                SourceCodeIndexEnd = javaTypeMethod.SourceCodeIndexEnd
            };

            // Save the method itself
            method.UId = dbContext.jtypQuery.InsertObjectIntoCache(method);

            SaveTypesUsageInMethods(dbContext, bio, javaTypeMethod, method, javaTypeId);
        }

        // We cannot save this before the paper deadline.
        private static void SaveTypesUsageInMethods(
            SmaliParsingDbContext dbContext,
            BinaryObjectWithContent bio,
            JavaTypeMethod method,
            Models.DataBase.JavaTypeMethod methodModel,
            int javaTypeId)
        {
            string methodCodeLine = method.CodeLines[0];
            int startIndex = 0;

            // Save used case of the return type
            // Save return type
            var usedType = GetShallowRefJavaType(dbContext, method.ReturnType.NameFullSmali,
                method.ReturnType.PackageName);
            usedType.ParentApkId = bio.ParentApkId;

            usedType.UId = dbContext.jtypQuery.InsertObjectIntoCache(usedType);

            var useCase = new JavaTypeUsedInType
            {
                DestinationMethodId = 0,
                DestinationMethodSmaliNameId = 0,
                DestinationFieldId = 0,
                DestinationFieldSmaliNameId = 0,
                IsParameter = false,
                IsReturnType = true,
                IsGetFieldAccessor = false,
                IsPutFieldAccessor = false,
                SourceLineIndex = method.SourceCodeIndexBeg,
                SourceMethodId = methodModel.UId.Value,
                SourceWithinLineIndex = methodCodeLine.LastIndexOf(
                    method.ReturnType.NameFullSmali,
                    StringComparison.OrdinalIgnoreCase)
            };
            dbContext.jtuQuery.InsertObjectIntoCache(useCase);

            // Save input parameters
            foreach (var paramTypes in method.InputTypeNames)
            { 
                usedType = GetShallowRefJavaType(dbContext, paramTypes.NameFullSmali, paramTypes.PackageName);
                usedType.ParentApkId = bio.ParentApkId;

                usedType.UId = dbContext.jtypQuery.InsertObjectIntoCache(usedType);

                useCase = new JavaTypeUsedInType
                {
                    DestinationMethodId = 0,
                    DestinationMethodSmaliNameId = 0,
                    DestinationFieldId = 0,
                    DestinationFieldSmaliNameId = 0,
                    IsParameter = true,
                    IsReturnType = false,
                    IsGetFieldAccessor = false,
                    IsPutFieldAccessor = false,
                    SourceLineIndex = method.SourceCodeIndexBeg,
                    SourceMethodId = methodModel.UId.Value,
                    SourceWithinLineIndex = methodCodeLine.IndexOf(
                        paramTypes.NameFullSmali,
                        startIndex,
                        StringComparison.OrdinalIgnoreCase)
                };
                startIndex = useCase.SourceWithinLineIndex.Value + 1;
                dbContext.jtuQuery.InsertObjectIntoCache(useCase);
            }

            // Save all invoked methods within this method
            foreach (var invokedMethod in method.InvokedMethods)
            {
                usedType = GetShallowRefJavaType(dbContext, invokedMethod.NameFullSmali, invokedMethod.PackageName);
                usedType.ParentApkId = bio.ParentApkId;

                usedType.UId = dbContext.jtypQuery.InsertObjectIntoCache(usedType);

                var smaliNameId = StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, invokedMethod.MethodSmaliName, StringValueType.JavaTypeMethodSmaliFullName);
                var methodId = smaliNameId > 0 ? dbContext.jtypQuery.GetMethodId(smaliNameId) : 0;
                useCase = new JavaTypeUsedInType
                {
                    DestinationMethodId = methodId,
                    DestinationMethodSmaliNameId = methodId == 0 ? smaliNameId : 0,
                    DestinationFieldId = 0,
                    DestinationFieldSmaliNameId = 0,
                    IsParameter = false,
                    IsReturnType = false,
                    IsGetFieldAccessor = false,
                    IsPutFieldAccessor = false,
                    SourceLineIndex = method.SourceCodeIndexBeg,
                    SourceMethodId = methodModel.UId.Value,
                    SourceWithinLineIndex = 0
                };
                dbContext.jtuQuery.InsertObjectIntoCache(useCase);
            }

            // Save all accessed fields
            foreach (var fieldAccessor in method.FieldAccessors)
            {
                usedType = GetShallowRefJavaType(dbContext, fieldAccessor.NameFullSmali, fieldAccessor.PackageName);
                usedType.ParentApkId = bio.ParentApkId;
                usedType.UId = dbContext.jtypQuery.InsertObjectIntoCache(usedType);

                var smaliNameId = StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, fieldAccessor.FieldSmaliName, StringValueType.JavaTypeFieldSmaliFullName);
                var fieldId = smaliNameId > 0 ? dbContext.jtypQuery.GetFieldId(smaliNameId) : 0;

                useCase = new JavaTypeUsedInType
                {
                    DestinationMethodId = 0,
                    DestinationMethodSmaliNameId = 0,
                    DestinationFieldId = fieldId,
                    DestinationFieldSmaliNameId = fieldId == 0 ? smaliNameId : 0,
                    IsParameter = false,
                    IsReturnType = false,
                    IsGetFieldAccessor = fieldAccessor.IsGet,
                    IsPutFieldAccessor = fieldAccessor.IsPut,
                    SourceLineIndex = method.SourceCodeIndexBeg,
                    SourceMethodId = methodModel.UId.Value,
                    SourceWithinLineIndex = 0
                };
                dbContext.jtuQuery.InsertObjectIntoCache(useCase);

            }
        }

    }
}
