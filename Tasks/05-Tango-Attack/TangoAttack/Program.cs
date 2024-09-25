using System;

namespace DavidPrasadTangoAttack
{
    class Program
    {
        static int EncryptDavidPrasad(int plainText, int key)
        {
            return plainText ^ key;
        }

        static int TangoAttack(int cipherText, int knownPlainText)
        {
            return cipherText ^ knownPlainText;
        }

        static void Main(string[] args)
        {
            
            int a = 5; // 0101 en binario
int result1 = a << 3; // Desplaza 1 posición a la izquierda: 1010 en binario, que es 10 en decimal
int result2 = a << 2; // Desplaza 2 posiciones a la izquierda: 10100 en binario, que es 20 en decimal

Console.WriteLine(result1); // Imprime 10
Console.WriteLine(result2); // Imprime 20

            return;
            
            
            Console.WriteLine("--Tango Attack--");

            
            for (int bitLength = 4; bitLength <= 8; bitLength++)
            {
                int maxValue = (1 << bitLength) - 1;
                Console.WriteLine($"\n--- Simulación para {bitLength} bits ---");

                // Prueba de encriptación y desencriptación usando David-Prasad
                int plainText = new Random().Next(0, maxValue); 
                int key = new Random().Next(0, maxValue);   


                int cipherText = EncryptDavidPrasad(plainText, key);
                Console.WriteLine($"Texto en claro: {Convert.ToString(plainText, 2).PadLeft(bitLength, '0')}");
                Console.WriteLine($"Clave: {Convert.ToString(key, 2).PadLeft(bitLength, '0')}");
                Console.WriteLine($"Texto cifrado: {Convert.ToString(cipherText, 2).PadLeft(bitLength, '0')}");

                // Simulación del Tango Attack
                int recoveredKey = TangoAttack(cipherText, plainText);
                Console.WriteLine($"Clave recuperada por Tango Attack: {Convert.ToString(recoveredKey, 2).PadLeft(bitLength, '0')}");

                // Verificación
                if (recoveredKey == key)
                {
                    Console.WriteLine("Ataque exitoso: Clave correctamente recuperada.");
                }
                else
                {
                    Console.WriteLine("Ataque fallido: Clave incorrecta.");
                }
            }
        }
    }
}
