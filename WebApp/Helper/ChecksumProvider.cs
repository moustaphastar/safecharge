using System.Security.Cryptography;
using System.Text;

namespace WebApp.Helper
{
    public class ChecksumProvider
    {
        public static string GetChecksumSha256(string text)
        {
            var provider = new SHA256CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] hash = provider.ComputeHash(data);

            string checksum = "";

            foreach (byte b in hash)
            {
                checksum += string.Format("{0:x2}", b);
            }

            return checksum;
        }
    }
}
