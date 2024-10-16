using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangoAttack2
{
    public class Tag
    {
        private int id, pid2, k1, k2;
        private int a, b, d, n1, n2;

        public Tag(int id, int pid, int pid2, int k1, int k2)
        {
            Console.WriteLine("[Init] - Initializing Tag");
            this.id = id;
            this.pid2 = pid2;
            this.k1 = k1;
            this.k2 = k2;
        }

        public void ReceivesABD(int a, int b, int d)
        {
            this.a = a;
            this.b = b;
            this.d = d;
        }

        public void CalculateN1N2()
        {
            // n1 = A XOR (pid2 & k1 & k2)
            int x1 = pid2 & k1 & k2;
            n1 = a ^ x1;

            // n2 = B XOR (~pid2 & k2 & k1)
            int x2 = (~pid2) & k2 & k1;
            n2 = b ^ x2;
        }

        public void CheckN1N2()
        {
            // K1∧n2 xor ⊕(K2∧n2)
            if (d != ((k1 & n2) ^ (k2 & n1)))
                throw new Exception("Failed Tag at check n1-n2");
        }

        public int GenerateE()
        {
            // (K1 xor n1 xor PID) xor (k2 ^ n2)
            return ((k1 ^ n1 ^ id) ^ (k2 & n2));
        }

        public int GenerateF()
        {
            // (K1 and n1) xor (K2 and n2)
            return ((k1 & n1) ^ (k2 & n2));
        }

        public void RecalculatePseudonim()
        {
            // (K1 and n1) xor (K2 and n2)
            pid2 = pid2 ^ n1 ^ n2;
        }
    }
}