using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class Monoalphabetic : ICryptographicTechnique<string, string>
    {
        string letters = "abcdefghijklmnopqrstuvwxyz";
        public string Analyse(string plainText, string cipherText)
        {
            string key = null;
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            string cipher = cipherText.ToLower();
            char[] emptyKeyArray = "                          ".ToCharArray();
            char[] plainTextArray = plainText.ToCharArray();

            string filledCharacters = null;
            for (int i = 0; i < plainTextArray.Length; i++)
            {
                int index = alphabet.IndexOf(plainTextArray[i]);
                if (index != -1)
                {
                    emptyKeyArray[index] = cipher[i];
                    filledCharacters += cipher[i];
                }
            }

            int charIndex = 0;
            for (int i = 0; i < alphabet.Length && charIndex < 26; i++)
            {
                char currentChar = alphabet[i];
                if (!filledCharacters.Contains(currentChar))
                {
                    while (charIndex < 26 && emptyKeyArray[charIndex] != ' ')
                    {
                        charIndex++;
                    }
                    emptyKeyArray[charIndex++] = currentChar;
                }
            }

            key = new string(emptyKeyArray);

            return key;
        }


        public string Decrypt(string cipherText, string key)
        {
            //throw new NotImplementedException();

            string plainText = "";

            foreach (char ch in cipherText.ToLower())
            {
                plainText += letters[key.IndexOf(ch)];
            }
            return plainText;
        }

        public string Encrypt(string plainText, string key)
        {
            //throw new NotImplementedException();

            string cipherText = "";

            foreach (char ch in plainText.ToLower())
            {
                cipherText += key[letters.IndexOf(ch)];
            }
            return cipherText;
        }







        /// <summary>
        /// Frequency Information:
        /// E   12.51%
        /// T	9.25
        /// A	=
        /// O	7.60
        /// I	7.26
        /// N	7.09
        /// S	6.54
        /// R	6.12
        /// H	5.49
        /// L	4.14
        /// D	3.99
        /// C	3.06
        /// U	2.71
        /// M	2.53
        /// F	2.30
        /// P	2.00
        /// G	1.96
        /// W	1.92
        /// Y	1.73
        /// B	1.54
        /// V	0.99
        /// K	0.67
        /// X	0.19
        /// J	0.16
        /// Q	0.11
        /// Z	0.09
        /// </summary>
        /// <param name="cipher"></param>
        /// <returns>Plain text</returns>
        /// 

        public string AnalyseUsingCharFrequency(string cipher)
        {
            string plaintext = null;
            char[] frequentAlphabet = { 'e', 't', 'a', 'o', 'i', 'n', 's', 'r', 'h', 'l', 'd', 'c', 'u', 'm', 'f', 'p', 'g', 'w', 'y', 'b', 'v', 'k', 'x', 'j', 'q', 'z' };
            char[] ciphertext = cipher.ToLower().ToCharArray();
            int[] frequencyCipher = new int[26];

            foreach (char character in ciphertext)
            {
                if (char.IsLetter(character))
                {
                    frequencyCipher[character - 'a']++;
                }
            }

            int[] sortedIndices = frequencyCipher
                .Select((frequency, index) => new { Frequency = frequency, Index = index })
                .OrderByDescending(pair => pair.Frequency)
                .Select(pair => pair.Index)
                .ToArray();

            char[] mappedAlphabet = new char[26];
            for (int i = 0; i < 26; i++)
            {
                mappedAlphabet[i] = (char)(sortedIndices[i] + 'a');
            }

            Dictionary<char, char> characterMapping = mappedAlphabet.Zip(frequentAlphabet, (mappedChar, freqChar) => new { MappedChar = mappedChar, FreqChar = freqChar })
                                                                    .ToDictionary(pair => pair.MappedChar, pair => pair.FreqChar);

            for (int i = 0; i < ciphertext.Length; i++)
            {
                ciphertext[i] = characterMapping.ContainsKey(ciphertext[i]) ? characterMapping[ciphertext[i]] : ciphertext[i];
            }

            plaintext = new string(ciphertext);

            return plaintext;
        }

    }
}