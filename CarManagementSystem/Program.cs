using System;
using System.Timers;
using System.Diagnostics;

namespace CarManagementSystem
{
    public enum TypOmezeni
    {
        Zadne,
        Tunel,
        Most
    }

    
    public delegate void ZmenaPocasi(object sender, PocasiInfo inf);
    public delegate void ZmenRychlost(double delta);

    
    class Program
    {
        static void Main()
        {
            Random r = new Random();
            Meteo meteo = new Meteo();

            RC ridici = new RC(100);

            Omezeni[] oArr = {
                new Omezeni(TypOmezeni.Tunel, 200, 500),
                new Omezeni(TypOmezeni.Most, 2550, 2700),
                new Omezeni(TypOmezeni.Tunel, 4200, 4500),
                new Omezeni(TypOmezeni.Most, 8550,8700)
            };

            Auto a = new Auto(100, 10.0);
            a.PridejOmezeni(oArr);
            ridici.Add(a);

            a = new Auto(80, 4.0);
            a.PridejOmezeni(new Omezeni(TypOmezeni.Most, 350, 600));
            a.PridejOmezeni(new Omezeni(TypOmezeni.Most, 1200, 1900));
            ridici.Add(a);

            ridici.SubscribeMeteo(meteo);
            ridici.AplikujStrategii(ridici.StrategieOpatrna);

            Timer ticker = new Timer(100);
            ticker.Elapsed += meteo.Check;
            ridici.AddTimerToFleet(ticker);
            ticker.Start();

            System.Threading.Thread.Sleep(200000);
        }
    }
}
