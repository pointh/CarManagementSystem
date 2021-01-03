using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace CarManagementSystem
{
    public enum Pocasi
    {
        Mlha,
        Mraz,
        Sucho,
    }

    public class PocasiInfo
    {
        public Pocasi pocasi;
        public double teplota;
    }

    public class Meteo
    {
        public event ZmenaPocasi Zmena;
        public void Check(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime.Ticks % 12 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Sucho, teplota = 20.0 };
                Debug.WriteLine("Meteo: Sucho");
                Zmena(this, pocI);
                return;
            }
            if (e.SignalTime.Ticks % 13 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Mraz, teplota = -10.0 };
                Debug.WriteLine("Meteo: Mráz");
                Zmena(this, pocI);
                return;
            }
            if (e.SignalTime.Ticks % 23 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Mraz, teplota = -10.0 };
                Debug.WriteLine("Meteo: Mlha");
                Zmena(this, pocI);
                return;
            }
        }
    }
}
