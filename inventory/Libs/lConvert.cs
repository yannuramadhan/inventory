using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace inventory.Libs
{
    public class lConvert
    {
        #region ConvertDynamic

        public JArray ConvertDynamicToJArray(List<dynamic> list, String key)
        {
            var jsonObject = new JObject();
            dynamic data = jsonObject;
            data.Lists = new JArray() as dynamic;
            dynamic detail = new JObject();
            foreach (dynamic dr in list)
            {
                detail = new JObject();
                foreach (var pair in dr)
                {
                    if (key.ToString() != "")
                    {
                        detail.Add(key, pair.Value);
                    }
                    else { 
                        detail.Add(pair.Key, pair.Value); 
                    }
                }
                data.Lists.Add(detail);
            }
            return data.Lists;
        }

        public List<string> ConvertDynamicToString(List<dynamic> dynamic)
        {
            var list = new List<string>();
            foreach (dynamic dr in dynamic)
            {
                list.Add(dr.cname);
            }
            return list;
        }

        #endregion

        #region Encript Decript

        public string EncryptString(string encryptString, string EncryptionKey)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Dispose();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public string DecryptString(string cipherText, string EncryptionKey)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Dispose();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public string Encrypt(string str)
        {
            var key = "idxpartners";
            string encrypted = EncryptString(str, key);
            return encrypted;
        }

        public string Decrypt(string str)
        {
            var key = "idxpartners";
            string decrypted = DecryptString(str, key);
            return decrypted;
        }

        #endregion

        #region Encript SHA256

        public string EncryptSha256(string value)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString().ToUpper();
            }
        }

        #endregion

        #region Encript Decript Base64

        public string ToBase64Encode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            byte[] base64EncodedBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(base64EncodedBytes);
        }

        public string ToBase64Decode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            byte[] base64DecodedBytes = Convert.FromBase64String(text);
            return System.Text.Encoding.UTF8.GetString(base64DecodedBytes);
        }

        #endregion
    }
}
