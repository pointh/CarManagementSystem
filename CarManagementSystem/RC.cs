using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace CarManagementSystem
{
    public class RC
    {
        Auto[] registr;
        int idxPosledniAuto;

        public RC(int maxAuta)
        {
            registr = new Auto[maxAuta];
        }
        public void Add(Auto a)
        {
            registr[idxPosledniAuto++] = a;
        }

        public void AddTimerToFleet(Timer t)
        {
            foreach (Auto a in registr)
                if (a != null)
                    t.Elapsed += a.Check;
        }

        public void ZmeniloSePocasi(object sender, PocasiInfo inf)
        {
            for (int i = 0; i < registr.Length && registr[i] != null; i++)
            {
                if (inf.teplota < 0)
                    registr[i].SnizRychlost(10);
                if (inf.pocasi == Pocasi.Mlha)
                {
                    registr[i].SnizRychlost(10);
                    registr[i].RozsvitSvetla();
                }
                if (inf.teplota > 0)
                    registr[i].ZvysRychlost(10);
                if (inf.pocasi == Pocasi.Sucho)
                    registr[i].ZvysRychlost(10);
            }
        }

        public void StrategieOpatrna(object sender, AutoInfo ai)
        {
            switch (ai.zmenaNaTrase)
            {
                case AktualniZmenaAuta.MostTrasa:
                    (sender as Auto).aktualniRychlost = (sender as Auto).beznaRychlost;
                    break;
                case AktualniZmenaAuta.TunelTrasa:
                    (sender as Auto).aktualniRychlost = (sender as Auto).beznaRychlost;
                    (sender as Auto).ZhasniSvetla();
                    break;
                case AktualniZmenaAuta.MostTunel:
                    (sender as Auto).RozsvitSvetla();
                    break;
                case AktualniZmenaAuta.TunelMost:
                    (sender as Auto).ZhasniSvetla();
                    break;
                case AktualniZmenaAuta.TrasaMost:
                    (sender as Auto).SnizRychlost(10);
                    break;
                case AktualniZmenaAuta.TrasaTunel:
                    (sender as Auto).SnizRychlost(10);
                    (sender as Auto).RozsvitSvetla();
                    break;
                case AktualniZmenaAuta.StartTrasa:
                    (sender as Auto).aktualniRychlost = (sender as Auto).beznaRychlost;
                    break;
                case AktualniZmenaAuta.TrasaStop:
                    break;
            }
        }

        public void SubscribeMeteo(Meteo m)
        {
            m.Zmena += ZmeniloSePocasi;
        }

        public void AplikujStrategii(ZmenaStavuAuta zmenaStavu)
        {
            for (int i = 0; i < idxPosledniAuto; i++)
            {
                registr[i].ZmenaStavu += zmenaStavu;
            }
        }
    }

}
