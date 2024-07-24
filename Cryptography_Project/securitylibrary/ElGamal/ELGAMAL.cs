using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.ElGamal
{
    public class ElGamal
    {
        private readonly RSA.RSA _rsa = new RSA.RSA();
        private readonly AES.ExtendedEuclid _extendedEuclid = new AES.ExtendedEuclid();

        /// <summary>
        /// Encrypts a message using the ElGamal encryption algorithm.
        /// </summary>
        /// <param name="q">The prime number q.</param>
        /// <param name="alpha">The primitive root modulo q.</param>
        /// <param name="y">The public key component y.</param>
        /// <param name="k">The random integer k.</param>
        /// <param name="m">The message to be encrypted.</param>
        /// <returns>A list containing the ciphertext components [C1, C2].</returns>
        public List<long> Encrypt(int q, int alpha, int y, int k, int m)
        {
            long c1 = _rsa.CalculateModularExponentiation(alpha, k, q);
            long kValue = _rsa.CalculateModularExponentiation(y, k, q);
            long c2 = _rsa.CalculateModularExponentiation((int)(kValue * m), 1, q);

            return new List<long> { c1, c2 };
        }

        /// <summary>
        /// Decrypts a message using the ElGamal decryption algorithm.
        /// </summary>
        /// <param name="c1">The first component of the ciphertext.</param>
        /// <param name="c2">The second component of the ciphertext.</param>
        /// <param name="x">The private key component x.</param>
        /// <param name="q">The prime number q.</param>
        /// <returns>The decrypted message.</returns>
        public int Decrypt(int c1, int c2, int x, int q)
        {
            int k = _rsa.CalculateModularExponentiation(c1, x, q);
            int kInverse = _extendedEuclid.GetMultiplicativeInverse(k, q);
            int m = _rsa.CalculateModularExponentiation(c2 * kInverse, 1, q);

            return m;
        }
    }
}