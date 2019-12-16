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

            a = new Auto(55, 10.0);
            a.PridejOmezeni(new Omezeni(TypOmezeni.Most, 350, 600));
            a.PridejOmezeni(new Omezeni(TypOmezeni.Most, 1200, 1900));
            ridici.Add(a);

           
            //ridici.Add(new Auto(80, 10.0).GenerujNahodnaOmezeni(30, r));
            //ridici.Add(new Auto(60, 25.0).GenerujNahodnaOmezeni(20, r));
            //ridici.Add(new Auto(90, 20.0).GenerujNahodnaOmezeni(40, r));
            
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
