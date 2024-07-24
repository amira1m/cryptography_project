using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class Columnar : ICryptographicTechnique<string, List<int>>
    {
        public List<int> Analyse(string plainText, string cipherText)
        {
            cipherText = cipherText.ToLower();
            int col = GetColumnCount(cipherText);
            int row = cipherText.Length / col;
            char[,] plain = FillPlainText(plainText, row, col);
            char[,] cipher = FillCipherText(cipherText, row, col);
            List<int> key = GetKey(plain, cipher, row, col);
            if (key.Count == 0)
            {
                for (int i = 0; i < col + 2; i++)
                {
                    key.Add(0);
                }
            }
            return key;
        }

        private int GetColumnCount(string cipherText)
        {
            int col = 0;
            for (int i = 2; i < 8; i++)
            {
                if (cipherText.Length % i == 0)
                {
                    col = i;
                }
            }
            return col;
        }

        private char[,] FillPlainText(string plainText, int row, int col)
        {
            char[,] plain = new char[row, col];
            int counter = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (counter < plainText.Length)
                    {
                        plain[i, j] = plainText[counter];
                        counter++;
                    }
                }
            }
            return plain;
        }

        private char[,] FillCipherText(string cipherText, int row, int col)
        {
            char[,] cipher = new char[row, col];
            int counter = 0;
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    if (counter < cipherText.Length)
                    {
                        cipher[j, i] = cipherText[counter];
                        counter++;
                    }
                }
            }
            return cipher;
        }

        private List<int> GetKey(char[,] plain, char[,] cipher, int row, int col)
        {
            List<int> key = new List<int>(col);
            int count = 0;
            for (int i = 0; i < col; i++)
            {
                for (int k = 0; k < col; k++)
                {
                    for (int j = 0; j < row; j++)
                    {
                        if (plain[j, i] == cipher[j, k])
                        {
                            count++;
                        }
                        if (count == row)
                        {
                            key.Add(k + 1);
                        }
                    }
                    count = 0;
                }
            }
            return key;
        }

        public string Decrypt(string cipherText, List<int> key)
        {
            // throw new NotImplementedException();


            string plainText = "";
            cipherText = cipherText.ToLower();
            double len = (double)cipherText.Length / key.Count;
            int rows = (int)Math.Ceiling(len);
            char[,] arr = new char[rows, key.Count];
            int cnt = 0;

            for (int i = 0; i < key.Count; i++)
            {
                int currIndex = key.IndexOf(i + 1);
                //Console.WriteLine("curr I: " + i);
                //Console.WriteLine("currIndex: " + currIndex);

                for (int j = 0; j < rows; j++)
                {
                    if (cnt < cipherText.Length)
                    {
                        arr[j, currIndex] = cipherText[cnt];
                        cnt++;
                    }
                    //Console.WriteLine(arr[j, i]);
                }
            }


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < key.Count; j++)
                {
                    //Console.Write(arr[i,j] + "\t");
                    plainText += arr[i, j];
                }
                //Console.WriteLine();
            }

            //Console.WriteLine("plainText: " + plainText);

            return plainText;

        }

        public string Encrypt(string plainText, List<int> key)
        {
            // throw new NotImplementedException();


            string cipherText = "";
            plainText = plainText.ToLower();
            double len = (double)plainText.Length / key.Count;
            int rows = (int)Math.Ceiling(len);
            char[,] arr = new char[rows, key.Count];
            int cnt = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < key.Count; j++)
                {
                    if (cnt < plainText.Length)
                    {
                        arr[i, j] = plainText[cnt];
                        cnt++;
                    }
                    else
                        arr[i, j] = 'x';
                    //Console.WriteLine(arr[i, j]);
                }
            }   
            
            for (int i = 0; i < key.Count; i++)
            {
                int currIndex = key.IndexOf(i + 1);
                //Console.WriteLine("index: " + key.IndexOf(i + 1));
                for (int j = 0; j < rows; j++)
                {
                    cipherText += arr[j, currIndex];

                }
            }

            //Console.WriteLine("cipherText: " + cipherText);

            return cipherText;
        }


    }
}
