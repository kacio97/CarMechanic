using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMechanic
{
    class Broker
    {
        public List<Mechanik> listaMechanikow = new List<Mechanik>();
        public List<Wiadomosc> listaOdpowiedzi = new List<Wiadomosc>();
        public List<Klient> listaKlientow = new List<Klient>();

        public static BlockingCollection<Wiadomosc> kolejka =
            new BlockingCollection<Wiadomosc>(new ConcurrentQueue<Wiadomosc>(), boundedCapacity:15);



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
                }
            }
        }


        public void dodajZlecenie(Wiadomosc w)
        {
            kolejka.Add(w);
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
                              w.poziomTrudnosci + " Priorytet: " + priorytetNaprawczy);

            for (int i = 0; i < listaMechanikow.Count; i++)
            {
                Wiadomosc nW = new Wiadomosc(w.idNadawca, w.poziomTrudnosci, w.priorytetNaprawczy);
                nW.zlecenie = Zdarzenie.ofertaNaprawy;
                listaMechanikow[i].dodajZlecenie(nW);
            }
        }

        public void napraw(Wiadomosc w)
        {
            Console.WriteLine("Klient " + w.idNadawca + " zleca naprawę, mechanikowi: " +
                              w.idOdbiorcy + " Jakosc naprawy " + w.jakoscNaprawy + " Cena: " + w.cena + " poziom trudnosci " + w.poziomTrudnosci);

            for (int i = 0; i < listaMechanikow.Count; i++)
            {
                if (listaMechanikow[i].idMechanika == w.idOdbiorcy)
                {
                    Wiadomosc nW = new Wiadomosc(w.idNadawca, w.idOdbiorcy, w.poziomTrudnosci, w.cena, w.jakoscNaprawy, w.priorytetNaprawczy);
                    nW.zlecenie = Zdarzenie.napraw;
                    listaMechanikow[i].dodajZlecenie(nW);
                    break;
                }
            }
        }

        public void ofertaNaprawy(Wiadomosc w)
        {
            Console.WriteLine("\nMechanik " + w.idNadawca + " przedstawia swoja oferte: \n cena:" + w.cena +
                              " \n jakosc naprawy: " +
                              w.jakoscNaprawy + "\n Dla klienta " + w.idOdbiorcy);

            //TODO ILOSC OFERT I LISTA ODPOWIEDZI PER CLIENT, MOZNA ZROBIC KLASE WEWNETRZNA LUB POLA DLA KLIENTA
            iloscOfert++;
            listaOdpowiedzi.Add(w);
            if (iloscOfert == listaMechanikow.Count)
            {
                //Priorytet naprawczy

                //PRIORYTET JAKOSC
                if (w.priorytetNaprawczy == 1)
                {
                    double _jakoscNaprawy = listaOdpowiedzi[0].jakoscNaprawy;
                    int _index = 0;

                    for (int i = 1; i < listaMechanikow.Count; i++)
                    {
                        if (listaOdpowiedzi[i].jakoscNaprawy > _jakoscNaprawy)
                        {
                            _jakoscNaprawy = listaOdpowiedzi[i].jakoscNaprawy;
                            _index = i;
                        }
                    }

                    Wiadomosc nW = new Wiadomosc(
                        listaOdpowiedzi[_index].idNadawca,
                        listaOdpowiedzi[_index].idOdbiorcy,
                        listaOdpowiedzi[_index].poziomTrudnosci,
                        listaOdpowiedzi[_index].cena,
                        _jakoscNaprawy,
                        listaOdpowiedzi[_index].priorytetNaprawczy);

                    nW.zlecenie = Zdarzenie.znaleziono;

                    for (int i = 0; i < listaKlientow.Count; i++)
                    {
                        if (listaKlientow[i].IdKlient == listaOdpowiedzi[_index].idOdbiorcy)
                        {
                            listaKlientow[i].dodajWiadomosc(nW);
                            break;
                        }
                    }
                }
                // PRIORYTET CENA
                else if (w.priorytetNaprawczy == 2)
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
                        listaOdpowiedzi[_index].idNadawca,
                        listaOdpowiedzi[_index].idOdbiorcy,
                        listaOdpowiedzi[_index].poziomTrudnosci,
                        _cenaNaprawy,
                        listaOdpowiedzi[_index].jakoscNaprawy,
                        listaOdpowiedzi[_index].priorytetNaprawczy);

                    nW.zlecenie = Zdarzenie.znaleziono;

                    for (int i = 0; i < listaKlientow.Count; i++)
                    {
                        if (listaKlientow[i].IdKlient == listaOdpowiedzi[_index].idOdbiorcy)
                        {
                            listaKlientow[i].dodajWiadomosc(nW);
                            break;
                        }
                    }
                }
                //PRIORYTET BALANS
                else if (w.priorytetNaprawczy == 3)
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
                        listaOdpowiedzi[_index].idNadawca,
                        listaOdpowiedzi[_index].idOdbiorcy,
                        listaOdpowiedzi[_index].poziomTrudnosci,
                        listaOdpowiedzi[_index].cena,
                        listaOdpowiedzi[_index].jakoscNaprawy,
                        listaOdpowiedzi[_index].priorytetNaprawczy);

                    nW.zlecenie = Zdarzenie.znaleziono;

                    for (int i = 0; i < listaKlientow.Count; i++)
                    {
                        if (listaKlientow[i].IdKlient == listaOdpowiedzi[_index].idOdbiorcy)
                        {
                            listaKlientow[i].dodajWiadomosc(nW);
                            break;
                        }
                    }
                }

                iloscOfert = 0;
                listaOdpowiedzi.Clear();
            }
        }
    }
}