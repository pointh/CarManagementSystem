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
            
            Omezeni tunel = new Omezeni(TypOmezeni.Tunel, 200, 500);
            Omezeni most = new Omezeni(TypOmezeni.Most, 550, 700);
            Auto a = new Auto(70, 100.0);
            a.PridejOmezeni(tunel);
            a.PridejOmezeni(most);
            ridici.Add(a);  

            ridici.Add(new Auto(55, 100.0).GenerujNahodnaOmezeni(5, r));
            ridici.Add(new Auto(80, 100.0).GenerujNahodnaOmezeni(3, r));
            ridici.Add(new Auto(60, 250.0).GenerujNahodnaOmezeni(2, r));
            ridici.Add(new Auto(90, 200.0).GenerujNahodnaOmezeni(4, r));
            
            ridici.SubscribeMeteo(meteo);
            ridici.AplikujStrategii(ridici.StrategieOpatrna);

            Timer ticker = new Timer(200);
            ticker.Elapsed += meteo.Check;
            ridici.AddTimerToFleet(ticker);
            ticker.Start();

            System.Threading.Thread.Sleep(200000);
        }
    }
}
