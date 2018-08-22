using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices;


namespace CsnowFramework.InputOutput
{

    /// <summary>
    /// Utilities class that helps to deal with files and directories.
    /// </summary>
    public class Utilities
    {

        /// <summary>
        /// Gets all files that matches specific pattern in a directory, including subdirectories.
        /// </summary>
        /// <param name="path">Path to look at</param>
        /// <param name="searchPattern">Optional. Search pattern to match filenames against</param>
        /// <returns>List of filenames</returns>
        public static List<string> GetChildFiles(string path, string extension = "", string searchPattern = "*")
        {
            string[] files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
            var filteredFiles = files.Where(fn => Path.GetFileName(fn).Length == 40 || fn.EndsWith(extension)).ToArray();
            return new List<string>(filteredFiles);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(
            string lpFileName,
            EFileAccess dwDesiredAccess,
            EFileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            ECreationDisposition dwCreationDisposition,
            EFileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        public static void Delete(string fileName)
        {
            string formattedName = @"\\?\" +fileName;
            DeleteFile(formattedName);
        }

        public static bool SaveAsFile(byte[] content, string filepath)
        {
            try
            {
                File.WriteAllBytes(filepath, content);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool CreateDir(string dirpath)
        {
            try
            {
                Directory.CreateDirectory(dirpath);
            }
            catch (System.Exception exp)
            {
                return false;
            }
            return true;
        }

        public static bool RemoveFile(string filepath)
        {
            try
            {
                Delete(filepath);
            }
            catch (System.Exception exp)
            {
                return false;
            }
            return true;
        }

        public static bool RemoveDir(string dirpath)
        {
            try
            {
                var childFiles = GetChildFiles(dirpath);
                bool failed = false;
                foreach (var filename in childFiles)
                {
                    failed = failed | !RemoveFile(filename);
                    if (failed)
                        return false;
                }
                DirectoryInfo dir = new DirectoryInfo(dirpath);
                dir.Delete(true);
            }
            catch (System.Exception exp)
            {
                return false;
            }
            return true;            
        }

        public static byte[] ReadAllBytes(string filepath)
        {
            string formattedName = @"\\?\" +filepath;

            // Create a file with generic write access
            SafeFileHandle fileHandle = CreateFile(formattedName,
                EFileAccess.GenericRead, EFileShare.None, IntPtr.Zero,
                ECreationDisposition.OpenExisting, 0, IntPtr.Zero);

            int lastWin32Error = Marshal.GetLastWin32Error();
            if (fileHandle.IsInvalid)
            {
                throw new System.ComponentModel.Win32Exception(lastWin32Error);
            }
            byte[] content = null;
            using (FileStream fs = new FileStream(fileHandle, FileAccess.Read))
            {
                fs.Seek(0, SeekOrigin.End);
                content = new byte[fs.Position];
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(content, 0, content.Length);
            }

            return content;
        }

        internal static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        internal static int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        internal const int MAX_PATH = 260;

        [StructLayout(LayoutKind.Sequential)]
        internal struct FILETIME
        {
            internal uint dwLowDateTime;
            internal uint dwHighDateTime;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATA
        {
            internal FileAttributes dwFileAttributes;
            internal FILETIME ftCreationTime;
            internal FILETIME ftLastAccessTime;
            internal FILETIME ftLastWriteTime;
            internal int nFileSizeHigh;
            internal int nFileSizeLow;
            internal int dwReserved0;
            internal int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string cFileName;
            // not using this
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternate;
        }

        [Flags]
        public enum EFileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,
        }

        [Flags]
        public enum EFileShare : uint
        {
            None = 0x00000000,
            Read = 0x00000001,
            Write = 0x00000002,
            Delete = 0x00000004,
        }

        public enum ECreationDisposition : uint
        {
            New = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5,
        }

        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

    }

}
