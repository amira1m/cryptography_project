using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{

    public class HillCipher : ICryptographicTechnique<string, string>, ICryptographicTechnique<List<int>, List<int>>
    {
        int modValue = 26;

        public List<int> Analyse(List<int> plainText, List<int> cipherText)
        {
            for (int i = 0; i < this.modValue; i++)
            {
                for (int j = 0; j < this.modValue; j++)
                {
                    for (int k = 0; k < this.modValue; k++)
                    {
                        for (int l = 0; l < this.modValue; l++)
                        {
                            List<int> key = new List<int>(new[] {
                                i, j,
                                k, l
                            });

                            if (!Encrypt(plainText, key).SequenceEqual(cipherText))
                            {
                                continue;
                            }

                            return key;
                        }
                    }
                }
            }

            throw new InvalidAnlysisException();
        }
        public string Analyse(string plainText, string cipherText)
        {
            return Analyse3By3Key(plainText, cipherText);
        }

        public List<int> Decrypt(List<int> cipherText, List<int> key)
        {
            int x = (int)Math.Sqrt(key.Count());
            int y = (int)Math.Ceiling((decimal)(cipherText.Count() / x));

            int[,] ct = new int[x, y];

            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    int index = x * j + i;

                    ct[i, j] = index < cipherText.Count()
                        ? cipherText[index]
                        : 23;
                }
            }

            int[,] keyArray = new int[x, x];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    keyArray[i, j] = key[i * x + j];
                }
            }

            int det = this.Det(keyArray) % 26;

            if (det == 0)
            {
                throw new DivideByZeroException();
            }

            if (det < 0)
            {
                det += this.modValue;
            }

            for (int i = 0; i < 26; i++)
            {
                if (det * i % 26 == 1)
                {
                    det = i;
                    break;
                }
            }


            int[,] invertedKey = new int[x, x];

            for (int j = 0; j < x; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    int[,] sub = this.GetSubmatrix(keyArray, x, j, i);

                    var temp = this.Det(sub);

                    var v = (((int)det * (int)Math.Pow(-1, j + i) * temp) % this.modValue);

                    if (v < 0)
                    {
                        v += this.modValue;
                    }

                    invertedKey[j, i] = v;
                }
            }

            var result = this.Multiply(invertedKey, ct);

            var list = new List<int>();
            for (int j = 0; j < result.GetLength(1); j++)
            {
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    list.Add((result[i, j] + this.modValue) % this.modValue);
                }
            }

            // Console.WriteLine(Det(keyArray));

            return list;
        }

        public string Decrypt(string mainCipher, string mainKey)
        {
            var ct = new List<int>();
            mainCipher = mainCipher.ToLower();
            foreach (var letter in mainCipher)
            {
                ct.Add(GetCharNumber(letter));
            }

            var kt = new List<int>();
            foreach (var letter in mainKey)
            {
                kt.Add(GetCharNumber(letter));
            }

            List<int> pt = this.Decrypt(ct, kt);

            string mainPlain = "";
            foreach (var number in pt)
            {
                mainPlain += (char)(number + 'a');
            }

            return mainPlain;
        }

        public int GetCharNumber(char letter)
        {
            if (!char.IsLetter(letter))
            {
                throw new ArgumentException("Input must be a letter");
            }

            int offset = char.IsLower(letter) ? 'a' : 'A';

            return (char.ToLower(letter) - offset) % this.modValue;
        }

        public string Encrypt(string mainPlain, string mainKey)
        {
            var pt = new List<int>();
            foreach (var letter in mainPlain)
            {
                pt.Add(GetCharNumber(letter));
            }

            var kt = new List<int>();
            foreach (var letter in mainKey)
            {
                kt.Add(GetCharNumber(letter));
            }

            List<int> ct = this.Encrypt(pt, kt);

            string mainCipher = "";
            foreach (var number in ct)
            {
                mainCipher += (char)(number + 'a');
            }

            return mainCipher;
        }

        public List<int> Encrypt(List<int> plainText, List<int> key)
        {
            int x = (int)Math.Sqrt(key.Count());
            int y = (int)Math.Ceiling((decimal)(plainText.Count() / x));

            int[,] pt = new int[x, y];

            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    int index = x * j + i;

                    pt[i, j] = index < plainText.Count()
                        ? plainText[index]
                        : 23;
                }
            }

            int[,] keyArray = new int[x, x];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    keyArray[i, j] = key[i * x + j];
                }
            }

            int[,] result = this.Multiply(keyArray, pt);

            var list = new List<int>();
            for (int j = 0; j < result.GetLength(1); j++)
            {
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    Console.WriteLine(result[i, j]);

                    list.Add(result[i, j]);
                }
            }

            return list;
        }

        public List<int> Analyse3By3Key(List<int> plain3, List<int> cipher3)
        {
            return this.Analyse(plain3, cipher3);
        }

        public int GCD(int a, int b)
        {
            return b == 0
                ? a
                : GCD(b, a % b);
        }

        public int[,] GetSubmatrix(int[,] matrix, int dimension, int colToRemove, int rowToRemove = 0)
        {
            int[,] submatrix = new int[dimension - 1, dimension - 1];
            int rowIndex = 0;

            for (int i = 0; i < dimension; i++)
            {
                if (i != rowToRemove)
                {
                    int colIndex = 0;
                    for (int j = 0; j < dimension; j++)
                    {
                        if (j != colToRemove)
                        {
                            submatrix[rowIndex, colIndex] = matrix[i, j];
                            colIndex++;
                        }
                    }
                    rowIndex++;
                }
            }

            return submatrix;
        }

        public int Det(int[,] matrix, bool is2d = false)
        {
            int x = (int)Math.Sqrt(matrix.Length);

            if (x == 1)
            {
                return matrix[0, 0];
            }

            if (x == 2)
            {
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            }

            int result = 0;
            for (int j = 0; j < x; j++)
            {
                for (int i = 0; i < (is2d ? x : 1); i++)
                {
                    int[,] sub = this.GetSubmatrix(matrix, x, j, i);

                    result += (int)Math.Pow(-1, j) * matrix[0, j] * Det(sub, is2d);
                }
            }

            return result;
        }

        public int[,] Multiply(int[,] matrixA, int[,] matrixB)
        {
            int rowsA = matrixA.GetLength(0);
            int colsA = matrixA.GetLength(1);
            int rowsB = matrixB.GetLength(0);
            int colsB = matrixB.GetLength(1);

            Console.WriteLine($"Matrix: {rowsA}, {colsA}, {rowsB}, {colsB}");

            if (colsA != rowsB)
                throw new ArgumentException($"Invalid matrix dimensions for multiplication.");

            int[,] result = new int[rowsA, colsB];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < colsA; k++)
                    {
                        sum += matrixA[i, k] * matrixB[k, j];
                    }
                    result[i, j] = sum % this.modValue;
                }
            }

            return result;
        }

        string ICryptographicTechnique<string, string>.Encrypt(string plainText, string key)
        {
            throw new NotImplementedException();
        }

        public string Analyse3By3Key(string plainText, string cipherText)
        {
            var pt = new List<int>();
            plainText = plainText.ToLower();
            foreach (var letter in plainText)
            {
                pt.Add(GetCharNumber(letter));
            }

            var ct = new List<int>();
            cipherText = cipherText.ToLower();
            foreach (var letter in cipherText)
            {
                ct.Add(GetCharNumber(letter));
            }

            var key = this.Analyse(pt, ct);

            string mainKey = "";
            foreach (var number in key)
            {
                mainKey += (char)(number + 'a');
            }

            return mainKey;
        }

        string ICryptographicTechnique<string, string>.Decrypt(string cipherText, string key)
        {
            throw new NotImplementedException();
        }
    }
}