namespace TangoAttack
{
    public class Tag
    {
        public int PID { get; set; }
        public int PID2 { get; set; }
        public int ID { get; set; }
        public int K1 { get; set; }
        public int K2 { get; set; }

        public Tag(int pid, int pid2, int id, int k1, int k2)
        {
            PID = pid;
            PID2 = pid2;
            ID = id;
            K1 = k1;
            K2 = k2;
        }

        // Paso 5: La etiqueta verifica D y genera {E, F}
        public (int E, int F) GenerateResponse(int A, int B, int D, int n1, int n2)
        {
            // Verificación de D
            int calculatedD = (K1 & n2) ^ (K2 & n1);

            if (calculatedD != D)
            {
                throw new Exception("Error en la autenticación: D no coincide.");
            }

            //Console.WriteLine("Autenticación del lector exitosa.");

            int E = (K1 ^ n1 ^ ID) ^ (K2 & n2);
            int F = (K1 & n1) ^ (K2 & n2);
            
            //Console.WriteLine($"Etiqueta generó E: {Convert.ToString(E, 2)}, F: {Convert.ToString(F, 2)}");
            return (E, F);
        }

        // Actualización de pseudónimos
        public void UpdatePseudonyms(int n1, int n2)
        {
            PID = PID2;
            PID2 = PID2 ^ n1 ^ n2;
            //Console.WriteLine($"Nuevos pseudónimos en la etiqueta: PID = {Convert.ToString(PID, 2)}, PID2 = {Convert.ToString(PID2, 2)}");
        }
    }
}