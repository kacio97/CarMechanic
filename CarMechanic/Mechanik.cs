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
            new BlockingCollection<Wiadomosc>(new ConcurrentQueue<Wiadomosc>());

        public Mechanik(Broker broker, int idMechanika, string nazwa, double umiejetnosci, double cenaBazowa)
        {
            this.broker = broker;
            this.idMechanika = idMechanika;
            this.nazwa = nazwa;
            this.umiejetnosci = umiejetnosci;
            this.cenaBazowa = cenaBazowa;
        }

        public void odbierajZlecenia()
        {
            while (true)
            {
                Wiadomosc _w;
                _w = listaZlecen.Take();

                switch (_w.zlecenie)
                {
                    case Zdarzenie.ofertaNaprawy:
                    {
                        ocenZlecenie(_w);
                        break;
                    }
                }
            }
        }

        private void ocenZlecenie(Wiadomosc w)
        {
            double jakoscUslugi = umiejetnosci / w.poziomTrudnosci;
            double cena = cenaBazowa + ((cenaBazowa / 10) * w.poziomTrudnosci) + umiejetnosci;
            
            Wiadomosc _w = new Wiadomosc(this.idMechanika, w.idNadawca, cena, jakoscUslugi, w.priorytetNaprawczy);
            _w.zlecenie = Zdarzenie.ofertaNaprawy;
            broker.dodajZlecenie(_w);
        }

        public void dodajZlecenie(Wiadomosc w)
        {
            listaZlecen.Add(w);
        }
    }
}