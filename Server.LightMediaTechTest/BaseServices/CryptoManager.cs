using System.Security.Cryptography;
using System.Text;

namespace Server.LightMediaTechTest.BaseServices
{
    public class CryptoManager
    {
        public (string ProcessedHash, string Salt) ProcessHash(string password, string? salt = null)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                salt = salt ?? Guid.NewGuid().ToString();
                byte[] combinedHash = Encoding.ASCII.GetBytes(salt + password);

                return new(Encoding.ASCII.GetString(sha512.ComputeHash(combinedHash)), salt);

            }
        }

        public bool ValidateHash(string password, string storedHash, string storedSalt)
        {
            var result = ProcessHash(password, storedSalt);

            if (result.ProcessedHash == storedHash)
                return true;
            else
                return false;
        }
    }
}
