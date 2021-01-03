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
            
            FlotilaVozu ridici = new FlotilaVozu();
            
            Omezeni tunel = new Omezeni(TypOmezeni.Tunel, 200, 500);
            Omezeni most = new Omezeni(TypOmezeni.Most, 550, 700);
            Auto a = new Auto(70, 11.0);
            a.PridejOmezeni(tunel);
            a.PridejOmezeni(most);
            ridici.PridejAuto(a);  

            ridici.PridejAuto(new Auto(55, 21.0).GenerujNahodnaOmezeni(5, r));
            ridici.PridejAuto(new Auto(80, 10.0).GenerujNahodnaOmezeni(3, r));
            ridici.PridejAuto(new Auto(60, 12.0).GenerujNahodnaOmezeni(2, r));
            ridici.PridejAuto(new Auto(90, 11.0).GenerujNahodnaOmezeni(4, r));
            
            ridici.SubscribeMeteo(meteo);
            ridici.AplikujStrategii(ridici.StrategieOpatrna);

            Timer ticker = new Timer(200);
            ticker.Elapsed += meteo.Check;
            ridici.AddTimerToFleet(ticker);
            ticker.Start();

            Console.ReadLine();
        }
    }
}
