using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace CarManagementSystem
{
    public delegate void ZmenaStavuAuta(object sender, AutoInfo inf);
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

    public class Auto
    {
        public event ZmenaStavuAuta ZmenaStavu;

        public Guid id;
        int citacOmezeni = 0;
        public double delkaTrasy, aktualniRychlost, beznaRychlost;
        public bool Svetla { get; set; }
        public Omezeni[] omezeni; // tunely a mosty
        public AktualniStavAuta stav;
        public TimeSpan CasNaCeste { get; set; }
        public double Ujeto { get; set; }
        public StandardniPodminky[] defaultConditions = 
            new StandardniPodminky[Enum.GetValues(typeof(AktualniStavAuta)).Length];
        public double KorekceRychlostiNaPocasi { get; set; } = 0.0;
        public bool KorekceSvetelNaPocasi { get; set; } = false;
        public Auto(double rychlost, double trasa)
        {
            omezeni = new Omezeni[100];
            id = Guid.NewGuid();
            Ujeto = 0.0;
            Svetla = false;
            this.delkaTrasy = trasa * 1000; // na metry
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
                new StandardniPodminky { rychlost = beznaRychlost - 10, svetla = true };
            defaultConditions[(int)AktualniStavAuta.Stop] = 
                new StandardniPodminky { rychlost = 0.0, svetla = false };
            KorekceRychlostiNaPocasi = 0.0;
            KorekceSvetelNaPocasi = false;
        }
        public void PridejOmezeni(Omezeni o)
        {
            omezeni[citacOmezeni++] = o;
        }

        public void PridejOmezeni(Omezeni[] oArr)
        {
            foreach ( var o in oArr)
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

        public AktualniStavAuta AktualniStav()
        {
            if (Ujeto >= delkaTrasy)
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

        public void Check(object sender, ElapsedEventArgs e)
        {
            TimeSpan usek = new TimeSpan(0, 0, 0, 0, 200);
            AutoInfo autoInfo = null;

            if (this.stav != AktualniStavAuta.Stop)
            {
                CasNaCeste = CasNaCeste.Add(usek);
                Ujeto += (aktualniRychlost + KorekceRychlostiNaPocasi) / 3.6 * usek.TotalSeconds;
                Svetla = Svetla || KorekceSvetelNaPocasi;
                this.stav = AktualniStav();

                autoInfo = new AutoInfo()
                {
                    aktualRychlost = aktualniRychlost + KorekceRychlostiNaPocasi,
                    cestRychlost = beznaRychlost,
                    poloha = Ujeto,
                    stav = this.stav
                };
            }

            if (ZmenaStavu != null) // auto není odhlášené
            {
                Debug.WriteLine(this);
                ZmenaStavu(this, autoInfo);
            }
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
                    stavStr = " ";
                    break;
            }

            if (this.Svetla)
                sv = "*";
            else
                sv = " ";

            return stavStr + sv + $"ID:{this.id.ToString().Substring(0, 3)} " +
                $"l={this.Ujeto:F3} v={(this.aktualniRychlost+this.KorekceRychlostiNaPocasi):f3} stav {this.stav.ToString()}";
        }
    }

}
