using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.AES
{
    /// <summary>
    /// If the string starts with 0x.... then it's Hexadecimal not string
    /// </summary>
    public class AES : CryptographicTechnique
    {
        private string FixLength(string s, int length)
        {
            return s.PadLeft(length, '0');
        }

        private string XOR(string v1, string v2)
        {
            v1 = HexaToBinary(v1);
            v2 = HexaToBinary(v2);

            int fixedLength = Math.Max(v1.Length, v2.Length);
            string xor_val = "";

            v1 = (v1.Length < fixedLength) ? FixLength(v1, fixedLength) : v1;
            v2 = (v2.Length < fixedLength) ? FixLength(v2, fixedLength) : v2;

            for (int i = 0; i < fixedLength; i++)
                xor_val += (v1[i] != v2[i]) ? "1" : "0";

            return xor_val;

        }

        private string[,] ToMatrix(string str)
        {
            string[,] matrix = new string[4, 4];
            int indexer = 2;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[j, i] += str[indexer];
                    matrix[j, i] += str[indexer + 1];
                    indexer += 2;
                }
            }
            return matrix;
        }
        private int[,] MatrixToInt(string[,] matrix)
        {
            int[,] result = new int[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result[i, j] = Convert.ToInt32(matrix[i, j], 16);
                }
            }
            return result;
        }
        private string ToString(string[,] matrix)
        {
            string str = "0x";
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[j, i].Length == 1)
                        str += "0" + matrix[j, i];
                    else
                        str += matrix[j, i];
                }
            }
            return str;
        }
        private string ToHexa(string n)
        {
            int decimalValue = Convert.ToInt32(n, 2);
            string hexString = decimalValue.ToString("X");
            return hexString;
        }

        private string HexaToBinary(string hexa)
        {
            long decimalValue = Convert.ToInt64(hexa, 16);
            string binaryValue = Convert.ToString(decimalValue, 2).PadLeft(64, '0');
            return binaryValue;
        }

        private string BinaryToHexa(string binary)
        {
            long decimalValue = Convert.ToInt64(binary, 2);
            string hexValue = decimalValue.ToString("X");
            return hexValue;
        }

        private string[,] GenerateRoundKey(string[,] prevKey, int round)
        {
            string[,] key = new string[prevKey.GetLength(0), prevKey.GetLength(1)];
            for (int i = 0; i < key.GetLength(0); i++)
            {
                for (int j = 0; j < key.GetLength(1); j++)
                {
                    if (i > 0)
                    {
                        key[j, i] = XOR(key[j, i - 1], prevKey[j, i]);
                        key[j, i] = ToHexa(key[j, i]);
                    }
                    else
                    {
                        key[j, i] = prevKey[(j + 1) % key.GetLength(1), prevKey.GetLength(0) - 1];
                        key[j, i] = Matrices.ReplaceFromSBox(key[j, i]);
                        string x1 = XOR(key[j, i], prevKey[j, i]);
                        x1 = ToHexa(x1);
                        key[j, i] = XOR(x1, Matrices.RCtable[round - 1, j].ToString("X"));
                        key[j, i] = ToHexa(key[j, i]);
                    }
                }
            }
            return key;
        }

        private string[,] InverseShiftRows(string[,] matrix)
        {
            string[,] result = new string[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result[i, j] = matrix[i, (j - i + matrix.GetLength(1)) % matrix.GetLength(1)];
                }
            }
            return result;
        }
        private string[,] InverseSubBytes(string[,] matrix)
        {
            string[,] result = new string[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result[i, j] = Matrices.ReplaceFromReversedSBox(matrix[i, j]);
                }
            }
            return result;
        }
        private string[,] InverseMixColumn(string[,] matrix)
        {
            return Matrices.MatrixMult(Matrices.RijndaelInverseMixColumnsMatrix, MatrixToInt(matrix));
        }

        public override string Decrypt(string cipherText, string key)
        {
            string[,] keyMatrix = ToMatrix(key);
            string[,] cipherMatrix = ToMatrix(cipherText);
            Dictionary<int, string[,]> keys = new Dictionary<int, string[,]>();
            for (int i = 1; i <= 10; i++)
                keys[i] = (i == 1) ? GenerateRoundKey(keyMatrix, i) : GenerateRoundKey(keys[i - 1], i);

            string[,] matrix = new string[cipherMatrix.GetLength(0), cipherMatrix.GetLength(1)];

            for (int i = 10; i >= 1; i--)
            {
                matrix = (i == 10) ? AddRoundKey(keys[i], cipherMatrix) : AddRoundKey(keys[i], matrix);
                matrix = (i != 10) ? InverseMixColumn(matrix) : matrix;
                matrix = InverseShiftRows(matrix);
                matrix = InverseSubBytes(matrix);
            }
            matrix = AddRoundKey(matrix, keyMatrix);

            return ToString(matrix);
        }
        private string[,] AddRoundKey(string[,] matrix, string[,] keyMatrix)
        {
            string[,] result = new string[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    string answer = XOR(matrix[i, j], keyMatrix[i, j]);
                    result[i, j] = BinaryToHexa(answer);
                }
            }

            return result;
        }
        private string[,] SubBytes(string[,] matrix)
        {
            string[,] subbedMatrix = new string[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    subbedMatrix[i, j] = Matrices.ReplaceFromSBox(matrix[i, j]);
                }
            }
            return subbedMatrix;
        }

        private string[,] ShiftRows(string[,] matrix)
        {
            string[,] result = new string[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result[i, j] = matrix[i, (j + i) % matrix.GetLength(1)];
                }
            }

            return result;
        }
        private string[,] MixColumns(string[,] matrix)
        {
            return Matrices.MatrixMult(Matrices.RijndaelMixColumnsMatrix, MatrixToInt(matrix));
        }
        public string[,] InitiateState(string plainText, string key)
        {
            string[,] plainMatrix = ToMatrix(plainText);
            string[,] keyMatrix = ToMatrix(key);
            return AddRoundKey(plainMatrix, keyMatrix);
        }
        public override string Encrypt(string plainText, string key)
        {
            string[,] matrix = InitiateState(plainText, key);
            string[,] keyMatrix = ToMatrix(key);
            for (int i = 1; i <= 10; i++)
            {
                keyMatrix = GenerateRoundKey(keyMatrix, i);
                matrix = SubBytes(matrix);
                matrix = ShiftRows(matrix);
                matrix = (i != 10) ? MixColumns(matrix) : matrix;
                matrix = AddRoundKey(matrix, keyMatrix);
            }
            return ToString(matrix);
        }
    }
}
