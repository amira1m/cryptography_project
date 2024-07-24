using System;
using System.Collections.Generic;
using System.Linq;

namespace SecurityLibrary
{
    public class Ceaser : ICryptographicTechnique<string, int>
    {
        string letters = "abcdefghijklmnopqrstuvwxyz";

        public string Encrypt(string plainText, int key)
        {
            //throw new NotImplementedException();
            string cipherText = "";

            foreach (char ch in plainText.ToLower())
            {
                //Console.WriteLine(ch + " " + letters.IndexOf(ch));
                cipherText += letters[(letters.IndexOf(ch) + key) % 26];
            }
            //Console.WriteLine(cipherText);
            return cipherText;
        }

        public string Decrypt(string cipherText, int key)
        {
            //throw new NotImplementedException();
            string plainText = "";

            foreach (char ch in cipherText.ToLower())
            {
                //Console.WriteLine(ch + " " + letters.IndexOf(ch));
                char result = (letters.IndexOf(ch) - key) >= 0 ? letters[(letters.IndexOf(ch) - key) % 26] : letters[((letters.IndexOf(ch) - key) + 26) % 26];
                //plainText += letters[(letters.IndexOf(ch) - key) % 26];
                plainText += result;
            }
            //Console.WriteLine(plainText);
            return plainText;
        }

        public int Analyse(string plainText, string cipherText)
        {
            //throw new NotImplementedException();
            plainText = plainText.ToLower();
            cipherText = cipherText.ToLower();

            //Console.WriteLine(letters.IndexOf(cipherText[1]));
            //Console.WriteLine(letters.IndexOf(plainText[1]));
            int key;
            int index = letters.IndexOf(cipherText[0]) - letters.IndexOf(plainText[0]);
            //Console.WriteLine(index);
            key = index >= 0 ? key = index : key = index + 26;
            //Console.WriteLine(key);
            return key;
        }
    }
}