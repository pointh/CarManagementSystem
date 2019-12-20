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
        Mokro,
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
            if (e.SignalTime.Ticks % 1211 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Sucho, teplota = 20.0 };
                Debug.WriteLine("Sucho" + $" teplota {pocI.teplota}");
                Zmena(this, pocI);
            }
            else if (e.SignalTime.Ticks % 1311 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Mokro, teplota = -10.0 };
                Debug.WriteLine("Mokro" + $" teplota {pocI.teplota}");
                Zmena(this, pocI);
            }
            else if (e.SignalTime.Ticks % 1511 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Mlha, teplota = 10.0 };
                Debug.WriteLine("Mlha" + $" teplota {pocI.teplota}");
                Zmena(this, pocI);
            }
        }
    }
}
