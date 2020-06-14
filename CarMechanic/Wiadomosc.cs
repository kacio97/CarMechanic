using System;
using System.Collections.Generic;
using System.Text;

namespace CarMechanic
{
    class Wiadomosc
    {
        public int idNadawca { get; set; }
        //Oblsuga id odbiorcy lub poziom trudnosci
        public int idOdbiorcy { get; set; }
        public int poziomTrudnosci { get; set; }
        public double cena { get; set; }
        public double jakoscNaprawy { get; set; }
        public Zdarzenie zlecenie { get; set; }

        //1- Jakosc
        //2 - cena
        //3 - wysrodkowane
        public int priorytetNaprawczy { get; set; }

        public Wiadomosc() { }



        public Wiadomosc(int idNadawca, int idOdbiorcy)
        {
            this.idNadawca = idNadawca;
            this.idOdbiorcy = idOdbiorcy;
        }

        public Wiadomosc(int idNadawca, int idOdbiorcy, int poziomTrudnosci, int priorytetNaprawczy)
        {
            this.idNadawca = idNadawca;
            this.idOdbiorcy = idOdbiorcy;
            this.poziomTrudnosci = poziomTrudnosci;
            this.priorytetNaprawczy = priorytetNaprawczy;
        }

        public Wiadomosc(int idNadawca, int poziomTrudnosci, int priorytetNaprawczy)
        {
            this.idNadawca = idNadawca;
            this.poziomTrudnosci = poziomTrudnosci;
            this.priorytetNaprawczy = priorytetNaprawczy;
        }

        public Wiadomosc(int idNadawca, int idOdbiorcy, double cena, double jakoscNaprawy)
        {
            this.idNadawca = idNadawca;
            this.idOdbiorcy = idOdbiorcy;
            this.cena = cena;
            this.jakoscNaprawy = jakoscNaprawy;
        }

        public Wiadomosc(int idNadawca, int idOdbiorcy, double cena, double jakoscNaprawy, int priorytetNaprawczy)
        {
            this.idNadawca = idNadawca;
            this.idOdbiorcy = idOdbiorcy;
            this.cena = cena;
            this.jakoscNaprawy = jakoscNaprawy;
            this.priorytetNaprawczy = priorytetNaprawczy;
        }

        public Wiadomosc(int idNadawca, int idOdbiorcy, int poziomTrudnosci, double jakoscNaprawy)
        {
            this.idNadawca = idNadawca;
            this.idOdbiorcy = idOdbiorcy;
            this.poziomTrudnosci = poziomTrudnosci;
            this.jakoscNaprawy = jakoscNaprawy;
        }

        public Wiadomosc(int idNadawca, int idOdbiorcy, int poziomTrudnosci, double cena, double jakoscNaprawy, int priorytetNaprawczy)
        {
            this.idNadawca = idNadawca;
            this.idOdbiorcy = idOdbiorcy;
            this.poziomTrudnosci = poziomTrudnosci;
            this.cena = cena;
            this.jakoscNaprawy = jakoscNaprawy;
            this.priorytetNaprawczy = priorytetNaprawczy;
        }
    }
}
