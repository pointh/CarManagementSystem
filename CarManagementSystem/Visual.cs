using System;
using System.Collections.Generic;
using System.Text;

namespace CarManagementSystem
{
    class Visual
    {
        RC ridici;
        Meteo met;
        Random rnd;

        public Visual(RC ridiciCentrum, Meteo met)
        {
            ridici = ridiciCentrum;
            this.met = met;
            rnd = new Random();
            met.Zmena += ZmeniloSePocasi;
            for (int i = 0; i < ridici.registr.Length && ridici.registr[i] != null; i++)
            {
                ridici.registr[i].ZmenaStavu += UkazStav;
            }
        }

        public void ZmeniloSePocasi(object sender, PocasiInfo inf)
        {
            switch (inf.pocasi)
            {
                case Pocasi.Mlha:
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                case Pocasi.Mokro:
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case Pocasi.Sucho:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
            }
        }

        public void UkazStav(object sender, AutoInfo ai)
        {

            //Console.Clear();
            for (int i = 0; i<ridici.registr.Length && ridici.registr[i] != null;i++)
            {
                Console.CursorVisible = false;
                Console.CursorLeft = (int)ridici.registr[i].Ujeto / 100;
                Console.CursorTop = i+2;
                Console.ForegroundColor = (ConsoleColor)(i+8);
                switch (ridici.registr[i].stav)
                {
                    case AktualniStavAuta.Most:
                        Console.Write("M");
                        break;
                    case AktualniStavAuta.Trasa:
                        Console.Write("C");
                        break;
                    case AktualniStavAuta.Tunel:
                        Console.Write("T");
                        break;
                    default:
                        Console.Write(" ");
                        break;
                }
            }
        }

    }
}
