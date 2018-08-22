using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Enums;
using APKInsight.Models;
using APKInsight.Models.DataBase;
using APKInsight.Queries;
using CsnowFramework;
using CsnowFramework.Enum;

namespace APKInsight.Globals
{
    /// <summary>
    /// Helper class that allows to resolve path when needed.
    /// Thread-safe implementation.
    /// </summary>
    internal static class PathResolver
    {
        private static Dictionary<int, BinaryObjectPath> _pathsCollection = null;
        private static Dictionary<int, StringValue> _packageNameCollection = null;
        private static Dictionary<int, StringValue> _filenameCollection = null;
        private static Dictionary<int, StringValue> _javatypeSmalinameCollection = null;
        private static Dictionary<int, LibraryAliases> _libraryAliaseses = null;

        private static Dictionary<string, int> _libraryNameToId = null;
        private static readonly  object _loadingLock = new object(); 

        public static void LoadAll()
        {
            lock (_loadingLock)
            {
                LoadPaths();
                LoadPackageName();
                LoadFilename();
                LoadSmaliName();
                LoadLibraryAliases();
            }
        }

        private static void LoadLibraryAliases()
        {
            if (_libraryAliaseses == null)
            {
                var query = new QueryLibrary();
                var libraryAliases = query.SelectAllLibraryAliases();
                _libraryAliaseses = new Dictionary<int, LibraryAliases>();
                _libraryNameToId = new Dictionary<string, int>();
                foreach (var libraryAlias in libraryAliases)
                {
                    _libraryAliaseses.Add(libraryAlias.PackageNameId.Value, libraryAlias);
                    _libraryNameToId.Add(GetPackageName(libraryAlias.PackageNameId.Value).Value, libraryAlias.PackageNameId.Value);
                }
            }
        }

        public static void LoadPaths()
        {
            if (_pathsCollection == null)
            {
                var query = new QueryBinaryObjectPath();
                var pathSegments = query.SelectBinaryObjectPath(null, null, null);
                _pathsCollection = new Dictionary<int, BinaryObjectPath>();
                foreach (var binaryObjectPath in pathSegments)
                {
                    _pathsCollection.Add(binaryObjectPath.UId.Value, binaryObjectPath);
                }
            }
        }

        public static void LoadPackageName()
        {
            if (_packageNameCollection == null)
            {
                var query = new QueryStringValue();
                var packageNames =
                    query.SelectStringValuesOfType(StringValueType.JavaPackageName);
                _packageNameCollection = new Dictionary<int, StringValue>();
                foreach (var packageName in packageNames)
                {
                    _packageNameCollection.Add(packageName.UId.Value, packageName);
                }
            }
        }

        public static void LoadSmaliName()
        {
            if (_javatypeSmalinameCollection == null)
            {
                var query = new QueryStringValue();
                var smaliNames =
                    query.SelectStringValuesOfType(StringValueType.JavaTypeSmaliFullName);
                _javatypeSmalinameCollection = new Dictionary<int, StringValue>();
                foreach (var smaliName in smaliNames)
                {
                    _javatypeSmalinameCollection.Add(smaliName.UId.Value, smaliName);
                }
            }
        }
        public static void LoadFilename()
        {
            if (_filenameCollection == null)
            {
                var query = new QueryStringValue();
                var fileNames = query.SelectStringValuesOfType(StringValueType.JavaTypeSourceFileName);
                _filenameCollection = new Dictionary<int, StringValue>();
                foreach (var filename in fileNames)
                {
                    _filenameCollection.Add(filename.UId.Value, filename);
                }
            }
        }

        public static BinaryObjectPath GetPath(int index)
        {
            return _pathsCollection[index];
        }

        public static StringValue GetPackageName(int index)
        {
            return _packageNameCollection[index];
        }

        public static StringValue GetJavaTypeSmaliName(int index)
        {
            return _javatypeSmalinameCollection[index];
        }

        public static StringValue GetFileName(int index)
        {
            return _filenameCollection[index];
        }

        public static LibraryAliases GetLibraryAlias(int id)
        {
            return _libraryAliaseses.ContainsKey(id) ? _libraryAliaseses[id] : null;
        }

        public static LibraryAliases GetLibraryAliasByName(string packageName)
        {
            if (!_libraryNameToId.ContainsKey(packageName))
                return null;
            return GetLibraryAlias(_libraryNameToId[packageName]);
        }
    }
}
