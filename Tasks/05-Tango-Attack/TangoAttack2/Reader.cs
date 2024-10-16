using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangoAttack2
{
    public class Reader
    {
        private int pid, pid2, k1, k2, n1, n2;
        private int e, f;
        private int id;

        public Reader(int pid, int pid2, int k1, int k2)
        {
            this.pid = pid;
            this.pid2 = pid2;
            this.k1 = k1;
            this.k2 = k2;
            Random random = new Random();
            this.n1 = random.Next(0, (int)Math.Pow(2, 8));
            this.n2 = random.Next(0, (int)Math.Pow(2, 8));
            Console.WriteLine("[Init] - Initializing Reader");
        }

        public int GenerateA() // (PID2 and K1 and K2) xor n1
        {
            return (pid2 & k1 & k2) ^ n1;
        }

        public uint GenerateB() // (~PID2 and K2 and K1) xor n2
        {
            return (uint)((~pid2 & k2 & k1) ^ n2);
        }

        public int GenerateD() // (K1 and n2) xor (K2 and n1)
        {
            return (k1 & n2) ^ (k2 & n1);
        }

        public void ReceivesEF(int e, int f)
        {
            this.e = e;
            this.f = f;
        }

        public int GetID()
        {
            int y1 = k2 ^ n2;
            int y2 = k1 ^ n1;
            int y3 = e ^ y1;
            id = y3 ^ y2;
            return id;
        }

        public void CheckN1N2()
        {
            if (f != ((k1 & n1) ^ (k2 & n2)))
                throw new Exception("Failed Reader at check n1-n2");
        }

        public void UpdateN1N2()
        {
            Random random = new Random();
            n1 = random.Next(0, (int)Math.Pow(2, 8));
            n2 = random.Next(0, (int)Math.Pow(2, 8));
        }

        public void RecalculatePseudonim()
        {
            pid = pid2;
            pid2 = pid2 ^ n1 ^ n2;
        }
    }
}