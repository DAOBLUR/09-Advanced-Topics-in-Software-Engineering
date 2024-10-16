namespace TangoAttack
{
    public class Server
    {
        public bool AuthorizeReader()
        {
            //Console.WriteLine("Autorizando al lector...");
            return true; // Siempre autoriza
        }

        // Devuelve las claves K1 y K2 si el PID2 es válido
        public (int K1, int K2) ProvideKeys(int PID2)
        {
            Random random = new Random();
            int K1 = random.Next(0, 16);
            int K2 = random.Next(0, 16);
            //Console.WriteLine($"Servidor devuelve K1: {Convert.ToString(K1, 2)}, K2: {Convert.ToString(K2, 2)}");
            return (K1, K2);
        }
    }
}