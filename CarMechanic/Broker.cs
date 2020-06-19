using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CarMechanic
{
    class Broker
    {
        public List<Mechanik> listaMechanikow = new List<Mechanik>();

        // public List<Wiadomosc> listaOdpowiedzi = new List<Wiadomosc>();
        public List<Klient> listaKlientow = new List<Klient>();

        // public List<Oferty> listaOfert = new List<Oferty>();
        public Dictionary<int, List<Oferty>> dziennikOfert = new Dictionary<int, List<Oferty>>();

        public static BlockingCollection<Wiadomosc> kolejka =
            new BlockingCollection<Wiadomosc>(boundedCapacity: 75);


        private int iloscOfert = 0;


        internal class Oferty
        {
            public int idKlient { get; set; }
            public int idMechanik { get; set; }
            public int poziomTrudnosci { get; set; }
            public double cena { get; set; }
            public double jakoscNaprawy { get; set; }
            public Zdarzenie zlecenie { get; set; }

            //1- Jakosc
            //2 - cena
            //3 - wysrodkowane
            public int priorytetNaprawczy { get; set; }
        }

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
                        szukaj(w);

                        break;
                    }
                    case Zdarzenie.ofertaNaprawy:
                    {
                        ofertaNaprawy(w);

                        break;
                    }
                    case Zdarzenie.napraw:
                    {
                        napraw(w);
                        break;
                    }
                    case Zdarzenie.ukonczonoZlecenie:
                    {
                        ukonczono(w);
                        break;
                    }
                }
            }
        }


        public void dodajZlecenie(Wiadomosc w)
        {
            kolejka.Add(w);
        }

        public void ukonczono(Wiadomosc w)
        {
            for (int i = 0; i < listaKlientow.Count; i++)
            {
                if (listaKlientow[i].IdKlient == w.idOdbiorcy)
                {
                    Wiadomosc nW = new Wiadomosc();
                    nW.zlecenie = Zdarzenie.ukonczonoZlecenie;
                    listaKlientow[i].dodajWiadomosc(nW);
                    break;
                }
            }
        }

        public void szukaj(Wiadomosc w)
        {
            string priorytetNaprawczy = "";

            //1- Jakosc
            //2 - cena
            //3 - wysrodkowane
            if (w.priorytetNaprawczy.Equals(1))
            {
                priorytetNaprawczy = "JAKOŚĆ";
            }
            else if (w.priorytetNaprawczy.Equals(2))
            {
                priorytetNaprawczy = "CENA";
            }
            else if (w.priorytetNaprawczy.Equals(3))
            {
                priorytetNaprawczy = "BALANS";
            }

            Console.WriteLine("Klient " + w.idNadawca + " zleca naprawę, trudność zadania: " +
                              w.poziomTrudnosci + "\n Priorytet: " + priorytetNaprawczy + "\n");

            for (int i = 0; i < listaMechanikow.Count; i++)
            {
                // Wiadomosc nW = new Wiadomosc(w.idNadawca, w.poziomTrudnosci, w.priorytetNaprawczy);

                Wiadomosc nW = new Wiadomosc();

                nW.idNadawca = w.idNadawca;
                nW.poziomTrudnosci = w.poziomTrudnosci;
                nW.priorytetNaprawczy = w.priorytetNaprawczy;
                nW.zlecenie = Zdarzenie.ofertaNaprawy;

                listaMechanikow[i].dodajZlecenie(nW);
            }
        }

        public void napraw(Wiadomosc w)
        {
            Console.WriteLine("Klient " + w.idNadawca + " zleca naprawę, mechanikowi: " +
                              w.idOdbiorcy + "\n Jakość naprawy: " + w.jakoscNaprawy + "\n Cena: " + w.cena + " zł " +
                              "\n Poziom trudności: " + w.poziomTrudnosci + "\n Priorytety: " + w.priorytetNaprawczy +
                              "\n");

            for (int i = 0; i < listaMechanikow.Count; i++)
            {
                if (listaMechanikow[i].idMechanika == w.idOdbiorcy)
                {
                    // Wiadomosc nW = new Wiadomosc(w.idNadawca, w.idOdbiorcy, w.poziomTrudnosci, w.cena, w.jakoscNaprawy, w.priorytetNaprawczy);

                    Wiadomosc nW = new Wiadomosc();

                    nW.idNadawca = w.idNadawca;
                    nW.idOdbiorcy = w.idOdbiorcy;
                    nW.poziomTrudnosci = w.poziomTrudnosci;
                    nW.cena = w.cena;
                    nW.jakoscNaprawy = w.jakoscNaprawy;
                    nW.priorytetNaprawczy = w.priorytetNaprawczy;
                    nW.zlecenie = Zdarzenie.napraw;

                    listaMechanikow[i].dodajZlecenie(nW);
                    break;
                }
            }
        }

        public void ofertaNaprawy(Wiadomosc w)
        {
            Console.WriteLine("\nMechanik " + w.idNadawca + " przedstawia swoją ofertę \n Cena: " + w.cena + " zł " +
                              " \n Jakość naprawy: " +
                              w.jakoscNaprawy + "\n Dla klienta:" + w.idOdbiorcy + "\n");

            Oferty of = new Oferty();
            of.idKlient = w.idOdbiorcy;
            of.idMechanik = w.idNadawca;
            of.cena = w.cena;
            of.jakoscNaprawy = w.jakoscNaprawy;
            of.priorytetNaprawczy = w.priorytetNaprawczy;
            of.poziomTrudnosci = w.poziomTrudnosci;


            if (!dziennikOfert.ContainsKey(w.idOdbiorcy))
            {
                List<Oferty> o = new List<Oferty>();
                dziennikOfert.Add(w.idOdbiorcy, o);
            }

            
            dziennikOfert[w.idOdbiorcy].Add(of);


            if (dziennikOfert[w.idOdbiorcy].Count == listaMechanikow.Count)
            {
                szacujOferty(dziennikOfert[w.idOdbiorcy]);
                dziennikOfert[w.idOdbiorcy].Clear();
            }
        }

        private void szacujOferty(List<Oferty> listaOdpowiedzi)
        {
            //Priorytet naprawczy

            //PRIORYTET JAKOSC
            if (listaOdpowiedzi[0].priorytetNaprawczy == 1)
            {
                double _jakoscNaprawy = listaOdpowiedzi[0].jakoscNaprawy;
                double _cena = listaOdpowiedzi[0].cena;
                int _index = 0;

                for (int i = 1; i < listaMechanikow.Count; i++)
                {
                    if (listaOdpowiedzi[i].jakoscNaprawy > _jakoscNaprawy)
                    {
                        _jakoscNaprawy = listaOdpowiedzi[i].jakoscNaprawy;
                        _cena = listaOdpowiedzi[i].cena;
                        _index = i;
                    }
                    else if (listaOdpowiedzi[i].jakoscNaprawy == _jakoscNaprawy)
                    {
                        if (listaOdpowiedzi[i].cena < _cena)
                        {
                            _jakoscNaprawy = listaOdpowiedzi[i].jakoscNaprawy;
                            _cena = listaOdpowiedzi[i].cena;
                            _index = i;
                        }
                    }
                }

                Wiadomosc nW = new Wiadomosc(
                    listaOdpowiedzi[_index].idMechanik,
                    listaOdpowiedzi[_index].idKlient,
                    listaOdpowiedzi[_index].poziomTrudnosci,
                    listaOdpowiedzi[_index].cena,
                    _jakoscNaprawy,
                    listaOdpowiedzi[_index].priorytetNaprawczy);

                nW.zlecenie = Zdarzenie.znaleziono;

                for (int i = 0; i < listaKlientow.Count; i++)
                {
                    if (listaKlientow[i].IdKlient == listaOdpowiedzi[_index].idKlient)
                    {
                        listaKlientow[i].dodajWiadomosc(nW);
                        break;
                    }
                }
            }
            // PRIORYTET CENA
            else if (listaOdpowiedzi[0].priorytetNaprawczy == 2)
            {
                double _cenaNaprawy = listaOdpowiedzi[0].cena;
                int _index = 0;

                for (int i = 1; i < listaMechanikow.Count; i++)
                {
                    if (listaOdpowiedzi[i].cena < _cenaNaprawy)
                    {
                        _cenaNaprawy = listaOdpowiedzi[i].cena;
                        _index = i;
                    }
                }

                Wiadomosc nW = new Wiadomosc(
                    listaOdpowiedzi[_index].idMechanik,
                    listaOdpowiedzi[_index].idKlient,
                    listaOdpowiedzi[_index].poziomTrudnosci,
                    _cenaNaprawy,
                    listaOdpowiedzi[_index].jakoscNaprawy,
                    listaOdpowiedzi[_index].priorytetNaprawczy);

                nW.zlecenie = Zdarzenie.znaleziono;

                for (int i = 0; i < listaKlientow.Count; i++)
                {
                    if (listaKlientow[i].IdKlient == listaOdpowiedzi[_index].idKlient)
                    {
                        listaKlientow[i].dodajWiadomosc(nW);
                        break;
                    }
                }
            }
            //PRIORYTET BALANS
            else if (listaOdpowiedzi[0].priorytetNaprawczy == 3)
            {
                double sredniaCena = 0.0;
                double sredniaJakosc = 0.0;
                //SZUKAM USREDNIONEJ WARTOSCI
                for (int i = 0; i < listaMechanikow.Count; i++)
                {
                    sredniaCena += listaOdpowiedzi[i].cena;
                    sredniaJakosc += listaOdpowiedzi[i].jakoscNaprawy;
                }

                sredniaCena /= listaMechanikow.Count;
                sredniaJakosc /= listaMechanikow.Count;


                double _cenaNaprawy = sredniaCena - listaOdpowiedzi[0].cena;
                double _jakoscNaprawy = sredniaJakosc - listaOdpowiedzi[0].jakoscNaprawy;

                _cenaNaprawy = Math.Round(_cenaNaprawy, 2);
                _jakoscNaprawy = Math.Round(_jakoscNaprawy, 2);

                if (_cenaNaprawy < 0)
                {
                    _cenaNaprawy *= -1;
                }

                if (_jakoscNaprawy < 0)
                {
                    _jakoscNaprawy *= -1;
                }

                int _index = 0;


                double tmpCena = 0.0;
                double tmpJakosc = 0.0;
                for (int i = 0; i < listaMechanikow.Count; i++)
                {
                    tmpCena = sredniaCena - listaOdpowiedzi[i].cena;
                    tmpJakosc = sredniaJakosc - listaOdpowiedzi[i].jakoscNaprawy;

                    tmpCena = Math.Round(tmpCena, 2);
                    tmpJakosc = Math.Round(tmpJakosc, 2);

                    if (tmpCena < 0)
                        tmpCena *= -1;
                    if (tmpJakosc < 0)
                        tmpJakosc *= -1;

                    if (_cenaNaprawy > tmpCena && _jakoscNaprawy > tmpJakosc)
                    {
                        _cenaNaprawy = tmpCena;
                        _jakoscNaprawy = tmpJakosc;
                        _index = i;
                    }
                }

                Wiadomosc nW = new Wiadomosc(
                    listaOdpowiedzi[_index].idMechanik,
                    listaOdpowiedzi[_index].idKlient,
                    listaOdpowiedzi[_index].poziomTrudnosci,
                    listaOdpowiedzi[_index].cena,
                    listaOdpowiedzi[_index].jakoscNaprawy,
                    listaOdpowiedzi[_index].priorytetNaprawczy);

                nW.zlecenie = Zdarzenie.znaleziono;

                for (int i = 0; i < listaKlientow.Count; i++)
                {
                    if (listaKlientow[i].IdKlient == listaOdpowiedzi[_index].idKlient)
                    {
                        listaKlientow[i].dodajWiadomosc(nW);
                        break;
                    }
                }
            }
        }
    }
}