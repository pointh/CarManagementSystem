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

    public enum StavAuta
    {
        Start,
        Stop,
        TrasaTunel,
        TunelTrasa,
        TrasaMost,
        MostTrasa,
        TunelMost,
        MostTunel
    }

    public enum Pocasi
    {
        Mlha,
        Mraz,
        Sucho,
    }
    public class AutoInfo
    {
        public double cestRychlost, aktualRychlost;
        public double poloha;
        public StavAuta zmenaNaTrase;
    }

    public class PocasiInfo
    {
        public Pocasi pocasi;
        public double teplota;
    }
    public class Omezeni
    {
        public Omezeni(TypOmezeni omezeni, double zacatek, double konec)
        {
            typOmezeni = omezeni;
            this.zacatek = zacatek;
            this.konec = konec;
        }
        TypOmezeni typOmezeni;
        double zacatek, konec;
    }

    public delegate void ZmenaStavuAuta(object sender, AutoInfo inf);
    public delegate void ZmenaPocasi(object sender, PocasiInfo inf);
    public delegate void ZmenRychlost(double delta);

    class RC
    {
        Auto[] registr;
        int idxPosledniAuto;

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

        public void HromadnaInstrukce(double deltaV, bool svetla = false)
        {
            for (int i = 0; i < registr.Length; i++)
            {
                if (registr[i] != null)
                    registr[i].SnizRychlost(deltaV);
            }
        }

        public void ZmeniloSePocasi(object sender, PocasiInfo inf) {
            if (inf.teplota < 0)
                HromadnaInstrukce(10);
            if (inf.pocasi == Pocasi.Mlha)
                HromadnaInstrukce(10);
            if (inf.teplota > 0)
                HromadnaInstrukce(-10);
            if (inf.pocasi == Pocasi.Sucho)
                HromadnaInstrukce(-10);
        }

        public void ZmenilSeStavAuta(object sender, AutoInfo ai)
        {
            switch (ai.zmenaNaTrase) {
                case StavAuta.MostTrasa:
                case StavAuta.TunelTrasa:
                    (sender as Auto).SnizRychlost(-10.0);
                    break;
                case StavAuta.MostTunel:
                case StavAuta.TunelMost:
                    break;
                case StavAuta.TrasaMost:
                case StavAuta.TrasaTunel:
                    (sender as Auto).SnizRychlost(10);
                    break;   
            }
        }

        public void SubscribeMeteo(Meteo m)
        {
            m.Zmena += ZmeniloSePocasi;
        }

        public void SubscribeAuto(Auto a)
        {
            a.ZmenaStavu += ZmenilSeStavAuta;
        }
    }

    class Auto
    {
        Guid id;
        int citacOmezeni = 0;
        public event ZmenaStavuAuta ZmenaStavu;
        public double trasa, aktualniRychlost;
        bool svetla;
        Omezeni[] omezeni;
        private double CasNaCeste { get; set; } 
        public double Ujeto { get; set; }

        public Auto(double rychlost, double trasa)
        {
            omezeni = new Omezeni[100];
            id = Guid.NewGuid();
            Ujeto = 0.0;
            svetla = false;
            trasa = 0.0;
            aktualniRychlost = rychlost;
            this.trasa = trasa;
        }
        public void PridejOmezeni(Omezeni o)
        {
            omezeni[citacOmezeni++] = o;
        }

        public void Registruj(RC ridiciCentrum)
        {
            ridiciCentrum.Add(this);
        }

        public void SnizRychlost(double deltaV)
        {
            this.aktualniRychlost -= deltaV;
            Debug.WriteLine($"Změna rychlosti o {deltaV}");
        }

        public void Check(object sender, ElapsedEventArgs e)
        {
            CasNaCeste += 1.0;
            Ujeto += aktualniRychlost * 1.0;
            Debug.WriteLine($"Auto: {this.id}, Ujeto {this.Ujeto}, Rychlost {this.aktualniRychlost}");
        }
    }

    class Meteo
    {
        public event ZmenaPocasi Zmena;

        
        public void Check(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime.Ticks % 125 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Sucho, teplota = 20.0 };
                Zmena(this, pocI);
            }
            else if (e.SignalTime.Ticks % 105 == 0)
            {
                PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Mraz, teplota = -10.0 };
                Zmena(this, pocI);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            
            Meteo meteo = new Meteo();
            
            RC ridici = new RC(100);
            Auto a1 = new Auto(3.5, 100.0);
            a1.PridejOmezeni(new Omezeni(TypOmezeni.Most, 10.0, 11.5));
            ridici.Add(a1);
            ridici.Add(new Auto(2.5, 100.0));
            ridici.Add(new Auto(3.0, 100.0));
            ridici.Add(new Auto(2.1, 250.0));
            ridici.Add(new Auto(3.7, 200.0));

            ridici.SubscribeMeteo(meteo);
            Timer ticker = new Timer(1500);
            ticker.Elapsed += meteo.Check;
            ridici.AddTimerToFleet(ticker);
            ticker.Start();
            System.Threading.Thread.Sleep(20000);
        }
    }
}
