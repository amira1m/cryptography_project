using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class RepeatingkeyVigenere : ICryptographicTechnique<string, string>
    {
        static string letters = "abcdefghijklmnopqrstuvwxyz";
        //Dictionary<Tuple<char, char>, char> charsTable = GenerateCharsTable();

        public string Analyse(string plainText, string cipherText)
        {
            // throw new NotImplementedException();

            string repeatedKey = "";
            plainText = plainText.ToLower();
            cipherText = cipherText.ToLower();

            for (int i = 0; i < cipherText.Length; i++)
                repeatedKey += letters[(letters.IndexOf(cipherText[i]) - letters.IndexOf(plainText[i]) + 26) % 26];

            //Console.WriteLine("repeatedKey: " + repeatedKey);

            string recoveredKey = DetectRepeatedKey(repeatedKey);

            //Console.WriteLine("recoveredKey: " + recoveredKey);

            return recoveredKey;

        }

        public string Decrypt(string cipherText, string key)
        {
            //            throw new NotImplementedException();

            string plainText = "";
            cipherText = cipherText.ToLower();
            key = key.ToLower();
            string repeatedKey = RepeatKey(key, cipherText.Length);

            for (int i = 0; i < cipherText.Length; i++)
                plainText += letters[(letters.IndexOf(cipherText[i]) - letters.IndexOf(repeatedKey[i]) + 26) % 26];

            return plainText;
        }

        public string Encrypt(string plainText, string key)
        {
            //throw new NotImplementedException();
            string cipherText = "";
            plainText = plainText.ToLower();
            key = key.ToLower();
            string repeatedKey = RepeatKey(key, plainText.Length);

            //Console.WriteLine("repeatedKey: "+ repeatedKey);
            for (int i = 0; i < plainText.Length; i++)
            {
                //cipherText += charsTable[new Tuple<char, char>(plainText[i], repeatedKey[i])];
                cipherText += letters[(letters.IndexOf(repeatedKey[i]) + letters.IndexOf(plainText[i])) % 26];
            }
            //Console.WriteLine("cipherText: " + cipherText);

            return cipherText;
        }

        static public Dictionary<Tuple<char, char>, char> GenerateCharsTable()
        {
            Dictionary<Tuple<char, char>, char> charsTable = new Dictionary<Tuple<char, char>, char>();

            char row, col;

            // Filling the table
            for (int i = 0; i < 26; i++)
            {
                row = letters[i];
                for (int j = 0; j < 26; j++)
                {
                    col = letters[j];
                    charsTable[new Tuple<char, char>(row, col)] = letters[(j+i)%26];
                }
            }

            return charsTable;

        }

        public string RepeatKey(string str, int targetLength)
        {
            int strLength = str.Length;
            int repetitions = targetLength / strLength;
            int remainder = targetLength % strLength;

            string repeatedString = "";

            for (int i = 0; i < repetitions; i++)
            {
                repeatedString += str;
            }

            repeatedString += str.Substring(0, remainder);

            return repeatedString;
        }

        public string DetectRepeatedKey(string str)
        {
            int length = str.Length;

            for (int i = 1; i <= length / 2; i++)
            {
                string pattern = str.Substring(0, i);
                int patternLength = pattern.Length;
                bool isRepeated = true;

                for (int j = i; j < length; j += patternLength)
                {
                    if (j + patternLength <= length && str.Substring(j, patternLength) != pattern)
                    {
                        isRepeated = false;
                        break;
                    }
                }

                if (isRepeated)
                {
                    return pattern;
                }
            }

            return null;
        }
    }
}

