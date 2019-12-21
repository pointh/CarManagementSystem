using System;
using CarManagementSystem.Properties;

namespace CarManagementSystem
{
    
    class Visual
    {
        readonly RC ridici;

        public Visual(RC ridiciCentrum, Meteo met)
        {
            Auto[] registr = ridiciCentrum.VratPoleRegistrovanychAut();

            ridici = ridiciCentrum;
            met.Zmena += ZmeniloSePocasi;
            for (int i = 0; i < registr.Length && registr[i] != null; i++)
            {
                registr[i].ZmenaStavu += UkazStav;
            }
        }

        public void ZmeniloSePocasi(object sender, PocasiInfo inf)
        {
            switch (inf.Pocasi)
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

        public void UkazStav(object sender, AutoInfoEventArgs ai)
        {

            Auto[] registr = ridici.VratPoleRegistrovanychAut();

            for (int i = 0; i < registr.Length && registr[i] != null; i++)
            {
                Console.CursorVisible = false;
                Console.CursorLeft = (int)registr[i].Ujeto / 100;
                Console.CursorTop = i + 2;
                Console.ForegroundColor = (ConsoleColor)(i + 8);
                switch (registr[i].Stav)
                {
                    case AktualniStavyAuta.Most:
                        Console.Write(Resources.AktualniStavAutaMost);
                        break;
                    case AktualniStavyAuta.Trasa:
                        Console.Write(Resources.AktualniStavAutaTrasa);
                        break;
                    case AktualniStavyAuta.Tunel:
                        Console.Write(Resources.AktualniStavAutaTunel);
                        break;
                    default:
                        Console.Write(Resources.AktualniStavAutaOstatni);
                        break;
                }
            }
        }

    }
}
