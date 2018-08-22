using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace APKInsight.Logic.Analysis.Data
{
    /// <summary>
    /// Holds information about specific use case
    /// </summary>
    public class UseCase
    {
        public ApkInfo ApplicationInfo;
        public string SmaliClassName;
        public string SmaliMethodName;
        public int InClassPos;
        public int InMethodPos;
        public string Filename;
        public string PackageName;
        public string ClassName;
        public string ApiSig;

        public static List<LibraryDefinition> Libraries = new List<LibraryDefinition>();
        public static List<LibraryDefinition> PossibleLibraries = new List<LibraryDefinition>();
        private static HashSet<string> _possibleLibrariesPackageNames = null;
       

        public bool IsClassNameObfuscated
        {
            get
            {
                var className = ClassName;
                if (className.Contains("$"))
                    className = className.Substring(className.LastIndexOf("$", StringComparison.Ordinal) + 1);
                if (className.Length == 1) return true;
                if (className.Length == 2 && char.IsLower(className[0])) return true;
                if (className.Length == 3 && char.IsLower(className[0])) return true;
                return false;
            }
        }

        public bool IsPackageNameFullyObfuscated
        {
            get
            {
                if (PackageName.Length == 0) return true;
                var parts = PackageName.Split('.');
                if (parts.All(p => p.Length == 1)) return true;

                var partsButFirst = parts.Skip(1);
                var firstPart = parts.First();
                if (
                        (firstPart == "com" ) ||
                        (firstPart == "ch") || 
                        (firstPart == "io") || 
                        (firstPart == "jp") || 
                        (firstPart == "org") || 
                        (firstPart == "net"))
                {
                    if (partsButFirst.All(p => p.Length == 1))
                        return true;
                }
                return false;
            }
        }

        public bool IsPackageNamePartiallyObfuscated => !IsPackageNameFullyObfuscated && PackageName.Split('.').Any(p => p.Length == 1);
        public bool IsPackageNameReadable => !IsPackageNameFullyObfuscated && !IsPackageNamePartiallyObfuscated;

        public bool IsCipherUseCaseRule1to6 => IsCipherUseCaseRule1 || IsCipherUseCaseRule2 || IsCipherUseCaseRule3 || IsCipherUseCaseRule4And5 || IsCipherUseCaseRule6;
        public bool IsCipherUseCaseRule1 => ApiSig.Contains("Ljavax/crypto/Cipher;->getInstance(Ljava/lang/String;)Ljavax/crypto/Cipher;");
        public bool IsCipherUseCaseRule2 => ApiSig.Contains("Ljavax/crypto/Cipher;->init(I");
        public bool IsCipherUseCaseRule3 => ApiSig.Contains("Ljavax/crypto/spec/SecretKeySpec;-><init>([BLjava/lang/String;)V");
        public bool IsCipherUseCaseRule4And5 => ApiSig.Contains("Ljavax/crypto/spec/PBEParameterSpec;-><init>") || ApiSig.Contains("Ljavax/crypto/spec/PBEKeySpec;-><init>");

        public bool IsCipherUseCaseRule6 =>
            ApiSig.Contains("Ljava/security/SecureRandom;-><init>([B)") ||
            ApiSig.Contains("Ljava/security/SecureRandom;->setSeed(");
        public bool IsSecRandUseCase => IsCipherUseCaseRule6;
        public bool IsCipherUseCase => IsCipherUseCaseRule1 || IsCipherUseCaseRule2 || IsCipherUseCaseRule3;
        public bool IsPbkdf2UseCase => IsCipherUseCaseRule4And5;

        public bool IsDataEncryptionOrDecryption =>
            ApiSig.Contains("Ljavax/crypto/Cipher;->update(") ||
            ApiSig.Contains("Ljavax/crypto/Cipher;->updateAAD(") ||
            ApiSig.Contains("Ljavax/crypto/Cipher;->doFinal(");

        public bool IsLibrary
        {
            get
            {
                foreach (var libraryDefinition in Libraries)
                {
                    if (PackageName.StartsWith(libraryDefinition.Prefix)) return true;
                    if (!string.IsNullOrWhiteSpace(libraryDefinition.RegExPattern))
                    {
                        var match = Regex.Match(PackageName, libraryDefinition.RegExPattern);
                        if (match.Success) return true;
                    }
                }
                return false;
            }
        }

        public string GetLibraryName
        {
            get
            {
                foreach (var libraryDefinition in Libraries)
                {
                    if (PackageName.StartsWith(libraryDefinition.Prefix)) return libraryDefinition.Prefix;
                    if (!string.IsNullOrWhiteSpace(libraryDefinition.RegExPattern))
                    {
                        var match = Regex.Match(PackageName, libraryDefinition.RegExPattern);
                        if (match.Success) return libraryDefinition.Prefix;
                    }
                }
                throw new Exception("Not a library");
            }
        }

        public bool IsPossibleLibrary
        {
            get
            {
                if (_possibleLibrariesPackageNames == null)
                {
                    _possibleLibrariesPackageNames = new HashSet<string>(PossibleLibraries.Select(p => p.Prefix).ToArray());
                }
                return _possibleLibrariesPackageNames.Contains(PackageName);
            }
        }

        /// <summary>
        /// The white listed libraries from CCS'13 paper
        /// </summary>
        private static string[] _ccs13excludedLibsRegEx =
        {
            "com.scoreloop",
            "com.google.android.vending",
            "com.android.vending",
            "com.urbanairship",
            "com.openfeint",
            "com.google.ads",
            "com.phonegap",
            "vpadn",
            "com.unity3d",
            "jp.co.microad",
            "com.amazonaws",
            "org.apache.james"

        };

        public bool Ccs13Whitelisting
        {
            get
            {
                foreach (var ccs13 in _ccs13excludedLibsRegEx)
                {
                    if (PackageName.StartsWith(ccs13) || PackageName.Contains(ccs13))
                        return true;
                }
                return false;
            }
        }

    }
}
