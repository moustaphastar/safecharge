using System.Security.Cryptography;
using System.Text;

namespace UI.SafeChargeModel
{
    public class CheckSummer
    {
        public CheckSummer()
        {

        }

        public string GetChecksumSha256(string text)
        {
            var provider = new SHA256CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] hash = provider.ComputeHash(data);

            // Transforms as hexa
            string hexaHash = "";

            foreach (byte b in hash)
            {
                hexaHash += string.Format("{0:x2}", b);
            }

            // Returns MD5 hexa hash
            return hexaHash;
        }
    }
}