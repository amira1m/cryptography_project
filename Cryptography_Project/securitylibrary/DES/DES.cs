using SecurityLibrary.AES;
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
    public class DES : CryptographicTechnique
    {
        private string FixLength(string s, int length)
        {
            return s.PadLeft(length, '0');
        }

        private string XOR(string v1, string v2)
        {

            string xor_val = "";
            int fixedLength = Math.Max(v1.Length, v2.Length), it = 0;
            v1 = (v1.Length < fixedLength) ? FixLength(v1, fixedLength) : v1;
            v2 = (v2.Length < fixedLength) ? FixLength(v2, fixedLength) : v2;

            for (int i = 0; i < fixedLength; i++)
                xor_val += (v1[i] != v2[i]) ? "1" : "0";

            return xor_val;
        }
        private string Encode(string key, List<int> permutation)
        {
            string result = "";
            foreach (var item in permutation)
                result += key[item - 1];
            return result;
        }
        private string GenerateSubKey(string s, int round)
        {
            if (round != 1 && round != 2 && round != 9 && round != 16)
            {
                s += s[0];
                s += s[1];
                return s.Substring(2);
            }
            else
            {
                s += s[0];
                return s.Substring(1);
            }
        }
        public static string ReplaceFromSBox(string str)
        {
            string result = "";
            for (int i = 1; i <= 8; i++)
            {
                string tmp = str.Substring(6 * (i - 1), 6);
                string s = "";
                s += tmp[0]; s += tmp[5];
                int x = Convert.ToInt32(s, 2);
                s = ""; s += tmp[1]; s += tmp[2]; s += tmp[3]; s += tmp[4];
                int y = Convert.ToInt32(s, 2);
                result += Convert.ToString(Matrices.SBox_3D[i][x, y], 2).PadLeft(4, '0');
            }
            return result;
        }
        private string RightValue(string r, string key)
        {
            r = Encode(r, Matrices.E_BitSelection);
            string XORed_value = XOR(r, key);
            string replacedValues = ReplaceFromSBox(XORed_value);
            return Encode(replacedValues, Matrices.P);
        }
        private string Divide(string s, string returnedHalf)
        {
            return (returnedHalf == "LEFT") ? s.Substring(0, s.Length / 2) : s.Substring(s.Length / 2);
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

        public static string ListToString(char[] list)
        {
            string result = "";
            foreach (var item in list)
                result += item;
            return result;
        }

        private string Decode(string s, List<int> permutation)
        {
            char[] result = new char[permutation.Max()];
            for (int i = 0; i < s.Length; i++)
            {
                result[permutation[i] - 1] = s[i];
            }

            return ListToString(result);
        }
        private List<string> GetKeys(string k0)
        {
            List<string> keys = new List<string>() { k0 };
            string c = Divide(k0, "LEFT");
            string d = Divide(k0, "RIGHT");

            for (int i = 1; i <= 16; i++)
            {
                c = GenerateSubKey(c, i);
                d = GenerateSubKey(d, i);
                string k = Encode(string.Concat(c, d), Matrices.PC_2);
                keys.Add(k);
            }
            return keys;
        }
        public override string Decrypt(string cipherText, string key)
        {
            key = HexaToBinary(key);
            string key_plus = Encode(key, Matrices.PC_1);
            List<string> keys = GetKeys(key_plus);

            cipherText = HexaToBinary(cipherText);
            cipherText = Decode(cipherText, Matrices.IP_neg1);
            string left = Divide(cipherText, "LEFT");
            string right = Divide(cipherText, "RIGHT");

            for (int i = 16; i >= 1; i--)
            {
                string tmpL = right;
                string tmpR = XOR(left, RightValue(right, keys[i]));
                left = tmpL; right = tmpR;

            }
            string plainText = Decode(right + left, Matrices.IP);

            return "0x" + BinaryToHexa(plainText).PadLeft(16, '0');
        }
        private string FinalStep(string left, string right)
        {
            string combined = string.Concat(right, left);
            long decimalValue = Convert.ToInt64(Encode(combined, Matrices.IP_neg1), 2);
            string hexValue = decimalValue.ToString("X");
            return "0x" + hexValue;
        }

        public override string Encrypt(string plainText, string key)
        {
            plainText = HexaToBinary(plainText);
            string EncodedPlain = Encode(plainText, Matrices.IP);
            string l = Divide(EncodedPlain, "LEFT");
            string r = Divide(EncodedPlain, "RIGHT");

            key = HexaToBinary(key);
            string key_plus = Encode(key, Matrices.PC_1);
            string c = Divide(key_plus, "LEFT");
            string d = Divide(key_plus, "RIGHT");

            for (int i = 1; i <= 16; i++)
            {
                c = GenerateSubKey(c, i);
                d = GenerateSubKey(d, i);
                string k = Encode(string.Concat(c, d), Matrices.PC_2);
                string newL = r;
                string newR = XOR(l, RightValue(r, k));
                l = newL;
                r = newR;
            }

            return FinalStep(l, r);
        }
    }
}
