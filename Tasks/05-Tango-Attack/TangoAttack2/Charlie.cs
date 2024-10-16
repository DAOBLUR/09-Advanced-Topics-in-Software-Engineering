using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangoAttack2
{
    public class Charlie
    {
        private int id, k1, k2, loop;
        private int L = 8;
        private List<int>[] k1List;
        private List<int>[] k2List;
        private List<int>[] idList;
        private int k1Estimation, k2Estimation, idEstimation;
        private int a, b, d, e, f;

        public Charlie(int id, int k1, int k2, int loop)
        {
            this.id = id;
            this.k1 = k1;
            this.k2 = k2;
            this.loop = loop;

            k1List = new List<int>[L];
            k2List = new List<int>[L];
            idList = new List<int>[L];

            for (int i = 0; i < L; i++)
            {
                k1List[i] = new List<int>();
                k2List[i] = new List<int>();
                idList[i] = new List<int>();
            }

            k1Estimation = 0;
            k2Estimation = 0;
            idEstimation = 0;
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
            IDEstimation(l);
        }

        private void K1Estimation(int l)
        {
            string output = "K1 estimation:";
            k1Estimation = 0;
            int[] operations = { d, f, a ^ d, a ^ b ^ f, ~b ^ d, b ^ f, a ^ b ^ d };

            for (int i = 0; i < L; i++)
            {
                for (int j = 0; j < operations.Length; j++)
                {
                    int aux = operations[j] / (1 << i);
                    int value = aux % 2;
                    k1List[i].Add(value);
                }

                k1Estimation += (int)Math.Round(k1List[i].Median()) * (1 << i);
            }

            if ((loop - 1) == l)
            {
                Console.WriteLine($"K1 value:     {Convert.ToString(k1, 2).PadLeft(8, '0')}");
                Console.WriteLine($"{output} {Convert.ToString(k1Estimation, 2).PadLeft(8, '0')}");
            }
        }

        private void K2Estimation(int l)
        {
            string output = "K2 estimation:";
            k2Estimation = 0;
            int[] operations = { d, f, ~(a ^ d), a ^ f, b ^ d, ~(b ^ f), a ^ b ^ f, a ^ b ^ d };

            for (int i = 0; i < L; i++)
            {
                for (int j = 0; j < operations.Length; j++)
                {
                    int aux = operations[j] / (1 << i);
                    int value = aux % 2;
                    k2List[i].Add(value);
                }

                k2Estimation += (int)Math.Round(k2List[i].Median()) * (1 << i);
            }

            if ((loop - 1) == l)
            {
                Console.WriteLine($"K2 value:     {Convert.ToString(k2, 2).PadLeft(8, '0')}");
                Console.WriteLine($"{output} {Convert.ToString(k2Estimation, 2).PadLeft(8, '0')}");
            }
        }

        public int HammingDistance(int a, int b)
        {
            int res = 0;
            for (int i = 0; i < L; i++)
            {
                int aux = (a ^ b) / (1 << i);
                int value = aux % 2;
                res += value;
            }
            return res;
        }

        public void PrintPlots()
        {
            List<int> k1Distances = new List<int>();
            for (int i = 0; i < loop; i++)
            {
                k1Distances.Add(HammingDistance(k1, k1Estimation));
            }

            // Aquí en lugar de utilizar matplotlib (que no está disponible en C#), puedes usar una biblioteca de gráficos como OxyPlot o ScottPlot para generar el gráfico.
            Console.WriteLine("Gráfico no implementado. Usar una biblioteca de gráficos para mostrar.");
        }

        private void IDEstimation(int l)
        {
            string output = "ID estimation:";
            idEstimation = 0;
            int[] operations = { ~e ^ f, a ^ b ^ e, a ^ d ^ e, a ^ e ^ f, b ^ d ^ e, d ^ e ^ f, ~a ^ b ^ d ^ e, a ^ d ^ e ^ f, ~b ^ d ^ e ^ f };

            for (int i = 0; i < L; i++)
            {
                for (int j = 0; j < operations.Length; j++)
                {
                    int aux = operations[j] / (1 << i);
                    int value = aux % 2;
                    idList[i].Add(value);
                }

                idEstimation += (int)Math.Round(idList[i].Median()) * (1 << i);
            }

            if ((loop - 1) == l)
            {
                Console.WriteLine($"ID value:     {Convert.ToString(id, 2).PadLeft(8, '0')}");
                Console.WriteLine($"{output} {Convert.ToString(idEstimation, 2).PadLeft(8, '0')}");
            }
        }
    }

    // Extensión para calcular la mediana de una lista en C#
    public static class ListExtensions
    {
        public static double Median(this List<int> source)
        {
            var sortedList = source.OrderBy(n => n).ToList();
            int count = sortedList.Count;
            if (count % 2 == 0)
            {
                return (sortedList[count / 2 - 1] + sortedList[count / 2]) / 2.0;
            }
            else
            {
                return sortedList[count / 2];
            }
        }
    }
}