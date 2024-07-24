using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class RailFence : ICryptographicTechnique<string, int>
    {
        public int Analyse(string plainText, string cipherText)
        {
            // throw new NotImplementedException();
            cipherText = cipherText.ToLower();
            plainText = plainText.ToLower();

            if (plainText.Length != cipherText.Length) return 0;

            for (int key = 1; key < plainText.Length; key++)
                if (Encrypt(plainText, key).Equals(cipherText)) return key;

            return 0;
        }

        public string Decrypt(string cipherText, int key)
        {
            // throw new NotImplementedException();
            string plainText = "";
            cipherText = cipherText.ToLower();
            double len = (double)cipherText.Length / key;
            int columns = (int)Math.Ceiling(len);
            char[,] arr = new char[key, columns];
            int cnt = 0;
            // Console.WriteLine("cipherText: " + cipherText);

            for (int i = 0; i < key; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (cnt < cipherText.Length)
                    { 
                        arr[i,j] = cipherText[cnt];
                        cnt++;
                    }
                }
            }

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < key; j++)
                {
                    if (arr[j,i] != '\0') 
                        plainText += arr[j, i];
                }
            }

            // Console.WriteLine("plainText: " + plainText);

            return plainText;
        }

        public string Encrypt(string plainText, int key)
        {
            // throw new NotImplementedException();
            string cipherText = "";
            plainText = plainText.ToLower();
            double len = (double)plainText.Length / key;
            int columns = (int)Math.Ceiling(len);
            char[,] arr = new char[key, columns];
            int cnt = 0;
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < key; j++)
                {
                    if (cnt < plainText.Length)
                    {
                        arr[j, i] = plainText[cnt];
                        cnt++;
                    }
                }
            }  
            
            for (int i = 0; i < key; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (arr[i, j] != '\0')
                        cipherText += arr[i, j];
                }
            }
            // Console.WriteLine("cipherText: " + cipherText);

            return cipherText;
        }
    }
}
