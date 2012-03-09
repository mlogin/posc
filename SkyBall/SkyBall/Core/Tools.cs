using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SkyBall.Core
{
    public class Tools
    {
        public static Random Rnd = new Random();

        private static string m_encryptionKey = "lI1l11Il";

        private static byte[] Encrypt(byte[] plainData, string sKey)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            byte[] encryptedData = desencrypt.TransformFinalBlock(plainData, 0, plainData.Length);
            return encryptedData;
        }

        private static byte[] Decrypt(byte[] encryptedData, string sKey)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desDecrypt = DES.CreateDecryptor();
            byte[] decryptedData = desDecrypt.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return decryptedData;
        }

        public static void SaveObjectToFile(object obj, string path)
        {
            try
            {
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(memStream, obj);
                byte[] encryptedBytes = Encrypt(memStream.ToArray(), m_encryptionKey);
                memStream.Close();
                Stream streamToFile = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                streamToFile.Write(encryptedBytes, 0, encryptedBytes.Length);
                streamToFile.Flush();
                streamToFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static object LoadObjectFromFile(string path)
        {
            try
            {

                Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] encryptedObj = new byte[fileStream.Length];
                fileStream.Read(encryptedObj, 0, (int)encryptedObj.Length);
                MemoryStream memStream = new MemoryStream(Decrypt(encryptedObj, m_encryptionKey));
                BinaryFormatter binFormatter = new BinaryFormatter();
                object decryptedObj = binFormatter.Deserialize(memStream);
                memStream.Close();
                fileStream.Close();
                return decryptedObj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
