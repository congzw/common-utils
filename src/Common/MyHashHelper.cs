using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class MyHashHelper
    {
        public string ComputeHash(string input, HashAlgorithm hashProvider, bool lowerCase)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (hashProvider == null)
            {
                throw new ArgumentNullException(nameof(hashProvider));
            }

            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = hashProvider.ComputeHash(inputBytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            if (lowerCase)
            {
                return hash.ToLower();
            }
            return hash;
        }

        public static MyHashHelper Instance = new MyHashHelper();
    }

    public static class CryptographyExtensions
    {
        public static string GetHashAsGitBlob(this string input)
        {
            //ref => http://alblue.bandlem.com/2011/08/git-tip-of-week-objects.html

            //比如提交一个空文件，Git计算出来的Hash值是e69de29bb2d1d6434b8b29ae775ad8c2e48c5391，实际计算的内容是"blob 0\0"。

            //test in git bash:
            //$ echo -ne "" | git hash-object --stdin
            //e69de29bb2d1d6434b8b29ae775ad8c2e48c5391

            //Note that the \0 is the escape code for the NUL character;
            //the -e to echo stipulates it should obey the escape.
            //If it is interpreting the \0 as two characters, the \ and the 0.

            //C#中以同样的方法计算Hash值
            var computedContent = string.Format("blob {0}\0{1}", input.Length, input);
            return computedContent.GetHashSha1();
        }

        /// <summary>
        /// 	Calculates the MD5 hash for the given string.
        /// </summary>
        /// <returns>A 32 char long MD5 hash.</returns>
        public static string GetHashMd5(this string input)
        {
            return ComputeHash(input, new MD5CryptoServiceProvider());
        }

        /// <summary>
        /// 	Calculates the SHA-1 hash for the given string.
        /// </summary>
        /// <returns>A 40 char long SHA-1 hash.</returns>
        public static string GetHashSha1(this string input)
        {
            return ComputeHash(input, new SHA1Managed());
        }

        /// <summary>
        /// 	Calculates the SHA-256 hash for the given string.
        /// </summary>
        /// <returns>A 64 char long SHA-256 hash.</returns>
        public static string GetHashSha256(this string input)
        {
            return ComputeHash(input, new SHA256Managed());
        }

        /// <summary>
        /// 	Calculates the SHA-384 hash for the given string.
        /// </summary>
        /// <returns>A 96 char long SHA-384 hash.</returns>
        public static string GetHashSha384(this string input)
        {
            return ComputeHash(input, new SHA384Managed());
        }

        /// <summary>
        /// 	Calculates the SHA-512 hash for the given string.
        /// </summary>
        /// <returns>A 128 char long SHA-512 hash.</returns>
        public static string GetHashSha512(this string input)
        {
            return ComputeHash(input, new SHA512Managed());
        }

        private static string ComputeHash(string input, HashAlgorithm hashProvider, bool lowerCase = true)
        {
            return MyHashHelper.Instance.ComputeHash(input, hashProvider, lowerCase);
        }
    }


    #region GuidHashMap

    public class GuidHashMap
    {
        public GuidHashMap()
        {
            Items = new ConcurrentDictionary<string, GuidHash>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, GuidHash> Items { get; set; }

        public GuidHash TryFindByShortHash(string shortHash)
        {
            var theOne = Items.Values.OrderBy(x => x.CreateAt).FirstOrDefault(x => x.Hash.StartsWith(shortHash, StringComparison.OrdinalIgnoreCase));
            return theOne;
        }

        public GuidHash[] TryFindAllByShortHash(string shortHash)
        {
            var all = Items.Values.OrderBy(x => x.CreateAt).Where(x => x.Hash.StartsWith(shortHash, StringComparison.OrdinalIgnoreCase)).ToArray();
            return all;
        }

        public GuidHash TryFindByHash(string hash)
        {
            var theOne = Items.Values.FirstOrDefault(x => x.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase));
            return theOne;
        }

        public GuidHashMap Add(Guid id, DateTime? createAt = null)
        {
            var guidHash = GuidHash.CreateGuidHash(id, createAt);
            Items.Add(guidHash.Hash, guidHash);
            return this;
        }

        public static GuidHashMap Instance = new GuidHashMap();
    }

    public class GuidHash
    {
        public GuidHash()
        {
            CreateAt = DateTime.Now;
        }
        public Guid Id { get; set; }
        public string Hash { get; set; }
        public DateTime CreateAt { get; set; }

        public static GuidHash CreateGuidHash(Guid id, DateTime? createAt = null)
        {
            //N   32 digits:
            //00000000000000000000000000000000
            //D   32 digits separated by hyphens:
            //00000000 - 0000 - 0000 - 0000 - 000000000000
            //B   32 digits separated by hyphens, enclosed in braces:
            //{ 00000000 - 0000 - 0000 - 0000 - 000000000000}
            //P   32 digits separated by hyphens, enclosed in parentheses:
            //(00000000 - 0000 - 0000 - 0000 - 000000000000)
            //X Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces:
            //{ 0x00000000,0x0000,0x0000,{ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00} }
            var guidHash = new GuidHash();
            guidHash.Id = id;
            guidHash.Hash = id.ToString("N").GetHashSha1();
            if (createAt != null)
            {
                guidHash.CreateAt = createAt.Value;
            }
            return guidHash;
        }
    }

    #endregion
}
