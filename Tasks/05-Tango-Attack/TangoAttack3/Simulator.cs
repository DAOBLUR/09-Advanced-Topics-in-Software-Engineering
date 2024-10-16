using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangoAttack3
{
    public class Simulator
    {
        public static int BitsLength { get; private set; } = 8;

        private readonly int sessions;
        private readonly Server server;
        private readonly Reader reader;
        private readonly Tag tag;
        private readonly Attacker attacker;
        private int a, b, d, e, f;

        public Simulator(int sessions, int bitsLength)
        {
            this.sessions = sessions;

            if (bitsLength >= 4 && bitsLength <= 8)
            {
                BitsLength = bitsLength;
            }

            var random = new Random();
            int pid = random.Next(0, (int)Math.Pow(2, BitsLength));
            int pid2 = random.Next(0, (int)Math.Pow(2, BitsLength));
            int id = random.Next(0, (int)Math.Pow(2, BitsLength));

            server = new Server();
            var (k1, k2) = server.GetK1K2();

            reader = new Reader(pid, pid2);
            tag = new Tag(id, pid, pid2, k1, k2);
            attacker = new Attacker(id, k1, k2, sessions);
        }

        public void StartSimulation()
        {
            for (int i = 0; i < sessions+1; i++)
            {
                // Step 4
                reader.GenerateN1N2();

                var (k1, k2) = server.GetK1K2();
                reader.SetK1K2(k1, k2);

                a = reader.GenerateA();
                b = reader.GenerateB();
                d = reader.GenerateD();

                tag.ReceivesABD(a, b, d);
                
                // Reader -> Attacker
                attacker.ReceivesABD(a, b, d);

                // Step 5
                tag.CalculateN1N2();
                tag.CheckN1N2();

                e = tag.GenerateE();
                f = tag.GenerateF();

                // Tag -> Attacker
                attacker.ReceivesEF(e, f);

                // Step 6
                reader.ReceivesEF(e, f);
                reader.GetID();
                reader.CheckN1N2();

                // Round finished, now Attacker tries to guess k1, k2, ID
                attacker.ComputeApproximation(i);

                // Step 7
                reader.RecalculatePseudonym();
                reader.UpdateN1N2();
                tag.RecalculatePseudonym();
            }
        }

        public (int, int, int) GetK1K2ID()
        {
            return (server.GetK1K2().Item1, server.GetK1K2().Item2, tag.GetID());
        }

        public (int, int, int) GetK1K2IDEstimation()
        {
            return attacker.GetK1K2IDEstimation();
        }

        public (List<int>, List<int>, List<int>) GetData()
        {
            return attacker.GetData();
        }
    }
}