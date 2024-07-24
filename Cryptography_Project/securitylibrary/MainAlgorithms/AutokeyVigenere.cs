using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class AutokeyVigenere : ICryptographicTechnique<string, string>
    {
        static string letters = "abcdefghijklmnopqrstuvwxyz";
        Dictionary<Tuple<char, char>, char> charsTable = GenerateCharsTable();

        public string Analyse(string plainText, string cipherText)
        {

            // throw new NotImplementedException();
            string autoKey = "", recoveredKey, intersectedPart;
            plainText = plainText.ToLower();
            cipherText = cipherText.ToLower();

            for (int i = 0; i < cipherText.Length; i++)
                autoKey += letters[(letters.IndexOf(cipherText[i]) - letters.IndexOf(plainText[i]) + 26) % 26];
            
            intersectedPart = FindIntersectedPart(autoKey, plainText);
            recoveredKey = RemoveIntersectedPart(autoKey, intersectedPart);

            //Console.WriteLine("intersectedPart: " + intersectedPart);
            //Console.WriteLine("recoveredKey: " + recoveredKey);

            return recoveredKey;
        }

        public string Decrypt(string cipherText, string key)
        {

            //            throw new NotImplementedException();

            string plainText = "";
            cipherText = cipherText.ToLower();
            key = key.ToLower();
            int keyLength= key.Length;


            while (key.Length < cipherText.Length)
            {
                if ((key.Length + plainText.Length) <= cipherText.Length)
                {
                    key += plainText;
                    plainText = AutoCipherKey(key, cipherText);
                    //Console.WriteLine("key is: " + key);
                    //Console.WriteLine("plainText is: " + plainText);
                }
                else
                {
                    Console.WriteLine("else:");

                    int remainingLength = cipherText.Length - plainText.Length;
                    int newKeyLength = key.Length - keyLength;

                    key += plainText.Substring(newKeyLength, (plainText.Length - newKeyLength));
                    plainText = AutoCipherKey(key, cipherText);

                    //Console.WriteLine("key is: " + key);
                    //Console.WriteLine("plainText is: " + plainText);
                }
            }

            Console.WriteLine("plain: " + plainText);

            return plainText;
        }

        public string Encrypt(string plainText, string key)
        {

            //     throw new NotImplementedException();
            string cipherText = "";
            plainText = plainText.ToLower();
            key = key.ToLower();
            string completedKey = AutoPlainKey(key, plainText);

            //Console.WriteLine("completedKey: " + completedKey);
            for (int i = 0; i < plainText.Length; i++)
            {
                //cipherText += charsTable[new Tuple<char, char>(plainText[i], completedKey[i])];
                cipherText += letters[(letters.IndexOf(completedKey[i]) + letters.IndexOf(plainText[i])) % 26];
            }
            //Console.WriteLine("cipherText: " + cipherText);

            return cipherText;

        }

        static public Dictionary<Tuple<char, char>, char> GenerateCharsTable()
        {
            Dictionary<Tuple<char, char>, char> charsTable = new Dictionary<Tuple<char, char>, char>();

            char row, col;

            for (int i = 0; i < 26; i++)
            {
                row = letters[i];
                for (int j = 0; j < 26; j++)
                {
                    col = letters[j];
                    charsTable[new Tuple<char, char>(row, col)] = letters[(j + i) % 26];
                }
            }

            return charsTable;

        }

        public string AutoPlainKey(string key, string targetText)
        {
            string completedString = key;
            while (completedString.Length < targetText.Length)
            {
                int remainingLength = targetText.Length - completedString.Length;
                completedString += remainingLength > key.Length ? targetText : targetText.Substring(0, remainingLength);
            }

            return completedString;
        }

        public static string AutoCipherKey(string key, string cipherText, string plainText = "")
        {
            for (int i = 0; i < key.Length && i < cipherText.Length; i++)
                plainText += letters[(letters.IndexOf(cipherText[i]) - letters.IndexOf(key[i]) + 26) % 26];

            return plainText;
        }
        public string FindIntersectedPart(string str1, string str2)
        {
            for (int i = 0; i < str1.Length; i++)
            {
                if (str2.StartsWith(str1.Substring(i)))
                {
                    return str1.Substring(i);
                }
            }
            return null;
        }
        static string RemoveIntersectedPart(string str, string intersectedPart)
        {
            int index = str.IndexOf(intersectedPart);
            if (index != -1)
            {
                return str.Remove(index, intersectedPart.Length);
            }
            return str;
        }
    }
}
