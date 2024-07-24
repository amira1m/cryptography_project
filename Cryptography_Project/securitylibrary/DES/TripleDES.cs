using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.DES
{
    /// <summary>
    /// If the string starts with 0x.... then it's Hexadecimal not string
    /// </summary>
    public class TripleDES : ICryptographicTechnique<string, List<string>>
    {
        public string Decrypt(string cipherText, List<string> key)
        {
            DES des = new DES();
            string firstDecrypt = des.Decrypt(cipherText, key[0]);
            string secondEncrypt = des.Encrypt(firstDecrypt, key[1]);
            string thirdDecrypt = des.Decrypt(secondEncrypt, key[0]);
            return thirdDecrypt;
        }

        public string Encrypt(string plainText, List<string> key)
        {
            DES des = new DES();
            string firstEncrypt = des.Encrypt(plainText, key[0]);
            string secondDecrypt = des.Decrypt(firstEncrypt, key[1]);
            string thirdEncrypt = des.Encrypt(secondDecrypt, key[0]);
            return thirdEncrypt;
        }

        public List<string> Analyse(string plainText, string cipherText)
        {
            throw new NotSupportedException();
        }

    }
}
