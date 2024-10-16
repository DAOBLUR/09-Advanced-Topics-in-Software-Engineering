using System;
using System.Collections.Generic;

namespace TangoAttack
{
    static class Program
    {
        // Funciones XOR y AND
        static int XOR(int a, int b) => a ^ b;
        static int AND(int a, int b) => a & b;

        // Método para calcular XOR de tres valores
        static int XOR(int a, int b, int c) => XOR(XOR(a, b), c);

        // Método para generar mensajes A, B, D, E, F del protocolo David-Prasad
        static (int A, int B, int D, int E, int F) GenerateMessages(int PID2, int K1, int K2, int n1, int n2)
        {
            int A = XOR(AND(PID2, AND(K1, K2)), n1);
            int B = XOR(AND(PID2, AND(K2, K1)), n2);
            int D = XOR(AND(K1, n2), AND(K2, n1));
            int E = XOR(XOR(K1, n1), AND(K2, n2));
            int F = XOR(AND(K1, n1), AND(K2, n2));

            return (A, B, D, E, F);
        }

        // Obtener buenas aproximaciones para el ataque Tango
        static List<int> GetGoodApproximations(int A, int B, int D, int E, int F)
        {
            var approximations = new List<int>
            {
                XOR(A, D), XOR(A, F), XOR(B, D), XOR(B, F), 
                XOR(A, B, D), XOR(A, B, F), XOR(E, F), 
                XOR(A, B, E), XOR(A, D, E), XOR(A, E, F),
                XOR(B, D, E), XOR(D, E, F)
            };
            return approximations;
        }

        // Combinación de múltiples aproximaciones
        static int CombineApproximations(List<int> approximations, int threshold)
        {
            int combinedValue = 0;
            foreach (var approx in approximations)
            {
                combinedValue += (approx >= threshold) ? 1 : 0;
            }
            return combinedValue;
        }

        static void RunTangoAttack(int bitLength)
        {
            int maxValue = (1 << bitLength) - 1; // Máximo valor para el número de bits

            // Inicializar los secretos y pseudónimos
            Random random = new Random();
            int PID2 = random.Next(0, maxValue);
            int K1 = random.Next(0, maxValue);
            int K2 = random.Next(0, maxValue);
            int n1 = random.Next(0, maxValue);
            int n2 = random.Next(0, maxValue);

            // Generar los mensajes según el protocolo David-Prasad
            var (A, B, D, E, F) = GenerateMessages(PID2, K1, K2, n1, n2);

            // Mostrar los mensajes generados
            Console.WriteLine($"\n--- Simulación para {bitLength} bits ---");
            Console.WriteLine($"A: {Convert.ToString(A, 2).PadLeft(bitLength, '0')}");
            Console.WriteLine($"B: {Convert.ToString(B, 2).PadLeft(bitLength, '0')}");
            Console.WriteLine($"D: {Convert.ToString(D, 2).PadLeft(bitLength, '0')}");
            Console.WriteLine($"E: {Convert.ToString(E, 2).PadLeft(bitLength, '0')}");
            Console.WriteLine($"F: {Convert.ToString(F, 2).PadLeft(bitLength, '0')}");

            // Obtener aproximaciones buenas (Good Approximations - GA)
            var approximations = GetGoodApproximations(A, B, D, E, F);

            // Combinar las aproximaciones usando un umbral
            int threshold = approximations.Count / 2; // Umbral para decidir el valor final
            int combinedValue = CombineApproximations(approximations, threshold);

            Console.WriteLine($"Valor combinado de las aproximaciones: {Convert.ToString(combinedValue, 2).PadLeft(bitLength, '0')}");

            // Validar si la clave secreta K1 ha sido aproximada correctamente
            if (combinedValue == K1)
            {
                Console.WriteLine("Aproximación correcta de K1.");
            }
            else
            {
                Console.WriteLine("Fallo en la aproximación de K1.");
            }
        }

        static void Main(string[] args)
        {
            int ID = 0x52;
            Console.WriteLine($"ID: {ID}");
            /*
            for (int bitLength = 4; bitLength <= 8; bitLength++)
            {
                RunTangoAttack(bitLength);
            }
            */

            RunTangoAttack(4);
        }
    }
}
