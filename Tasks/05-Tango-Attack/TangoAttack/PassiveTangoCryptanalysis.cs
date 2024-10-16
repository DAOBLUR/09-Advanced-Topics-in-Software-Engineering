
namespace TangoAttack
{
    public class PassiveTangoCryptanalysis
    {
        const double GAMMA = 4.5;

        public int E0 { get; set; }
        public int F0 { get; set; }
        public int BitLength { get; set; }

        public List<List<int>> HammingDistances = new List<List<int>>();

        public PassiveTangoCryptanalysis(int e0, int f0, int bitLength)
        {
            E0 = e0;
            F0 = f0;
            BitLength = bitLength;
        }

        public bool HammingDistance(int E1, int F1)
        {

            int distance1 = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((E0 & (1 << i)) != (E1 & (1 << i)))
                {
                    distance1++;
                }
            }

            int distance2 = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((F0 & (1 << i)) != (F1 & (1 << i)))
                {
                    distance2++;
                }
            }

            var percentaje1 = (distance1 * 100) / BitLength;
            var percentaje2 = (distance2 * 100) / BitLength;

            if(percentaje1 < 10 && percentaje2 < 10)
            {
                /*
                Console.WriteLine($"Distancia de Hamming entre E0 y E1: {distance1} ({percentaje1}%)");
                Console.WriteLine($"Distancia de Hamming entre F0 y F1: {distance2} ({percentaje2}%)");
                */
                HammingDistances.Add(new List<int> { distance1, distance2 });

                E0 = E1;
                F0 = F1;

                return true;
            }

            //Console.WriteLine($"Distancia de Hamming entre E0 y E1: {distance1}");
            //Console.WriteLine($"Distancia de Hamming entre F0 y F1: {distance2}");

            E0 = E1;
            F0 = F1;

            return false;
        }

        public static List<List<char>> data = new();

        public static void Session(int PID2, int A, int B, int D, int E, int F)
        {
            /*
            (E⊕F)
            (A⊕B⊕E)
            (A⊕D⊕E)
            (A⊕E⊕F)
            (B⊕D⊕E)
            (D⊕E⊕F)
            (A⊕B⊕D⊕E)
            (A⊕D⊕E⊕F)
            (B⊕D⊕E⊕F)
            */

            /*
            Console.WriteLine($"E ^ F  = {E ^ F} {Convert.ToString(E ^ F, 2)}");
            Console.WriteLine($"A ^ B ^ E  = {A ^ B ^ E} {Convert.ToString(A ^ B ^ E, 2)}");
            Console.WriteLine($"A ^ D ^ E  = {A ^ D ^ E} {Convert.ToString(A ^ D ^ E, 2)}");
            Console.WriteLine($"A ^ E ^ F  = {A ^ E ^ F} {Convert.ToString(A ^ E ^ F, 2)}");
            Console.WriteLine($"B ^ D ^ E  = {B ^ D ^ E} {Convert.ToString(B ^ D ^ E, 2)}");
            Console.WriteLine($"D ^ E ^ F  = {D ^ E ^ F} {Convert.ToString(D ^ E ^ F, 2)}");
            Console.WriteLine($"A ^ B ^ D ^ E  = {A ^ B ^ D ^ E} {Convert.ToString(A ^ B ^ D ^ E, 2)}");
            Console.WriteLine($"A ^ D ^ E ^ F  = {A ^ D ^ E ^ F} {Convert.ToString(A ^ D ^ E ^ F, 2)}");
            Console.WriteLine($"B ^ D ^ E ^ F  = {B ^ D ^ E ^ F} {Convert.ToString(B ^ D ^ E ^ F, 2)}");
            */

            var values = new List<int>
            {
                E ^ F,
                A ^ B ^ E,
                A ^ D ^ E,
                A ^ E ^ F,
                B ^ D ^ E,
                D ^ E ^ F,
                A ^ B ^ D ^ E,
                A ^ D ^ E ^ F,
                B ^ D ^ E ^ F
            };

            foreach (var item in values)
            {
                var binaryResult = Convert.ToString(item, 2);
                var list = binaryResult.ToList();

                data.Add(list);

            }
        }

        public void PrintResults(int sessions)
        {
            Console.WriteLine("\n--- Resultados del ataque Tango ---");
            Console.WriteLine($"Número de sesiones analizadas: {HammingDistances.Count}");

            if(HammingDistances.Count == 0)
            {
                Console.WriteLine("No se encontraron buenas aproximaciones.");
                return;
            }

            Console.WriteLine("Distancias de Hamming:");

            foreach (var session in HammingDistances)
            {
                Console.WriteLine($"E: {session[0]}, F: {session[1]}");
            }

            List<int> verticalSum = new();

            foreach(var item in data)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    if (verticalSum.Count <= i)
                    {
                        verticalSum.Add(0);
                    }

                    if (item[i] == '1')
                    {
                        verticalSum[i]++;
                    }
                }
            }

            string result = string.Empty;

            foreach (var item in verticalSum)
            {
                if (item >= GAMMA * sessions)
                {
                    result += "1";
                }
                else
                {
                    result += "0";
                }
            }

            Console.WriteLine($"Resultado del ataque: {result} - {Convert.ToInt32(result, 2)}");

            result = string.Empty;

            foreach (var item in verticalSum)
            {
                if (item >= GAMMA * HammingDistances.Count)
                {
                    result += "1";
                }
                else
                {
                    result += "0";
                }
            }

            Console.WriteLine($"Resultado del ataque (Goods): {result} - {Convert.ToInt32(result, 2)}");


            result = string.Empty;

            foreach (var item in verticalSum)
            {
                if (item >= sessions * 0.5 * HammingDistances.Count)
                {
                    result += "1";
                }
                else
                {
                    result += "0";
                }
            }

            Console.WriteLine($"Resultado del ataque (Goods): {result} - {Convert.ToInt32(result, 2)}");

            // ANALIZAR
            //Número de sesiones para el descifrado * 0.5 * número de buenas aproximaciones
        }
    }
}