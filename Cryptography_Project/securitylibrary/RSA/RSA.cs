using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.RSA
{
    public class RSA
    {
        /// <summary>
        /// Calculates (baseValue^exponent) mod modulus.
        /// </summary>
        /// <param name="baseValue">The base value.</param>
        /// <param name="exponent">The exponent value.</param>
        /// <param name="modulus">The modulus value.</param>
        /// <returns>The result of (baseValue^exponent) mod modulus.</returns>
        public int CalculateModularExponentiation(int baseValue, int exponent, int modulus)
        {
            int result = 1;
            while (exponent != 0)
            {
                result = (result * baseValue) % modulus;
                exponent--;
            }
            return result;
        }

        /// <summary>
        /// Encrypts a message using the RSA encryption algorithm.
        /// </summary>
        /// <param name="primeP">Prime number p.</param>
        /// <param name="primeQ">Prime number q.</param>
        /// <param name="message">The message to be encrypted.</param>
        /// <param name="publicExponent">The public key exponent.</param>
        /// <returns>The encrypted message.</returns>
        public int Encrypt(int primeP, int primeQ, int message, int publicExponent)
        {
            int modulus = primeP * primeQ;
            return CalculateModularExponentiation(message, publicExponent, modulus);
        }

        /// <summary>
        /// Decrypts a message using the RSA decryption algorithm.
        /// </summary>
        /// <param name="primeP">Prime number p.</param>
        /// <param name="primeQ">Prime number q.</param>
        /// <param name="cipherText">The encrypted message (ciphertext).</param>
        /// <param name="publicExponent">The public key exponent.</param>
        /// <returns>The decrypted message.</returns>
        public int Decrypt(int primeP, int primeQ, int cipherText, int publicExponent)
        {
            int modulus = primeP * primeQ;
            int totient = (primeP - 1) * (primeQ - 1);
            AES.ExtendedEuclid extendedEuclid = new AES.ExtendedEuclid();
            int privateExponent = extendedEuclid.GetMultiplicativeInverse(publicExponent, totient);
            return CalculateModularExponentiation(cipherText, privateExponent, modulus);
        }
    }
}