using System;
using System.Collections.Generic;
using System.Threading;

namespace CarMechanic
{
    class Program
    {
        static void Main(string[] args)
        {
            // List<Thread> threads = new List<Thread>();

            Broker b = new Broker();
            for (int i = 0; i < 10; i++)
            {
                b.listaKlientow.Add(new Klient(b, i + 1));
            }

            b.listaMechanikow.Add(new Mechanik(b, 1, "Mietek", 33, 290));
            b.listaMechanikow.Add(new Mechanik(b, 2, "Jozek", 54, 430));
            b.listaMechanikow.Add(new Mechanik(b, 3, "Benek", 67, 550));
            b.listaMechanikow.Add(new Mechanik(b, 4, "Guslaw", 24, 210));
            b.listaMechanikow.Add(new Mechanik(b, 5, "Dimytri", 49, 350));


            b.odbierajZlecenia();
        }
    }
}