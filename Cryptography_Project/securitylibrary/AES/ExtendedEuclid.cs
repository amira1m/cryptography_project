using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.AES
{
    public class ExtendedEuclid 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="baseN"></param>
        /// <returns>Mul inverse, -1 if no det_neg_1</returns>
        public int GetMultiplicativeInverse(int number, int baseN)
        {
            List<Round> rounds = new List<Round>
            {
                new Round(0, 1, 0, baseN, 0, 1, number)
            };
            while (true)
            {
                if (rounds.Last().B3 == 0)
                    return -1;
                else if (rounds.Last().B3 == 1)
                    return mod(rounds.Last().B2, baseN);

                int Q = rounds.Last().A3 / rounds.Last().B3;
                rounds.Add(new Round(Q,
                    rounds.Last().B1,
                    rounds.Last().B2,
                    rounds.Last().B3,
                    rounds.Last().A1 - Q * rounds.Last().B1,
                    rounds.Last().A2 - Q * rounds.Last().B2,
                    rounds.Last().A3 - Q *  rounds.Last().B3));
            }
        }
        private int mod(int n, int b) => n > 0 ? n : b - (n * -1 % b); 
    }
}
