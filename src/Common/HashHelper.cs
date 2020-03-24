using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class HashHelper
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

        public static HashHelper Instance = new HashHelper();
    }

    public static class CryptographyExtensions
    {
        public static string GetGitHash(this string input)
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

        public static string ComputeHash(string input, HashAlgorithm hashProvider, bool lowerCase = true)
        {
            return HashHelper.Instance.ComputeHash(input, hashProvider, lowerCase);
        }
    }
}
