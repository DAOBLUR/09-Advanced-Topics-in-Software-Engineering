using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Numerics;

namespace TangoAttack3
{
    public class Attacker
    {
        private readonly int id;
        private readonly int k1;
        private readonly int k2;
        private readonly int sessions;

        private readonly List<int>[] k1_list;
        private int k1_estimation;
        private readonly List<int> k1_estimation_list;

        private readonly List<int>[] k2_list;
        private int k2_estimation;
        private readonly List<int> k2_estimation_list;

        private readonly List<int>[] id_list;
        private int id_estimation;
        private readonly List<int> id_estimation_list;

        private int a, b, d, e, f;

        public Attacker(int id, int k1, int k2, int sessions)
        {
            this.id = id;
            this.k1 = k1;
            this.k2 = k2;
            this.sessions = sessions;

            k1_list = new List<int>[Simulator.BitsLength];
            for (int i = 0; i < Simulator.BitsLength; i++) k1_list[i] = new List<int>();
            k1_estimation_list = new List<int>();

            k2_list = new List<int>[Simulator.BitsLength];
            for (int i = 0; i < Simulator.BitsLength; i++) k2_list[i] = new List<int>();
            k2_estimation_list = new List<int>();

            id_list = new List<int>[Simulator.BitsLength];
            for (int i = 0; i < Simulator.BitsLength; i++) id_list[i] = new List<int>();
            id_estimation_list = new List<int>();
        }

        public void ReceivesABD(int a, int b, int d)
        {
            this.a = a;
            this.b = b;
            this.d = d;
        }

        public void ReceivesEF(int e, int f)
        {
            this.e = e;
            this.f = f;
        }

        public void ComputeApproximation(int l)
        {
            K1Estimation(l);
            K2Estimation(l);
            IdEstimation(l);
        }

        public (int, int, int) GetK1K2IDEstimation()
        {
            return (k1_estimation, k2_estimation, id_estimation);
        }
        private void K1Estimation(int s)
        {
            string output = "K1 estimation:";
            k1_estimation = 0;

            int[] operations = 
            { 
                d, 
                f, 
                a ^ d,
                a ^ b ^ f,
                (int)((~(b ^ d) & 0xFF)), 
                b ^ f,
                a ^ b ^ d
            };

            for (int i = 0; i < Simulator.BitsLength; i++)
            {
                foreach (int op in operations)
                {
                    int aux = op / (1 << i);
                    int value = aux % 2;
                    k1_list[i].Add(value);
                }
                k1_estimation += (int)Median(k1_list[i]) * (1 << i);
            }

            k1_estimation_list.Add(k1_estimation);
            if ((sessions - 1) == s)
            {
                Console.WriteLine("K1 value:     " + Convert.ToString(k1, 2).PadLeft(Simulator.BitsLength, '0'));
                Console.WriteLine(output + " " + Convert.ToString(k1_estimation, 2).PadLeft(Simulator.BitsLength, '0'));
            }
        }

        private void K2Estimation(int s)
        {
            string output = "K2 estimation:";
            k2_estimation = 0;

            int[] operations =
            {
                d, 
                f, 
                (int)((~(a ^ d) & 0xFF)), 
                a ^ f, 
                (b ^ d), 
                (int)((~(b ^ f) & 0xFF)), 
                a ^ b ^ f, 
                a ^ b ^ d 
            };

            for (int i = 0; i < Simulator.BitsLength; i++)
            {
                foreach (int op in operations)
                {
                    int aux = op / (1 << i);
                    int value = aux % 2;
                    k2_list[i].Add(value);
                }
                k2_estimation += (int)Median(k2_list[i]) * (1 << i);
            }

            k2_estimation_list.Add(k2_estimation);
            if ((sessions - 1) == s)
            {
                Console.WriteLine("K2 value:     " + Convert.ToString(k2, 2).PadLeft(Simulator.BitsLength, '0'));
                Console.WriteLine(output + " " + Convert.ToString(k2_estimation, 2).PadLeft(Simulator.BitsLength, '0'));
            }
        }

        private void IdEstimation(int s)
        {
            string output = "ID estimation:";
            id_estimation = 0;
            int[] operations = 
            {
                (int)(~(e ^ f) & 0xFF), 
                a ^ b ^ e, 
                a ^ d ^ e, 
                a ^ e ^ f, 
                b ^ d ^ e, 
                d ^ e ^ f, 
                (int)(~(a ^ b ^ d ^ e) & 0xFF), 
                a ^ d ^ e ^ f, 
                (int)(~(b ^ d ^ e ^ f) & 0xFF)
            };

            for (int i = 0; i < Simulator.BitsLength; i++)
            {
                foreach (int op in operations)
                {
                    int aux = op / (1 << i);
                    int value = aux % 2;
                    id_list[i].Add(value);
                }
                id_estimation += (int)Median(id_list[i]) * (1 << i);
            }

            id_estimation_list.Add(id_estimation);
            if ((sessions - 1) == s)
            {
                Console.WriteLine("ID value:     " + Convert.ToString(id, 2).PadLeft(Simulator.BitsLength, '0'));
                Console.WriteLine(output + " " + Convert.ToString(id_estimation, 2).PadLeft(Simulator.BitsLength, '0'));
            }
        }

        private int HammingDistance(int a, int b)
        {
            int res = 0;
            for (int i = 0; i < Simulator.BitsLength; i++)
            {
                int aux = (a ^ b) / (1 << i);
                int value = aux % 2;
                res += value;
            }
            return res;
        }

        public (List<int>, List<int>, List<int>) GetData()
        {
            var k1_distances = new List<int>();
            for (int i = 0; i < sessions; i++)
            {
                k1_distances.Add(HammingDistance(k1, k1_estimation_list[i]));
            }

            var k2_distances = new List<int>();
            for (int i = 0; i < sessions; i++)
            {
                k2_distances.Add(HammingDistance(k2, k2_estimation_list[i]));
            }

            var id_distances = new List<int>();
            for (int i = 0; i < sessions; i++)
            {
                id_distances.Add(HammingDistance(id, id_estimation_list[i]));
            }

            return (k1_distances, k2_distances, id_distances);
        }

        private double Median(List<int> source)
        {
            if (source == null || source.Count == 0)
                throw new InvalidOperationException("Cannot compute median for a null or empty list.");

            var sortedList = source.OrderBy(n => n).ToList();
            int count = sortedList.Count;
            double medianValue = (count % 2 == 0) ? (sortedList[count / 2 - 1] + sortedList[count / 2]) / 2.0 : sortedList[count / 2];
            return medianValue;
        }
    }
}