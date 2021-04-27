using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TemplateTest.Helpers
{
    public class Utils
    {
       
        /// <summary>
        /// This method is used to test whether two password match.
        /// </summary>
        /// <param name="password">The password to be verified.</param>
        /// <param name="passwordHash">The password against the previous one is going to be verified.</param>
        /// <returns>True if they match, false otherwise.</returns>
        public static bool PasswordMatches(string password, byte[] passwordHash, byte[]salt)
        {
            if (string.IsNullOrEmpty(password))
                return false;
            byte[] passHash = MakePasswordHash(password, salt);
            if (passHash.SequenceEqual(passwordHash))
                return true;
            else return false;
        }

        /// <summary>
        /// Function for creating random salt
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// This method is used to create a password hash.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>The hash over the input password string.</returns>
        public static byte[] MakePasswordHash(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
            
            
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8; // four cores
            argon2.Iterations = 4;
            argon2.MemorySize = 1024; // 1 GB

            return argon2.GetBytes(16);

            //using SHA512 shaM = new SHA512Managed();
            //byte[] passHash = shaM.ComputeHash(Encoding.UTF8.GetBytes(password));
            //return passHash;
        }
    }

}
