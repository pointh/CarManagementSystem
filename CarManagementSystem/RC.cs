using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace CarManagementSystem
{
   

    public class RC
    {

        Auto[] registr;
        int idxPosledniAuto;
        ZmenaStavuAuta zmenaStavu;

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
            const double blizkoMrazu = 3.0;
            for (int i = 0; i < registr.Length && registr[i] != null; i++)
            {
                
                if (inf.pocasi == Pocasi.Mlha)
                {
                    if (inf.teplota > blizkoMrazu)
                        registr[i].KorekceRychlostiNaPocasi = -10.0;
                    else
                        registr[i].KorekceRychlostiNaPocasi = -20.0;
                    registr[i].KorekceSvetelNaPocasi = true;
                }
                if (inf.pocasi == Pocasi.Mokro)
                {
                    if (inf.teplota > blizkoMrazu)
                        registr[i].KorekceRychlostiNaPocasi = -10.0;
                    else
                        registr[i].KorekceRychlostiNaPocasi = -30.0;
                    registr[i].KorekceSvetelNaPocasi = false;
                }
                if (inf.pocasi == Pocasi.Sucho)
                {
                    registr[i].KorekceRychlostiNaPocasi = 0.0;
                    registr[i].KorekceSvetelNaPocasi = false;
                }
            }
        }

        public void StrategieOpatrna(object sender, AutoInfo ai)
        {
            Auto a = sender as Auto;

            if (ai != null)
            {
                a.aktualniRychlost = a.defaultConditions[(int)ai.stav].rychlost;
                a.Svetla = a.defaultConditions[(int)ai.stav].svetla;
            }
            else
            {
                OdhlasAuto(a);
            }
        }

        public void SubscribeMeteo(Meteo m)
        {
            m.Zmena += ZmeniloSePocasi;
        }

        public void AplikujStrategii(ZmenaStavuAuta zmenaStavu)
        {
            this.zmenaStavu = zmenaStavu;
            for (int i = 0; i < idxPosledniAuto; i++)
            {
                registr[i].ZmenaStavu += zmenaStavu;
            }
        }

        private void OdhlasAuto(Auto a)
        {
            Debug.WriteLine($"Systém odhlašuje auto {a.id}. Doba na cestě {a.CasNaCeste}.");

            registr[Array.IndexOf(registr, a)].ZmenaStavu -= this.zmenaStavu;
        }
    }

}
