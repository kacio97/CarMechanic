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
        public BlockingCollection<Wiadomosc> ListaWiadomosci = new BlockingCollection<Wiadomosc>(new ConcurrentQueue<Wiadomosc>());
        private Random rand = new Random();

        public Klient(Broker broker, int idKlient)
        {
            this.broker = broker;
            IdKlient = idKlient;

            Thread obsluga = new Thread(odbierajWiadomosci);
            obsluga.Start();
            Thread szukaj = new Thread(szukajMechanika);
            szukaj.Start();
        }

        private void szukajMechanika()
        {
            while (true)
            {
                Wiadomosc w = new Wiadomosc(this.IdKlient, rand.Next(1, 101), rand.Next(1, 4));
                w.zlecenie = Zdarzenie.szukaj;
                broker.dodajZlecenie(w);
                Thread.Sleep(10000);
            }

        }

        private void naprawAuto(Wiadomosc w)
        {
            Wiadomosc _w = new Wiadomosc(this.IdKlient, w.idNadawca, w.poziomTrudnosci, w.priorytetNaprawczy);
            _w.zlecenie = Zdarzenie.napraw;
            broker.dodajZlecenie(_w);
            Thread.Sleep(10000);
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
        }

        public void dodajWiadomosc(Wiadomosc w)
        {
            ListaWiadomosci.Add(w);
        }
    }
}
