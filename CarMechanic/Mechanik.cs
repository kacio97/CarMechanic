using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CarMechanic
{
    class Mechanik
    {
        public int idMechanika { get; set; }
        public string nazwa { get; set; }
        public double umiejetnosci { get; set; }
        public double cenaBazowa { get; set; }

        private Broker broker;

        private BlockingCollection<Wiadomosc> listaZlecen =
            new BlockingCollection<Wiadomosc>(boundedCapacity: 20);

        private Random rand = new Random();

        public Mechanik(Broker broker, int idMechanika, string nazwa, double umiejetnosci, double cenaBazowa)
        {
            this.broker = broker;
            this.idMechanika = idMechanika;
            this.nazwa = nazwa;
            this.umiejetnosci = umiejetnosci;
            this.cenaBazowa = cenaBazowa;

            Thread obsluga = new Thread(odbierajZlecenia);
            obsluga.Name = "Mechanik " + this.idMechanika + " " + this.nazwa;
            obsluga.Start();
        }

        public void odbierajZlecenia()
        {
            while (true)
            {
                Wiadomosc nW;
                nW = listaZlecen.Take();


                switch (nW.zlecenie)
                {
                    case Zdarzenie.ofertaNaprawy:
                    {
                        ocenZlecenie(nW);
                        break;
                    }
                    case Zdarzenie.napraw:
                    {
                        napraw(nW);
                        break;
                    }
                }

                // Thread.Sleep(rand.Next(10000,30000));
            }
        }

        private void napraw(Wiadomosc w)
        {
            double exp = 0d;
            double premiaDoPensji = 0d;

            if (w.cena > 2000 && w.poziomTrudnosci > 50)
            {
                exp = ((w.poziomTrudnosci * w.jakoscNaprawy) / (w.cena / 40));
                premiaDoPensji = w.cena * 0.006;
            }
            else if (w.cena > 1000 && w.poziomTrudnosci > 20)
            {
                exp = ((w.poziomTrudnosci * w.jakoscNaprawy) / (w.cena / 15));
                premiaDoPensji = w.cena * 0.01;
            }
            else if (w.cena <= 1000 && w.poziomTrudnosci < 35)
            {
                exp = ((w.poziomTrudnosci * w.jakoscNaprawy) / (w.cena / 10));
                premiaDoPensji = w.cena * 0.015;
            }
            else
            {
                exp = ((w.poziomTrudnosci * w.jakoscNaprawy) / (w.cena / 8));
                premiaDoPensji = w.cena * 0.02;
            }


            // exp = Math.Round(exp, 2);
            // premiaDoPensji = Math.Round(premiaDoPensji, 2);
            if (this.umiejetnosci >= 100)
            {
                this.umiejetnosci = 100;
                this.cenaBazowa += premiaDoPensji;
            }
            else
            {
                this.umiejetnosci += exp;
                this.cenaBazowa += (premiaDoPensji);
            }

            exp = Math.Round(exp, 2);
            premiaDoPensji = Math.Round(premiaDoPensji, 2);

            int czas = (int)this.umiejetnosci * w.poziomTrudnosci * rand.Next(4,8);

           Thread.Sleep(czas);

            Console.WriteLine("\nMechanik " + this.nazwa + " id " + this.idMechanika + " naprawia auto dla klienta " +
                              w.idNadawca + "\n Zyskuje " + exp + " doświadczenia oraz premię do pensji: " +
                              premiaDoPensji + " zł \n czas pracy: " + czas +
                              " ms\n" +
                              " Całkowite doświadczenie: " + Math.Round(this.umiejetnosci, 2) + " Obecna stawka: " +
                              Math.Round(this.cenaBazowa, 2) + " zł " +
                              "\n");

            Wiadomosc nW = new Wiadomosc();
            
            nW.idOdbiorcy = w.idNadawca;
            nW.zlecenie = Zdarzenie.ukonczonoZlecenie;
            
            broker.dodajZlecenie(nW);
        }

        private void ocenZlecenie(Wiadomosc w)
        {
            //TODO: DLUGOSC OCENY ZLECENIA ZALEZNA OD POZIOMU TRUDNOSCI


            double jakoscUslugi = umiejetnosci / w.poziomTrudnosci;

            if (jakoscUslugi > 1)
                jakoscUslugi = 1.00;

            double cena = cenaBazowa + ((cenaBazowa / 10) * w.poziomTrudnosci) + umiejetnosci;
            jakoscUslugi = Math.Round(jakoscUslugi, 2);
            cena = Math.Round(cena, 2);

            // Wiadomosc nW = new Wiadomosc(this.idMechanika, w.idNadawca, w.poziomTrudnosci, cena, jakoscUslugi, w.priorytetNaprawczy);
            Wiadomosc nW = new Wiadomosc();

            nW.idNadawca = this.idMechanika;
            nW.idOdbiorcy = w.idNadawca;
            nW.poziomTrudnosci = w.poziomTrudnosci;
            nW.cena = cena;
            nW.jakoscNaprawy = jakoscUslugi;
            nW.priorytetNaprawczy = w.priorytetNaprawczy;
            nW.zlecenie = Zdarzenie.ofertaNaprawy;

            broker.dodajZlecenie(nW);
        }

        public void dodajZlecenie(Wiadomosc w)
        {
            listaZlecen.Add(w);
        }
    }
}