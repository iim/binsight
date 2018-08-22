using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Enums;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Models;
using APKInsight.Models.DataBase;
using APKInsight.Queries;

namespace APKInsight.Logic.PipelineTasks
{
    internal partial class TaskProcessSmaliFiles
    {
        private void PrepareSmaliProcessingStage()
        {
            // Delete all indices
            var query = new QueryStringValue();
            query.DropUniqueIndex();
            query.DropAllIndices();

            var jtypQuery = new QueryJavaType();
            jtypQuery.DropAllIndices();
            jtypQuery.DisableSelfKeys();

            // Previous state for this stage is all unprocessed 
            _maxId = 0;

            var queryBio = new QueryBinaryObject();
            LeftToSchedule = queryBio.SelectRootBiosCount(_dataSetId);
        }

        private void StartJavaTypeProcessingThread()
        {
            var query = new QueryBinaryObject();
            var data = query.SelectNextRootBio(
                        _maxId.Value,
                        _dataSetId);
            var maxId = data.Any() ? data[0].UId : null;
            if (maxId != null)
            {
                _maxId.Value = (int)maxId;
                LeftToSchedule.Value = LeftToSchedule.Value - data.Count;
                ForkThread(data[0]);
                if (LeftToSchedule.Value <= 0)
                    HaveWork = false;
            }
            else
            {
                HaveWork = false;
                var jtypQuery = new QueryJavaType();
                jtypQuery.EnableSelfKeys();
            }
        }

        private void Stage1SaveJavaTypesDefinitions(BinaryObject binaryObject)
        {
            SmaliParser smaliParser = new SmaliParser();
            SmaliParsingDbContext dbContext = new SmaliParsingDbContext
            {
                bioQuery = new QueryBinaryObject(),
                jtypQuery = new QueryJavaType(),
                jtuQuery = new QueryJavaTypeUsedInType(),
                strQuery = new QueryStringValue()
            };
            HashSet<int> processedBinaryContents = new HashSet<int>();

            // Set the current master bio as being in progress
            dbContext.bioQuery.UpdateBinaryObjectProcessState(binaryObject.UId.Value, (int)BinaryObjectApkProcessingStage.SmaliFilesProcessingInProgress);

            // Fetch all internal binaries for that APK
            var binaryObjects =
                dbContext.bioQuery.SelectSmaliFilesToProcess(binaryObject.UId.Value);


            for (int i = 0; i < binaryObjects.Count(); i++)
            {
                var bio = binaryObjects[i];
                if (processedBinaryContents.Contains(bio.ContentId.Value))
                    continue;
                processedBinaryContents.Add(bio.ContentId.Value);

                try
                {

                    // Parse bio object
                    smaliParser.ProcessSmaliFile(bio);
                    var jtModel = ConvertToJavaTypeModel(dbContext, bio, smaliParser.JavaType);

                    // Save super class
                    jtModel.SuperClassId = SaveSuperClass(dbContext, smaliParser.JavaType);

                    // Save Outter class
                    jtModel.OuterClassId = SaveOuterClasses(dbContext, bio, smaliParser.JavaType);

                    // Save the main object
                    jtModel.UId = dbContext.jtypQuery.InsertObjectIntoCache(jtModel);

                    // Save implemented interfaces
                    SaveImplementedInterfaces(dbContext, bio, smaliParser.JavaType, jtModel.UId.Value);

                    // Save fields
                    SaveFields(dbContext, bio, smaliParser.JavaType, jtModel.UId.Value);

                    // Save methods
                    SaveMethods(dbContext, bio, smaliParser.JavaType, jtModel.UId.Value);

                }
                catch (Exception exp)
                {
                }
                if (Cancelled)
                    break;
            }

            dbContext.strQuery.SaveCache();
            dbContext.jtypQuery.SaveCache();
            dbContext.jtuQuery.SaveCache();

            // Update BIO's processed stage.
            dbContext.bioQuery.UpdateBinaryObjectProcessState(binaryObject.UId.Value, (int)BinaryObjectSmaliProcessingStage.Processed);

            RaiseOnTaskThreadItemCompleted();
            RaiseOnTaskThreadCompleted();
        }

        // Converts currently parsed JT model into DB JT model.
        private JavaType ConvertToJavaTypeModel(
                    SmaliParsingDbContext dbContext,
                    BinaryObject bio,
                    ContentParsing.JavaObjects.JavaType parsedJavaType)
        {
            return new JavaType()
            {
                // Parent refs
                ParentApkId = bio.ParentApkId,
                ParentContentId = bio.ContentId,

                // Access control (this should set the value too)
                JavaAccessControl = parsedJavaType.AccessControl,

                // Names and related refs
                PackageNameId = StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, parsedJavaType.PackageName, StringValueType.JavaPackageName),
                SmaliFullNameId = StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, parsedJavaType.NameFullSmali, StringValueType.JavaTypeSmaliFullName),
                FileNameId = StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, parsedJavaType.FileName, StringValueType.JavaTypeSourceFileName),
                PathId = StringValueUtils.SaveStringValueWithNoSearch(dbContext.strQuery, parsedJavaType.FilePath, StringValueType.JavaPath),

                // Flags
                IsStatic = parsedJavaType.IsStatic,
                IsAbstract = parsedJavaType.IsAbstract,
                IsAnnotation = parsedJavaType.IsAnnotation,
                IsClass = parsedJavaType.IsClass,
                IsEnum = parsedJavaType.IsEnum,
                IsFinal = parsedJavaType.IsFinal,
                IsInterface = parsedJavaType.IsInterface,
                IsReferenceOnly = false,

                // Defined in an outer class
                OuterClassId = 0,

                // Super class name
                SuperClassId = 0,

                // Debug flags
                DbgSourceNotFound = parsedJavaType.DbgSourceNotFound
            };
        }

        struct SmaliParsingDbContext
        {
            public QueryBinaryObject bioQuery;
            public QueryJavaType jtypQuery;
            public QueryJavaTypeUsedInType jtuQuery;
            public QueryStringValue strQuery;
        }

    }
}
