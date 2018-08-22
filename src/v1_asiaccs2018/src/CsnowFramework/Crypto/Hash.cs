using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace CsnowFramework.Crypto
{
    /// <summary>
    /// The main class that allows hashing file or byte arrays
    /// </summary>
    public class Hash
    {
        private const int cBufferSize = 1024 * 1024; // 1MB buffer

        // Private variables
        private byte[] _buffer = null;
        private HashAlgorithm _hashAlgo = null;

        public byte[] ComputeHash(string value)
        {
            var buffer = Encoding.UTF8.GetBytes(value);
            _hashAlgo.Initialize();
            _hashAlgo.TransformFinalBlock(buffer, 0, buffer.Length);
            return _hashAlgo.Hash;
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            _hashAlgo.Initialize();
            _hashAlgo.TransformFinalBlock(_buffer, 0, _buffer.Length);
            return _hashAlgo.Hash;
        }


        #region Public Static Functions

        /// <summary>
        /// Hash a file with MD5.
        /// </summary>
        /// <param name="filename">Filename to hash</param>
        /// <returns>String value of the hash function</returns>
        public static string HashMD5(string filename)
        {
            Hash hash = new Hash("md5");
            hash.AddBytesFromFile(filename);
            return hash.HashValue;
        }

        /// <summary>
        /// Hash a file with SHA512.
        /// </summary>
        /// <param name="filename">Filename to hash</param>
        /// <returns>String value of the hash function</returns>
        public static string HashSHA512(string filename)
        {
            Hash hash = new Hash("sha512");
            hash.AddBytesFromFile(filename);
            return hash.HashValue;
        }

        /// <summary>
        /// Hash a byte array.
        /// </summary>
        /// <param name="buffer">Byte array to hash</param>
        /// <returns>String value of the hash function</returns>
        public static string HashMD5(byte[] buffer)
        {
            Hash hash = new Hash("md5");
            hash.AddBytes(buffer);
            return hash.HashValue;
        }

        /// <summary>
        /// Hash a file with SHA512.
        /// </summary>
        /// <param name="filename">Filename to hash</param>
        /// <returns>String value of the hash function</returns>
        public static string HashSHA512(byte[] buffer)
        {
            Hash hash = new Hash("sha512");
            hash.AddBytes(buffer);
            return hash.HashValue;
        }

        public static string HashSha256(byte[] buffer)
        {
            Hash hash = new Hash("sha256");
            hash.AddBytes(buffer);
            return hash.HashValue;
        }

        public static string HashSha1(byte[] buffer)
        {
            Hash hash = new Hash("sha1");
            hash.AddBytes(buffer);
            return hash.HashValue;
        }

        public static byte[] GetHashSha1Bytes(byte[] buffer)
        {
            Hash hash = new Hash("sha1");
            hash.AddBytes(buffer);
            hash._hashAlgo.TransformFinalBlock(hash._buffer, 0, 0);
            return hash._hashAlgo.Hash;
        }

        public static byte[] GetHashMd5Bytes(byte[] buffer)
        {
            Hash hash = new Hash("md5");
            hash.AddBytes(buffer);
            hash._hashAlgo.TransformFinalBlock(hash._buffer, 0, 0);
            return hash._hashAlgo.Hash;
        }

        public static byte[] GetHashSha1Bytes(string filename)
        {
            Hash hash = new Hash("sha1");
            hash.AddBytesFromFile(filename);
            hash._hashAlgo.TransformFinalBlock(hash._buffer, 0, 0);
            return hash._hashAlgo.Hash;
        }

        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="hashname">Name of the hash function to use</param>
        public Hash(string hashname)
        {
            _buffer = new byte[cBufferSize];
            switch (hashname.ToLower())
            {
                case "md5":
                    _hashAlgo = MD5.Create();
                    break;
                case "sha1":
                    _hashAlgo = SHA1.Create();
                    break;
                case "sha256":
                    _hashAlgo = SHA256.Create();
                    break;
                case "sha384":
                    _hashAlgo = SHA384.Create();
                    break;
                case "sha512":
                    _hashAlgo = SHA512.Create();
                    break;
                default:
                    throw new Exception("Undefined hash algorithm");                    
            }
        }

        #endregion


        #region Private functions

        // Add byte to current hash object
        private void AddBytes(byte[] buffer, int begIdx = -1, int endIdx = -1)
            {
            if (begIdx == -1)
                begIdx = 0;
            if (endIdx == -1)
                endIdx = buffer.Length - 1;
            if (endIdx >= buffer.Length)
                throw new Exception("Index our of bounds.");
            _hashAlgo.TransformBlock(buffer, begIdx, endIdx - begIdx + 1, buffer, 0);
        }

        // Read bytes from file and hash them
        private void AddBytesFromFile(string filename)
        {
            int bytesRead;
            using (var fileStream = File.OpenRead(filename))
            {
                while ((bytesRead = fileStream.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    AddBytes(_buffer, 0, bytesRead - 1);
                }
            }
        }

        // Returns the hash value as a string
        private string HashValue
        {
            get
            {
                _hashAlgo.TransformFinalBlock(_buffer, 0, 0);
                return BitConverter.ToString(_hashAlgo.Hash).Replace("-", "");
            }
        }

        #endregion

    }
}
