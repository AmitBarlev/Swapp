using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Swap.Object.Tools
{
    public static class FormatTools
    {
        public static string ComputePassword(string input)
        {
            using (SHA512 SHA512Tool = SHA512.Create())
            {
                byte[] bytesOfInput = Encoding.ASCII.GetBytes(input);
                var hash = SHA512Tool.ComputeHash(bytesOfInput);
                return Convert.ToBase64String(hash);
            }
        }

        public static string ComputePictureName(string base64)
        {
            byte[] buffer = Convert.FromBase64String(base64);
            return HexDigest(buffer);
        }

        public static string HexDigest(string textToDigest) => HexDigest(Encoding.UTF8.GetBytes(textToDigest));

        public static string HexDigest(byte[] buffer)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                buffer = sha1.ComputeHash(buffer);
            }

            StringBuilder hexDigseted = new StringBuilder();

            foreach (byte currentByte in buffer)
            {
                hexDigseted.AppendFormat("{0:x2}", currentByte);
            }

            return hexDigseted.ToString();
        }
        

        public static Dictionary<string,string> ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
    }
}
