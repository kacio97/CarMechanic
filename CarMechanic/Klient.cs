using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CarMechanic
{
    class Klient
    {
        public int IdKlient { get; set; }
        private Broker broker;

        public BlockingCollection<Wiadomosc> ListaWiadomosci =
            new BlockingCollection<Wiadomosc>(new ConcurrentQueue<Wiadomosc>(), boundedCapacity: 2);

        private Random rand = new Random();

        public Klient(Broker broker, int idKlient)
        {
            this.broker = broker;
            IdKlient = idKlient;

            Thread obsluga = new Thread(odbierajWiadomosci);
            obsluga.Name = "Klient " + this.IdKlient;
            obsluga.Start();
            Thread szukaj = new Thread(szukajMechanika);
            szukaj.Start();
        }

        private void szukajMechanika()
        {
            while (true)
            {
                Wiadomosc w = new Wiadomosc(this.IdKlient, rand.Next(10, 101), rand.Next(1, 4));
                w.zlecenie = Zdarzenie.szukaj;
                broker.dodajZlecenie(w);
                Thread.Sleep(rand.Next(20000, 40000));
            }
        }

        private void naprawAuto(Wiadomosc w)
        {
            Wiadomosc nW = new Wiadomosc(this.IdKlient, w.idNadawca, w.poziomTrudnosci, w.cena, w.jakoscNaprawy, w.priorytetNaprawczy);
            nW.zlecenie = Zdarzenie.napraw;
            broker.dodajZlecenie(nW);
        }

        public void odbierajWiadomosci()
        {
            Wiadomosc w;
            w = ListaWiadomosci.Take();

            switch (w.zlecenie)
            {
                case Zdarzenie.znaleziono:
                {
                    naprawAuto(w);
                    break;
                }
            }

            // Thread.Sleep(rand.Next(10000, 20000));
        }

        public void dodajWiadomosc(Wiadomosc w)
        {
            ListaWiadomosci.Add(w);
        }
    }
}