namespace TangoAttack3
{
    public class Reader
    {
        private int pid, pid2, k1, k2, n1, n2, e, f, y1, y2, y3, id;
        private static Random random = new Random();

        public Reader(int pid, int pid2)
        {
            this.pid = pid;
            this.pid2 = pid2;
        }

        public void GenerateN1N2()
        {
            this.n1 = random.Next(0, (int)Math.Pow(2, Simulator.BitsLength));
            this.n2 = random.Next(0, (int)Math.Pow(2, Simulator.BitsLength));
        }

        public void SetK1K2(int k1, int k2)
        {
            this.k1 = k1;
            this.k2 = k2;
        }

        public int GenerateA() // (PID2 and K1 and K2) xor n1
        {
            return (this.pid2 & this.k1 & this.k2) ^ this.n1;
        }

        public int GenerateB() // (negado PID2and K2 and K1) xor n2
        {
            return ((~this.pid2) & this.k2 & this.k1) ^ this.n2;
        }

        public int GenerateD() // (K1 and n2) xor (K2 and n1)
        {
            return ((this.k1 & this.n2) ^ (this.k2 & this.n1));
        }

        public void ReceivesEF(int e, int f)
        {
            this.e = e;
            this.f = f;
        }

        public void GetID()
        {
            this.y1 = this.k2 ^ this.n2;
            this.y2 = this.k1 ^ this.n1;
            this.y3 = this.e ^ this.y1;
            this.id = this.y3 ^ this.y2;
        }

        public void CheckN1N2()
        {
            if (this.f != ((this.k1 & this.n1) ^ (this.k2 & this.n2)))
            {
                throw new Exception("Failed Reader at check n1-n2");
            }
        }

        public void UpdateN1N2()
        {
            this.n1 = random.Next(0, (int)Math.Pow(2, Simulator.BitsLength));
            this.n2 = random.Next(0, (int)Math.Pow(2, Simulator.BitsLength));
        }

        public int RecalculatePseudonym() // (K1 and n1) xor (K2 and n2)
        {
            this.pid = this.pid2;
            this.pid2 = this.pid2 ^ this.n1 ^ this.n2;
            return 0;
        }
    }
}
