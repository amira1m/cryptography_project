using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.AES
{
    public class Round
    {
        public int q, A1, A2, A3, B1, B2, B3;
        public Round(int q, int A1, int A2, int A3, int B1, int B2, int B3) 
        {
            this.q = q;
            this.A1 = A1;
            this.A2 = A2;
            this.A3 = A3;
            this.B1 = B1;
            this.B2 = B2;
            this.B3 = B3;
        }
    }
}
