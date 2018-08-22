using System.Collections.Generic;
using System.Linq;
using System.IO;
using APKInsight.Queries;
using APKInsight.Models;
using APKInsight.Enums;
using APKInsight.Models.Custom;
using APKInsight.Models.DataBase;
using CsnowFramework.Crypto;

namespace APKInsight.Logic
{
    /// <summary>
    /// This is the main logic file that maps an APK file to a directory and uploads it.
    /// </summary>
    class UploadApk
    {
        private readonly int _dataSetId;
        readonly List<ApplicationCategoryWithCount> _appCategories;

        public UploadApk(int dataSetId)
        {
            _dataSetId = dataSetId;
            QueryApplicationCategories apcQuery = new QueryApplicationCategories();
            _appCategories = apcQuery.SelectAllApplicationCategories(dataSetId);
        }

        /// <summary>
        /// Uploads an APK and returns newly created BinaryObject model.
        /// </summary>
        /// <param name="filename">Path to the APK to be uploaded</param>
        /// <returns>Inserted BinaryObject</returns>
        public BinaryObject UploadApkFile(string filename)
        {
            // An apk cannot be less than 1K
            if ((new FileInfo(filename)).Length <= 1024)
            {
                return null;
            }
            BinaryObject result = new BinaryObject();
            result.DataSetApplicationCategoryId = GetDataSetApkCategory(filename);
            result.RankInCategory = GetApkRank(filename);
            result.ContentId = UploadApkContent(filename, ref result);
            result.PathId = 0;
            result.ParentApkId = 0;
            var fn = Path.GetFileName(filename);
            if (fn.Contains("."))
                fn = fn.Substring(fn.IndexOf(".") + 1);
            result.FileName = fn;
            result.IsRoot = 1;
            result.ProcessingStage = (int)BinaryObjectApkProcessingStage.Unprocessed;
            QueryBinaryObject bioQuery = new QueryBinaryObject();
            var existingObject = bioQuery.SelectBinaryObject(result);
            if (existingObject.Count > 0)
            {
                result.UId = existingObject[0].UId;
            }
            else
            {
                bioQuery.AddObject(ref result);
            }
            return result;
        }

        private int UploadApkContent(string filename, ref BinaryObject bio)
        {
            BinaryObjectContent content = new BinaryObjectContent {Hash = Hash.GetHashSha1Bytes(filename)};
            bio.Hash = content.Hash;
            QueryBinaryObjectContent bocQuery = new QueryBinaryObjectContent();
            content.Content = File.ReadAllBytes(filename);
            content.Length = content.Content.Length;
            content.UId = bocQuery.AddObject(content, Path.GetFileName(filename));
            return (int)content.UId;
        }

        private int GetApkRank(string filename)
        {
            string fn = Path.GetFileName(filename);
            int delFrom = fn.IndexOf(".");
            if (delFrom == -1)
                return -1;
            string rankStr = fn.Substring(0, delFrom);
            int rank = -1;
            int.TryParse(rankStr, out rank);
            return rank;
        }

        private int GetDataSetApkCategory(string filename)
        {
            string path = Path.GetDirectoryName(filename);
            string parentDirectoryName = Path.GetFileName(path);
            parentDirectoryName = parentDirectoryName.Replace("APP_", "").ToLower();
            if (_appCategories.AsQueryable().Any(cat => cat.Name.ToLower() == parentDirectoryName))
            {
                return _appCategories.AsQueryable().Where(cat => cat.Name.ToLower() == parentDirectoryName).ElementAt(0).DataSetApplicationCategoryId;
            }
            parentDirectoryName = parentDirectoryName.Replace("_", "");
            if (_appCategories.AsQueryable().Any(cat => cat.Name.ToLower() == parentDirectoryName))
            {
                return _appCategories.AsQueryable().Where(cat => cat.Name.ToLower() == parentDirectoryName).ElementAt(0).DataSetApplicationCategoryId;
            }
            parentDirectoryName = parentDirectoryName.Replace("widgets", "widget");
            if (_appCategories.AsQueryable().Count(cat => cat.Name.ToLower() == parentDirectoryName) > 0)
            {
                return _appCategories.AsQueryable().Where(cat => cat.Name.ToLower() == parentDirectoryName).ElementAt(0).DataSetApplicationCategoryId;
            }
            parentDirectoryName = parentDirectoryName.Replace("booksandreference", "booksandrefs");
            if (_appCategories.AsQueryable().Count(cat => cat.Name.ToLower() == parentDirectoryName) > 0)
            {
                return _appCategories.AsQueryable().Where(cat => cat.Name.ToLower() == parentDirectoryName).ElementAt(0).DataSetApplicationCategoryId;
            }
            if (_appCategories.AsQueryable().Count(cat => cat.Name.ToLower().StartsWith(parentDirectoryName)) > 0)
            {
                return _appCategories.AsQueryable().Where(cat => cat.Name.ToLower().StartsWith(parentDirectoryName)).ElementAt(0).DataSetApplicationCategoryId;
            }
            return _appCategories.AsQueryable().Where(cat => cat.Name.ToLower() == "UNDEFINED".ToLower()).ElementAt(0).DataSetApplicationCategoryId;
        }
    }
}
