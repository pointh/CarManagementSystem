using System;
using System.Diagnostics;
using System.Timers;
using System.Reflection;

namespace CarManagementSystem
{


    public class RC
    {

        readonly Auto[] registr;
        int idxPosledniAuto;
        ZmenaStavuAutaEventHandler zmenaStavu;

        public RC(int maxAuta)
        {
            registr = new Auto[maxAuta];
        }
        public void Add(Auto a)
        {
            registr[idxPosledniAuto++] = a;
        }

        public Auto[] VratPoleRegistrovanychAut()
        {
            return registr;
        }

        public void AddTimerToFleet(Timer t)
        {
            if (t != null)
            {
                foreach (Auto a in registr)
                    if (a != null)
                        t.Elapsed += a.Check;
            }
            else
                throw new ArgumentNullException(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        public void ZmeniloSePocasi(object sender, PocasiInfo inf)
        {
            const double blizkoMrazu = 3.0;

            if (inf != null)
            {
                for (int i = 0; i < registr.Length && registr[i] != null; i++)
                {

                    if (inf.Pocasi == Pocasi.Mlha)
                    {
                        if (inf.Teplota > blizkoMrazu)
                            registr[i].KorekceRychlostiNaPocasi = -10.0;
                        else
                            registr[i].KorekceRychlostiNaPocasi = -20.0;
                        registr[i].KorekceSvetelNaPocasi = true;
                    }
                    if (inf.Pocasi == Pocasi.Mokro)
                    {
                        if (inf.Teplota > blizkoMrazu)
                            registr[i].KorekceRychlostiNaPocasi = -10.0;
                        else
                            registr[i].KorekceRychlostiNaPocasi = -30.0;
                        registr[i].KorekceSvetelNaPocasi = false;
                    }
                    if (inf.Pocasi == Pocasi.Sucho)
                    {
                        registr[i].KorekceRychlostiNaPocasi = 0.0;
                        registr[i].KorekceSvetelNaPocasi = false;
                    }
                }
            }
            else
                throw new ArgumentNullException(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        public void StrategieOpatrna(object sender, AutoInfoEventArgs ai)
        {
            if (sender != null)
            {
                Auto a = sender as Auto;

                if (ai != null)
                {
                    a.AktualniRychlost = a.VratAktualniPodminky().Rychlost;
                    a.Svetla = a.VratAktualniPodminky().Svetla;
                }
                else
                {
                    OdhlasAuto(a);
                }
            }
            else
                throw new ArgumentNullException(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        public void SubscribeMeteo(Meteo m)
        {
            if (m!= null)
                m.Zmena += ZmeniloSePocasi;
            else
                throw new ArgumentNullException(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        public void AplikujStrategii(ZmenaStavuAutaEventHandler zmenaStavu)
        {
            this.zmenaStavu = zmenaStavu;
            for (int i = 0; i < idxPosledniAuto; i++)
            {
                registr[i].ZmenaStavu += zmenaStavu;
            }
        }

        private void OdhlasAuto(Auto a)
        {
            Debug.WriteLine($"Systém odhlašuje auto {a.Id}. Doba na cestě {a.CasNaCeste}.");

            registr[Array.IndexOf(registr, a)].ZmenaStavu -= this.zmenaStavu;
        }
    }

}
