using System;
using System.Diagnostics;
using System.Timers;
using System.Resources;
using System.Globalization;
using CarManagementSystem.Properties;

namespace CarManagementSystem
{
    public delegate void ZmenaStavuAutaEventHandler(object sender, AutoInfoEventArgs inf);

    public class StandardniPodminky
    {
        public double Rychlost { get; set; }
        public bool Svetla { get; set; }
    }

    public class AutoInfoEventArgs : EventArgs
    {
        public AktualniStavyAuta Stav { get; set; }
        public AutoInfoEventArgs(AktualniStavyAuta stav)
        {
            this.Stav = stav;
        }
    }

    public class Omezeni
    {
        public Omezeni(TypOmezeni omezeni, double zacatek, double konec)
        {
            TypOmezeni = omezeni;
            this.Zacatek = zacatek;
            this.Konec = konec;
        }
        public TypOmezeni TypOmezeni { get; set; }
        public double Zacatek { get; set; } 
        public double Konec {get; set;}
    }

    public class Auto
    {
        public event ZmenaStavuAutaEventHandler ZmenaStavu;

        public Guid Id { get; }
        int citacOmezeni = 0;
        readonly double delkaTrasy, beznaRychlost;
        public double AktualniRychlost { get; set; }
        public bool Svetla { get; set; }
        readonly Omezeni[] omezeni; // tunely a mosty
        public AktualniStavyAuta Stav { get; set; }
        public TimeSpan CasNaCeste { get; set; }
        public double Ujeto { get; set; }
        readonly StandardniPodminky[] defaultConditions =
            new StandardniPodminky[Enum.GetValues(typeof(AktualniStavyAuta)).Length];
        public double KorekceRychlostiNaPocasi { get; set; } = 0.0;
        public bool KorekceSvetelNaPocasi { get; set; } = false;
        public Auto(double rychlost, double trasa)
        {
            omezeni = new Omezeni[100];
            Id = Guid.NewGuid();
            Ujeto = 0.0;
            Svetla = false;
            this.delkaTrasy = trasa * 1000; // na metry
            beznaRychlost = rychlost;
            AktualniRychlost = 0.0;
            this.Stav = AktualniStavyAuta.Start;
            defaultConditions[(int)AktualniStavyAuta.Start] =
                new StandardniPodminky { Rychlost = beznaRychlost, Svetla = false };
            defaultConditions[(int)AktualniStavyAuta.Trasa] =
                new StandardniPodminky { Rychlost = beznaRychlost, Svetla = false };
            defaultConditions[(int)AktualniStavyAuta.Most] =
                new StandardniPodminky { Rychlost = beznaRychlost - 10, Svetla = false };
            defaultConditions[(int)AktualniStavyAuta.Tunel] =
                new StandardniPodminky { Rychlost = beznaRychlost - 10, Svetla = true };
            defaultConditions[(int)AktualniStavyAuta.Stop] =
                new StandardniPodminky { Rychlost = 0.0, Svetla = false };
            KorekceRychlostiNaPocasi = 0.0;
            KorekceSvetelNaPocasi = false;
        }

        public StandardniPodminky VratAktualniPodminky()
        {
            return defaultConditions[(int)this.Stav];
        }

        public void PridejOmezeni(Omezeni o)
        {
            omezeni[citacOmezeni++] = o;
        }

        public void PridejOmezeni(Omezeni[] oArr)
        {

            if (oArr != null)
            {
                foreach (var o in oArr)
                    omezeni[citacOmezeni++] = o;
            }
            else
                throw new ArgumentException(Resources.ExcPoleNemuzeBytPrazdne);
        }

        public bool NaMoste()
        {
            for (int i = 0; i < citacOmezeni; i++)
            {
                if (omezeni[i].TypOmezeni == TypOmezeni.Most &&
                    omezeni[i].Zacatek < Ujeto &&
                    Ujeto < omezeni[i].Konec)
                    return true;
            }
            return false;
        }

        public bool VTunelu()
        {
            for (int i = 0; i < citacOmezeni; i++)
            {
                if (omezeni[i].TypOmezeni == TypOmezeni.Tunel &&
                    omezeni[i].Zacatek < Ujeto &&
                    Ujeto < omezeni[i].Konec)
                    return true;
            }
            return false;
        }

        public AktualniStavyAuta AktualniStav()
        {
            if (Ujeto >= delkaTrasy)
                return AktualniStavyAuta.Stop;

            if (VTunelu())
            {
                return AktualniStavyAuta.Tunel;
            }

            if (NaMoste())
            {
                return AktualniStavyAuta.Most;
            }

            return AktualniStavyAuta.Trasa;
        }

        public void Check(object sender, ElapsedEventArgs e)
        {
            TimeSpan usek = new TimeSpan(0, 0, 0, 0, 200);
            AutoInfoEventArgs autoInfo = null;

            if (this.Stav != AktualniStavyAuta.Stop)
            {
                CasNaCeste = CasNaCeste.Add(usek);
                Ujeto += (AktualniRychlost + KorekceRychlostiNaPocasi) / 3.6 * usek.TotalSeconds;
                Svetla = Svetla || KorekceSvetelNaPocasi;
                this.Stav = AktualniStav();

                autoInfo = new AutoInfoEventArgs(this.Stav);
            }

            if (ZmenaStavu != null) // auto není odhlášené
            {
                Debug.WriteLine(this);
                ZmenaStavu(this, autoInfo);
            }
        }

        public override string ToString()
        {
            string stavStr = "";
            switch (this.Stav)
            {
                case AktualniStavyAuta.Most:
                    stavStr = Resources.AktualniStavAutaMost;
                    break;
                case AktualniStavyAuta.Tunel:
                    stavStr = Resources.AktualniStavAutaTunel;
                    break;
                case AktualniStavyAuta.Trasa:
                    stavStr = Resources.AktualniStavAutaTrasa;
                    break;
            }

            string sv;
            if (this.Svetla)
                sv = "*";
            else
                sv = " ";

            return stavStr + sv + $"ID:{this.Id.ToString().Substring(0, 3)} " +
                $"l={this.Ujeto:F3} v={(this.AktualniRychlost + this.KorekceRychlostiNaPocasi):f3} stav {this.Stav.ToString()}";
        }
    }

}
