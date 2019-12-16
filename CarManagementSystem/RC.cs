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
            Auto a = sender as Auto;

            a.aktualniRychlost = a.defaultConditions[(int)ai.stav].rychlost;
            a.Svetla = a.defaultConditions[(int)ai.stav].svetla;
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
