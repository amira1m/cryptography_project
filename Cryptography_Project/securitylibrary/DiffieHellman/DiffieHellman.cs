using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SecurityLibrary.DiffieHellman
{
    public class DiffieHellman
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
        /// Generates a public key using the Diffie-Hellman key exchange algorithm.
        /// </summary>
        /// <param name="baseValue">The base value (alpha).</param>
        /// <param name="privateKey">The private key.</param>
        /// <param name="modulus">The modulus value (prime number q).</param>
        /// <returns>The public key.</returns>
        public int GeneratePublicKey(int baseValue, int privateKey, int modulus)
        {
            return CalculateModularExponentiation(baseValue, privateKey, modulus);
        }

        /// <summary>
        /// Generates the secret keys for both parties using the Diffie-Hellman key exchange algorithm.
        /// </summary>
        /// <param name="modulus">The prime number q.</param>
        /// <param name="baseValue">The base value (alpha).</param>
        /// <param name="privateKeyA">The private key of party A.</param>
        /// <param name="privateKeyB">The private key of party B.</param>
        /// <returns>A list containing the secret keys for both parties.</returns>
        public List<int> GetKeys(int modulus, int baseValue, int privateKeyA, int privateKeyB)
        {
            List<int> secretKeys = new List<int>();
            int publicKeyA = GeneratePublicKey(baseValue, privateKeyA, modulus);
            int publicKeyB = GeneratePublicKey(baseValue, privateKeyB, modulus);
            int secretKeyA = GeneratePublicKey(publicKeyB, privateKeyA, modulus);
            int secretKeyB = GeneratePublicKey(publicKeyA, privateKeyB, modulus);
            secretKeys.Add(secretKeyA);
            secretKeys.Add(secretKeyB);
            return secretKeys;
        }
    }
}