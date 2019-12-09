using System;

namespace CarManagementSystem
{
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
        public StavAuta sa;
    }

    public class PocasiInfo
    {
        public Pocasi pocasi;
        public double teplota;
    }
    public class Omezeni
    {
        double zacatek, konec;
    }
    public delegate void ZmenaStavuAuta(object sender, AutoInfo inf);
    public delegate void ZmenaPocasi(object sender, PocasiInfo inf);
    class RC
    {
        Auto[] registr;
        int idxPosledniAuto;

        public void Add(Auto a)
        {
            registr[idxPosledniAuto++] = a;
        }

        public void HromadnaInstrukce(double deltaV, bool svetla = false)
        {
            for (int i = 0; i < registr.Length; i++)
            {
                if (registr[i] != null)
                    registr[i].SnizRychlost(10);
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
            switch (ai.sa) {
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

        public Auto()
        {
            omezeni = new Omezeni[100];
            id = Guid.NewGuid();
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
        }
    }

    class Meteo
    {
        public event ZmenaPocasi Zmena;

        public void Check()
        {
            PocasiInfo pocI = new PocasiInfo() { pocasi = Pocasi.Sucho, teplota = 20.0 };
            Zmena(this, pocI);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {

            Meteo meteo = new Meteo();
            RC ridici = new RC();
            ridici.Add(new Auto());
            ridici.Add(new Auto());
            ridici.Add(new Auto());
            ridici.Add(new Auto());
            ridici.Add(new Auto());
            ridici.SubscribeMeteo(meteo);
            
        }
    }
}
