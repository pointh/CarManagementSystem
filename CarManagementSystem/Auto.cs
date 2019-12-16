using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace CarManagementSystem
{
   
    public enum AktualniStavAuta
    {
        Start,
        Trasa,
        Tunel,
        Most,
        Stop
    }

    public class StandardniPodminky
    {
        public double rychlost;
        public bool svetla;
        public double[] korekceRychlosti;
        public bool[] korekceSvetel;
    }

    public class AutoInfo:EventArgs
    {
        public double cestRychlost, aktualRychlost;
        public double poloha;
        public AktualniStavAuta stav;
    }

    public class Omezeni
    {
        public Omezeni(TypOmezeni omezeni, double zacatek, double konec)
        {
            typOmezeni = omezeni;
            this.zacatek = zacatek;
            this.konec = konec;
        }
        public TypOmezeni typOmezeni;
        public double zacatek, konec;
    }

    public delegate void ZmenaStavuAuta(object sender, AutoInfo inf);

    public class Auto
    {
        Guid id;
        int citacOmezeni = 0;
        public event ZmenaStavuAuta ZmenaStavu;
        public double trasa, aktualniRychlost, beznaRychlost;
        public bool Svetla { get; set; }
        Omezeni[] omezeni;
        AktualniStavAuta stav;
        private double CasNaCeste { get; set; }
        public double Ujeto { get; set; }
        public StandardniPodminky[] defaultConditions = new StandardniPodminky[Enum.GetValues(typeof(AktualniStavAuta)).Length];

        public Auto(double rychlost, double trasa)
        {
            omezeni = new Omezeni[100];
            id = Guid.NewGuid();
            Ujeto = 0.0;
            Svetla = false;
            this.trasa = trasa * 1000; // na metry
            beznaRychlost = rychlost;
            aktualniRychlost = 0.0;
            this.stav = AktualniStavAuta.Start;
            defaultConditions[(int)AktualniStavAuta.Start] = 
                new StandardniPodminky { rychlost = beznaRychlost, svetla = false };
            defaultConditions[(int)AktualniStavAuta.Trasa] = 
                new StandardniPodminky { rychlost = beznaRychlost, svetla = false };
            defaultConditions[(int)AktualniStavAuta.Most] = 
                new StandardniPodminky { rychlost = beznaRychlost - 10, svetla = false };
            defaultConditions[(int)AktualniStavAuta.Tunel] = 
                new StandardniPodminky
                {
                    rychlost = beznaRychlost - 10,
                    svetla = true,
                    korekceRychlosti = new double[] { 0, -10, -10 },
                    korekceSvetel = new bool[] {false, false, true}
                };
            defaultConditions[(int)AktualniStavAuta.Stop] = 
                new StandardniPodminky { rychlost = 0.0, svetla = false };
        }
        public void PridejOmezeni(Omezeni o)
        {
            omezeni[citacOmezeni++] = o;
        }

        public bool NaMoste()
        {
            for (int i = 0; i < citacOmezeni; i++)
            {
                if (omezeni[i].typOmezeni == TypOmezeni.Most &&
                    omezeni[i].zacatek < Ujeto &&
                    Ujeto < omezeni[i].konec)
                    return true;
            }
            return false;
        }

        public bool VTunelu()
        {
            for (int i = 0; i < citacOmezeni; i++)
            {
                if (omezeni[i].typOmezeni == TypOmezeni.Tunel &&
                    omezeni[i].zacatek < Ujeto &&
                    Ujeto < omezeni[i].konec)
                    return true;
            }
            return false;
        }

        public bool NaTrase()
        {
            return !VTunelu() && !NaMoste() && Ujeto < trasa;
        }

        public AktualniStavAuta AktualniStav()
        {
            if (Ujeto >= trasa)
                return AktualniStavAuta.Stop;

            if (VTunelu())
            {
                return AktualniStavAuta.Tunel;
            }

            if (NaMoste())
            {
                return AktualniStavAuta.Most;
            }

            return AktualniStavAuta.Trasa;
        }

        public Auto GenerujNahodnaOmezeni(int pocet, Random rnd)
        {
            double start = 0.0;
            for (int i = 0; i < pocet; i++)
            {
                double delka = 50 + 500 * rnd.NextDouble();
                start = start + (this.trasa - start) * rnd.NextDouble();
                if (start + delka > trasa) // omezení končí za koncem trasy
                    break;
                omezeni[i] = new Omezeni((TypOmezeni)rnd.Next(1, Enum.GetValues(typeof(TypOmezeni)).Length),
                    start, start + delka);
            }
            return this;
        }

        public void Registruj(RC ridiciCentrum)
        {
            ridiciCentrum.Add(this);
        }

        public void SnizRychlost(double deltaV)
        {
            if (this.aktualniRychlost > deltaV)
            {
                this.aktualniRychlost -= deltaV;
            }
        }

        public void ZvysRychlost(double deltaV)
        {
            if (this.aktualniRychlost < this.beznaRychlost)
                this.aktualniRychlost += deltaV;
        }

        public void RozsvitSvetla()
        {
            if (Svetla == false)
            {
                Svetla = true;
            }
        }

        public void ZhasniSvetla()
        {
            if (Svetla == true)
            {
                Svetla = false;
            }
        }

        public void Check(object sender, ElapsedEventArgs e)
        {
            AutoInfo autoInfo = new AutoInfo() { 
                aktualRychlost = aktualniRychlost, 
                cestRychlost = beznaRychlost, 
                poloha = Ujeto };
            CasNaCeste += 1.0;
            Ujeto += aktualniRychlost / 3.6 * 1.0;
            autoInfo.stav = this.stav = AktualniStav();
            Debug.WriteLineIf(this.stav == AktualniStavAuta.Tunel || this.stav == AktualniStavAuta.Most, this);
            ZmenaStavu(this, autoInfo);
        }

        public override string ToString()
        {
            string sv = "";
            string stavStr = "";
            switch (this.stav)
            {
                case AktualniStavAuta.Most:
                    stavStr = "M";
                    break;
                case AktualniStavAuta.Tunel:
                    stavStr = "T";
                    break;
                case AktualniStavAuta.Trasa:
                    stavStr = "T";
                    break;
            }

            if (this.Svetla)
                sv = "*";
            else
                sv = "-";

            return stavStr + sv + $"ID:{this.id.ToString().Substring(0, 3)} l={this.Ujeto:F3} v={this.aktualniRychlost:f3} stav {this.stav.ToString()}";
        }
    }

}
