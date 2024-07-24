using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class PlayFair : ICryptographic_Technique<string, string>
    {
        public string Analyse(string cipherText)
        {
            throw new NotImplementedException();

        }

        public string Decrypt(string cipherText, string key)
        {
            key = key.ToUpper();
            var cipherPairs = new List<string>();
            char[,] matrix = new char[5, 5];
            string keyCharacters = string.Empty;
            StringBuilder decryptedText, alphabetString;
            int count = 0;
            string alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
            cipherText = cipherText.ToUpper();
            decryptedText = new StringBuilder();
            string tempText;
            var uniqueKeyChars = new HashSet<char>(key);
            string remainingAlphabet = string.Empty;
            string plainText;

            foreach (char ch in uniqueKeyChars)
            {
                keyCharacters += ch;
            }

            alphabetString = new StringBuilder(alphabet);

            foreach (char ch in keyCharacters)
            {
                foreach (char alphaChar in alphabet)
                {
                    if (ch == alphaChar)
                    {
                        int index = alphabetString.ToString().IndexOf(alphaChar);
                        alphabetString[index] = index != -1 ? ' ' : alphabetString[index];
                    }
                }
            }

            for (int i = 0; i < alphabetString.Length; i++)
            {
                remainingAlphabet += alphabetString[i] != ' ' ? alphabetString[i].ToString() : string.Empty;
            }

            string matrixElement = keyCharacters + remainingAlphabet;


            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    matrix[i, j] = matrixElement[count];
                    count++;
                }
            }


            for (int i = 0; i < cipherText.Length; i += 2)
            {
                cipherPairs.Add(cipherText.Substring(i, 2));
            }

            int row1 = -1, col1 = -1, row2 = -1, col2 = -1;
            foreach (string pair in cipherPairs)
            {
                for (int k = 0; k < 2; k++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (matrix[i, j] == pair[k])
                            {
                                if (k == 0)
                                {
                                    row1 = i;
                                    col1 = j;
                                }
                                else
                                {
                                    row2 = i;
                                    col2 = j;
                                }
                            }
                        }
                    }
                }

                if (row1 == row2)
                {
                    col1 = col1 == 0 ? 5 : col1;
                    col2 = col2 == 0 ? 5 : col2;
                    decryptedText.Append(matrix[row1, (col1 - 1) % 5]);
                    decryptedText.Append(matrix[row2, (col2 - 1) % 5]);
                }
                else if (col1 == col2)
                {
                    row1 = row1 == 0 ? 5 : row1;
                    row2 = row2 == 0 ? 5 : row2;
                    decryptedText.Append(matrix[(row1 - 1) % 5, col1]);
                    decryptedText.Append(matrix[(row2 - 1) % 5, col2]);
                }
                else
                {
                    decryptedText.Append(matrix[row1, col2]);
                    decryptedText.Append(matrix[row2, col1]);
                }

                row1 = col1 = row2 = col2 = 0;
            }


            RemoveLastX(ref decryptedText);

            tempText = decryptedText.ToString();
            StringBuilder plainTextBuilder = new StringBuilder(tempText);


            RemoveExtraX(ref plainTextBuilder);
            plainText = plainTextBuilder.ToString();
            return plainText;
        }

        public string Encrypt(string plainText, string key)
        {
            // throw new NotImplementedException();

            key = key.ToLower();
            plainText = plainText.ToLower();
            Console.WriteLine("plain: " + plainText);

            //plainText = CheckAdjacentChars(plainText);

            plainText = CheckPairs(plainText);
            Console.WriteLine("plain: " + plainText);

            plainText = CheckOddLength(plainText);
            Console.WriteLine("plain: " + plainText);

            List<string> pairs = SplitIntoPairs(plainText);
            string letters = "abcdefghiklmnopqrstuvwxyz", cipherText = "";
            char[,] matrix = new char[5, 5];

            // Fill Matrix with data
            FillMatrix(matrix, key, letters);

            Console.WriteLine("rowsss: " + AreCharsInSameRow(matrix, 'o', 'l'));
            Console.WriteLine("colsss: " + AreCharsInSameColumn(matrix, 'd', 'e'));

            printMatrix(matrix);

            Console.WriteLine("plain: " + plainText);
            foreach (string item in pairs)
            {
                char[] result;
                string charsCase = CheckCharsCase(matrix, item[0], item[1]);
                Console.WriteLine("item: " + item);
                // Console.WriteLine("item 1: " + item[0]);
                // Console.WriteLine("item 2: " + item[1]);
                Console.WriteLine("case: " + charsCase);
                switch (charsCase)
                {
                    // case 1
                    case "sameRow":
                        result = GetNextCharsRow(matrix, item[0], item[1]);
                        Console.WriteLine($"Next chars in row: {result[0]}, {result[1]}");
                        //cipherText += result[0] + result[1];
                        cipherText = AppendCharsToString(cipherText, result);
                        Console.WriteLine("cipher: " + cipherText);
                        break;
                    // case 2                
                    case "sameColumn":
                        result = GetNextCharsColumn(matrix, item[0], item[1]);
                        Console.WriteLine($"Next chars in column: {result[0]}, {result[1]}");
                        cipherText = AppendCharsToString(cipherText, result);
                        Console.WriteLine("cipher: " + cipherText);
                        break;
                    // case 3
                    case "Rectangle":
                        result = FindOtherCorners(matrix, item[0], item[1]);
                        Console.WriteLine($"The other two corners are: {result[0]}, {result[1]}");
                        cipherText = AppendCharsToString(cipherText, result);
                        Console.WriteLine("cipher: " + cipherText);
                        break;
                    default:
                        break;
                }

            }


            return cipherText;
        }

        public void RemoveLastX(ref StringBuilder text)
        {
            if (text.Length > 0 && text[text.Length - 1] == 'X')
            {
                text.Remove(text.Length - 1, 1);
            }
        }
        public void RemoveExtraX(ref StringBuilder text)
        {
            int t = 1;
            while (t < text.Length)
            {
                if (text[t] == 'X' && text[t + 1] == text[t - 1])
                {
                    text.Remove(t, 1);
                    t++;
                }
                t += 2;
            }
        }
        public string CheckPairs(string plaintext)
        {
            string newplaintext = "";
            bool bol = false;
            for (int i = 0; i < plaintext.Length - 1; i += 2)
            {
                if (plaintext[i] != plaintext[i + 1])
                {
                    newplaintext += plaintext[i];
                    newplaintext += plaintext[i + 1];
                }
                else
                {
                    newplaintext += plaintext[i];
                    newplaintext += 'x';
                    bol = true;
                    i--;
                }
            }
            int newLength = newplaintext.Length;
            if ((newLength == plaintext.Length && bol == true) || (newLength < plaintext.Length))
            {
                newplaintext += plaintext[plaintext.Length - 1];
            }
            if (newLength % 2 != 0 && newplaintext[newLength - 1] != 'x')
            {
                newplaintext += 'x';
            }

            return newplaintext;
        }
        public static void FillMatrix(char[,] matrix, string key, string letters)
        {
            int row = 0, col = 0;

            foreach (char ch in key)
            {
                if (letters.IndexOf(ch) != -1)
                {
                    matrix[row, col] = ch;

                    letters = letters.Remove(letters.IndexOf(ch), 1);

                    col++;
                    if (col == 5)
                    {
                        col = 0;
                        row++;
                    }
                }
            }

            foreach (char ch in letters)
            {
                matrix[row, col] = ch;
                col++;
                if (col == 5)
                {
                    col = 0;
                    row++;
                }
            }
        }

        public bool ElementExists(int[,] array, int element)
        {
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (array[i, j] == element)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void printMatrix(char[,] matrix)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        public int[] FindIndices(char[,] matrix, char target)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix[i, j] == target)
                    {
                        return new int[] { i, j };
                    }
                }
            }

            // If the target is not found, return null
            return null;
        }
        public bool AreCharsInSameRow(char[,] matrix, char char1, char char2)
        {
            int[] indicesOfChar1 = FindIndices(matrix, char1);
            int[] indicesOfChar2 = FindIndices(matrix, char2);
            Console.WriteLine("char1: " + indicesOfChar1[0] + " " + indicesOfChar1[1]);
            Console.WriteLine("char2: " + indicesOfChar2[0] + " " + indicesOfChar2[1]);
            if (indicesOfChar1[0] == indicesOfChar2[0])
                return true;

            return false;
        }

        public bool AreCharsInSameColumn(char[,] matrix, char char1, char char2)
        {
            int[] indicesOfChar1 = FindIndices(matrix, char1);
            int[] indicesOfChar2 = FindIndices(matrix, char2);
            Console.WriteLine("char1: " + indicesOfChar1[0] + " " + indicesOfChar1[1]);
            Console.WriteLine("char2: " + indicesOfChar2[0] + " " + indicesOfChar2[1]);
            if (indicesOfChar1[1] == indicesOfChar2[1])
                return true;

            return false;
        }

        public string CheckCharsCase(char[,] matrix, char char1, char char2)
        {
            if (AreCharsInSameRow(matrix, char1, char2))
                return "sameRow";
            else if (AreCharsInSameColumn(matrix, char1, char2))
                return "sameColumn";
            else
                return "Rectangle";
        }

        public string CheckAdjacentChars(string str)
        {
            string modifiedStr = "";
            for (int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == str[i + 1])
                {
                    modifiedStr += str[i] + "x";
                }
                else
                {
                    modifiedStr += str[i];
                }
            }
            modifiedStr += str[str.Length - 1];
            return modifiedStr;
        }

        public string CheckOddLength(string str)
        {
            if (str.Length % 2 != 0 && str[str.Length - 1] != str[str.Length - 2])
            {
                str += "x";
            }
            return str;
        }

        public List<string> SplitIntoPairs(string input)
        {
            List<string> pairs = new List<string>();

            for (int i = 0; i < input.Length; i += 2)
            {
                pairs.Add(input.Substring(i, 2));
            }

            return pairs;
        }


        public char[] FindOtherCorners(char[,] matrix, char corner1, char corner2)
        {
            char[] corners = new char[2];

            int row1 = -1, col1 = -1, row2 = -1, col2 = -1;

            // Find the positions of the two corners
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (matrix[i, j] == corner1)
                    {
                        row1 = i;
                        col1 = j;
                    }
                    else if (matrix[i, j] == corner2)
                    {
                        row2 = i;
                        col2 = j;
                    }
                }
            }

            // Determine the other two corners
            corners[0] = matrix[row1, col2];
            corners[1] = matrix[row2, col1];

            return corners;
        }

        public char[] GetNextCharsRow(char[,] matrix, char firstChar, char secondChar)
        {
            char[] result = new char[2] { '\0', '\0' }; // default values if not found

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == firstChar)
                    {
                        result[0] = matrix[i, (j + 1) % matrix.GetLength(1)]; // getting next char in the row
                    }
                    if (matrix[i, j] == secondChar)
                    {
                        result[1] = matrix[i, (j + 1) % matrix.GetLength(1)]; // getting next char in the row
                    }
                }
            }

            return result;
        }

        public char[] GetNextCharsColumn(char[,] matrix, char firstChar, char secondChar)
        {
            char[] result = new char[2] { '\0', '\0' }; // default values if not found

            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    if (matrix[i, j] == firstChar)
                    {
                        result[0] = matrix[(i + 1) % matrix.GetLength(0), j]; // getting next char in the column
                    }
                    if (matrix[i, j] == secondChar)
                    {
                        result[1] = matrix[(i + 1) % matrix.GetLength(0), j]; // getting next char in the column
                    }
                }
            }

            return result;
        }
        static string AppendCharsToString(string originalString, char[] chars)
        {
            return originalString + chars[0] + chars[1];
        }
    }
}