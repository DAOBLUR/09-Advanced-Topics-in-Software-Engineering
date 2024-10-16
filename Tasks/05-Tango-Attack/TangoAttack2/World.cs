using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangoAttack2
{
    public class World
    {
        private Reader reader;
        private Tag tag;
        private Charlie charlie;
        private int loop;

        public World(int loop)
        {
            Random random = new Random();
            int pid = random.Next(0, (int)Math.Pow(2, 8));
            int pid2 = random.Next(0, (int)Math.Pow(2, 8));
            int k1 = random.Next(0, (int)Math.Pow(2, 8));
            int k2 = random.Next(0, (int)Math.Pow(2, 8));
            int identificator = random.Next(0, (int)Math.Pow(2, 8));

            this.loop = loop;
            this.reader = new Reader(pid, pid2, k1, k2); // Reciben lo mismo tag y reader al init
            this.tag = new Tag(identificator, pid, pid2, k1, k2); // (self,id, pid, pid2, k1, k2)
            this.charlie = new Charlie(identificator, k1, k2, loop);
        }

        public void StartSimulation()
        {
            for (int i = 0; i < loop; i++)
            {
                // Generar A, B, D
                int a = reader.GenerateA();
                int b = (int)reader.GenerateB();
                int d = reader.GenerateD();

                // Reader -> Charlie
                charlie.ReceivesABD(a, b, d);

                // Reader -> Tag
                tag.ReceivesABD(a, b, d); // Recibe A, B, D y calcula n1 y n2
                tag.CalculateN1N2(); // Calcular n1 y n2
                tag.CheckN1N2(); // Comprobar resultado con D

                // Generar E y F en Tag
                int e = tag.GenerateE();
                int f = tag.GenerateF();

                // Tag -> Charlie
                charlie.ReceivesEF(e, f);

                // Tag -> Reader
                reader.ReceivesEF(e, f);
                reader.GetID(); // Calcular ID con E
                reader.CheckN1N2(); // Comprobar con F

                // Charlie intenta adivinar k1, k2, ID
                charlie.ComputeApproximation(i);

                // Recalcular pseudónimos y actualizar n1 y n2
                reader.RecalculatePseudonim();
                reader.UpdateN1N2();
                tag.RecalculatePseudonim();
            }

            // Mostrar gráficos de estimación
            charlie.PrintPlots();
        }
    }
}