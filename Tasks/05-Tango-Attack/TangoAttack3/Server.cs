namespace TangoAttack3
{
    public class Server
    {
        private int k1, k2;

        public Server()
        {
            var random = new Random();
            k1 = random.Next(0, (int)Math.Pow(2, Simulator.BitsLength));
            k2 = random.Next(0, (int)Math.Pow(2, Simulator.BitsLength));
        }

        public (int, int) GetK1K2()
        {
            return (k1, k2);
        }
    }
}