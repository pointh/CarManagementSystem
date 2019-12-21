using System.Diagnostics;
using System.Timers;

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
        public Pocasi Pocasi { get; set; }
        public double Teplota { get; set; }
    }

    public class Meteo
    {
        public event ZmenaPocasi Zmena;
        public void Check(object sender, ElapsedEventArgs e)
        {
            if (e?.SignalTime.Ticks % 121 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { Pocasi = Pocasi.Sucho, Teplota = 20.0 };
                Debug.WriteLine("Sucho" + $" teplota {pocI.Teplota}");
                Zmena(this, pocI);
            }
            else if (e?.SignalTime.Ticks % 131 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { Pocasi = Pocasi.Mokro, Teplota = -10.0 };
                Debug.WriteLine("Mokro" + $" teplota {pocI.Teplota}");
                Zmena(this, pocI);
            }
            else if (e?.SignalTime.Ticks % 151 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { Pocasi = Pocasi.Mlha, Teplota = 10.0 };
                Debug.WriteLine("Mlha" + $" teplota {pocI.Teplota}");
                Zmena(this, pocI);
            }
        }
    }
}
