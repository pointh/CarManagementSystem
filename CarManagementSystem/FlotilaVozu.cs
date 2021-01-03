using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Linq;

namespace CarManagementSystem
{
    public class FlotilaVozu
    {
        List<Auto> registr;

        public FlotilaVozu()
        {
            registr = new List<Auto>();
        }

        public void PridejAuto(Auto a)
        {
            registr.Add(a);
        }

        public void OdeberAuto(Auto a)
        {
            registr.Remove(a);
            a = null;
        }

        public void AddTimerToFleet(Timer t)
        {
            foreach (Auto a in registr)
                if (a != null)
                    t.Elapsed += a.AktualizujStav;
        }

        public void ZmeniloSePocasi(object sender, PocasiInfo inf)
        {
            foreach (var a in registr )
            {
                if (inf.teplota < 0)
                    a.SnizRychlost(10);
                if (inf.pocasi == Pocasi.Mlha)
                {
                    a.SnizRychlost(10);
                    a.RozsvitSvetla();
                }
                if (inf.teplota > 0)
                    a.ZvysRychlost(10);
                if (inf.pocasi == Pocasi.Sucho)
                    a.ZvysRychlost(10);
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
                case AktualniZmenaAuta.KonecRegistrace:
                    OdeberAuto(sender as Auto);
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
            foreach (var a in registr)
            {
                a.ZmenaStavu += zmenaStavu;
            }
        }
    }

}
