﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Transactions;

namespace CarMechanic
{
    class Klient
    {
        public int IdKlient { get; set; }
        public bool szukam { get; set; }

        private Broker broker;

        public BlockingCollection<Wiadomosc> ListaWiadomosci =
            new BlockingCollection<Wiadomosc>(5);

        private Random rand = new Random();

        public Klient(Broker broker, int idKlient)
        {
            this.broker = broker;
            IdKlient = idKlient;
            szukam = true;

            Thread obsluga = new Thread(odbierajWiadomosci);
            obsluga.Name = "Klient " + this.IdKlient;
            obsluga.Start();
            Thread szukaj = new Thread(szukajMechanika);
            szukaj.Name = "Szukaj " + this.IdKlient;
            szukaj.Start();
        }

        private void szukajMechanika()
        {
            while (true)
            {
                if (!szukam )
                {
                    // Thread.Sleep(5000);
                    continue;
                }
               
                    Wiadomosc w = new Wiadomosc();
                    w.idNadawca = IdKlient;
                    w.poziomTrudnosci = rand.Next(10, 100);
                    w.priorytetNaprawczy = rand.Next(1, 4);
                    w.zlecenie = Zdarzenie.szukaj;
                    broker.dodajZlecenie(w);

                    Wiadomosc wk = new Wiadomosc();
                wk.zlecenie = Zdarzenie.oczekuj;
                dodajWiadomosc(wk);

                Thread.Sleep(rand.Next(10000, 20000));
                

                // Wiadomosc w = new Wiadomosc(this.IdKlient, rand.Next(10, 101), rand.Next(1, 4));

               
            }
        }

        private void setBool(bool b)
        {
            this.szukam = b;
        }

        private void naprawAuto(Wiadomosc w)
        {
            // Wiadomosc nW = new Wiadomosc(this.IdKlient, w.idNadawca, w.poziomTrudnosci, w.cena, w.jakoscNaprawy, w.priorytetNaprawczy);

            Wiadomosc nW = new Wiadomosc();
            nW.idNadawca = this.IdKlient;
            nW.idOdbiorcy = w.idNadawca;
            nW.poziomTrudnosci = w.poziomTrudnosci;
            nW.cena = w.cena;
            nW.jakoscNaprawy = w.jakoscNaprawy;
            nW.priorytetNaprawczy = w.priorytetNaprawczy;
            nW.zlecenie = Zdarzenie.napraw;

            broker.dodajZlecenie(nW);
        }

        public void odbierajWiadomosci()
        {
           
            while (true) {
                Wiadomosc w;
                w = ListaWiadomosci.Take();
                switch (w.zlecenie)
                {
                    case Zdarzenie.znaleziono:
                    {
                        naprawAuto(w);
                        break;
                    }
                    case Zdarzenie.ukonczonoZlecenie:
                    {
                        setBool(true);
                        Console.WriteLine("\nKlient " + IdKlient + " odbiera swoje auto ! \n");
                        break;
                    }
                    case Zdarzenie.oczekuj:
                    {
                        setBool(false);
                        break;
                    }
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