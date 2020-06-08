using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CarMechanic
{
    class Broker
    {
        public List<Mechanik> listaMechanikow = new List<Mechanik>();
        public List<Wiadomosc> listaOdpowiedzi = new List<Wiadomosc>();

        public static BlockingCollection<Wiadomosc> kolejka =
            new BlockingCollection<Wiadomosc>(new ConcurrentQueue<Wiadomosc>());

        private int iloscOfert = 0;

        public void odbierajZlecenia()
        {
            while (true)
            {
                Wiadomosc w;
                w = kolejka.Take();

                switch (w.zlecenie)
                {
                    case Zdarzenie.szukaj:
                    {
                        Console.WriteLine("Klient " + w.idNadawca + " zleca naprawę, trudność zadania: " +
                                          w.poziomTrudnosci + " Priorytet: " + w.priorytetNaprawczy);
                        for (int i = 0; i < listaMechanikow.Count; i++)
                        {
                            Wiadomosc _w = new Wiadomosc(w.idNadawca, w.poziomTrudnosci, w.priorytetNaprawczy);
                            _w.zlecenie = Zdarzenie.ofertaNaprawy;
                            listaMechanikow[i].dodajZlecenie(_w);
                        }

                        break;
                    }
                    case Zdarzenie.ofertaNaprawy:
                    {
                        Console.WriteLine("Mechanik " + w.idNadawca + " przedstawia swoja oferte: " + w.cena + " " +
                                          w.jakoscNaprawy + " dla klienta " + w.idOdbiorcy);

                        iloscOfert++;
                        listaOdpowiedzi.Add(w);
                        if (iloscOfert == listaMechanikow.Count)
                        {
                            for (int i = 0; i < listaMechanikow.Count; i++)
                            {
                                //TODO: POROWNANIE I WYBOR OFERT DLA KLIENTA
                            }
                        }

                        break;
                    }
                }
            }
        }

        public void dodajZlecenie(Wiadomosc w)
        {
            kolejka.Add(w);
        }
    }
}