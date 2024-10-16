namespace TangoAttack
{
    public class Reader
    {
        public int Certificate { get; set; }

        public Reader()
        {
            Certificate = -1;
        }

        // Solicita un certificado del servidor
        public bool RequestCertificate(Server server)
        {
            if (server.AuthorizeReader())
            {
                Certificate = 1; // Certificado válido
                //Console.WriteLine("Certificado de autorización recibido.");
                return true;
            }
            else
            {
                //Console.WriteLine("Solicitud de certificado fallida.");
                return false;
            }
        }

        // Paso 4: Genera n1, n2 y calcula A, B, D
        public (int A, int B, int D) GenerateMessages(int PID2, int K1, int K2, int n1, int n2)
        {
            int A = (PID2 & K1 & K2) ^ n1;
            int B = (PID2 & K2 & K1) ^ n2;
            int D = (K1 & n2) ^ (K2 & n1);
            
            //Console.WriteLine($"Lector generó A: {Convert.ToString(A, 2)}, B: {Convert.ToString(B, 2)}, D: {Convert.ToString(D, 2)}");
            return (A, B, D);
        }

        // Paso 6: Verifica F y recupera el ID
        public int VerifyTagResponse(int E, int F, int K1, int K2, int n1, int n2)
        {
            int calculatedF = (K1 & n1) ^ (K2 & n2);

            if (calculatedF != F)
            {
                throw new Exception("Error en la autenticación del tag: F no coincide.");
            }

            //Console.WriteLine("Autenticación del tag exitosa.");

            // Recuperar ID
            int ID = E ^ (K2 & n2) ^ K1 ^ n1;
            //Console.WriteLine($"ID del tag recuperado: {Convert.ToString(ID, 2)}");
            return ID;
        }
    }
}