using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangoAttack3
{
    public class Tag
    {
        //private readonly ILogger<Tag> _logger;
        private readonly int id;
        private int pid2;
        private readonly int k1;
        private readonly int k2;
        private int a, b, d;
        private int x1, n1, x2, n2;
        
        public Tag(int id, int pid, int pid2, int k1, int k2)
        {
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
            x1 = pid2 & k1 & k2;
            n1 = a ^ x1;

            x2 = (int)((uint)~pid2 & 0xFFFFFFFF) & k2 & k1;
            n2 = b ^ x2;
        }

        public void CheckN1N2()
        {
            // K1∧n2 xor ⊕(K2∧n2)
            if (d != ((k1 & n2) ^ (k2 & n1)))
            {
                throw new Exception("Failed Tag at check n1-n2");
            }
        }

        public int GenerateE()
        {
            // (K1 xor n1 xor PID) xor (k2 ^ n2)
            return (k1 ^ n1 ^ id) ^ (k2 & n2);
        }

        public int GenerateF()
        {
            // (K1 and n1) xor (K2 and n2)
            return (k1 & n1) ^ (k2 & n2);
        }

        public int RecalculatePseudonym()
        {
            // (K1 and n1) xor (K2 and n2)
            pid2 = pid2 ^ n1 ^ n2;
            return 0;
        }

        public int GetID()
        {
            return id;
        }
    }
}