using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace CarManagementSystem
{
    public enum AktualniZmenaAuta
    {
        StartTrasa,
        TrasaStop,
        TrasaTunel,
        TunelTrasa,
        TrasaMost,
        MostTrasa,
        TunelMost,
        MostTunel,
        KonecRegistrace,
        BezeZmeny
    }

    public enum AktualniStavAuta
    {
        Start,
        Trasa,
        Tunel,
        Most,
        Stop
    }

    public class AutoInfo
    {
        public double cestRychlost, aktualRychlost;
        public double poloha;
        public AktualniZmenaAuta zmenaNaTrase;
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
        public event ZmenaStavuAuta ZmenaStavu;
        public double delkaTrasy, aktualniRychlost, beznaRychlost;
        bool svetla;
        List<Omezeni> omezeni;
        AktualniStavAuta stav, minulyStav;
        public double Ujeto { get; set; }

        public Auto(double rychlost, double trasa)
        {
            omezeni = new List<Omezeni>();
            id = Guid.NewGuid();
            Ujeto = 0.0;
            svetla = false;
            this.delkaTrasy = trasa * 1000; // na metry
            beznaRychlost = rychlost;
            aktualniRychlost = 0.0;
            stav = minulyStav = AktualniStavAuta.Start;
        }
        public void PridejOmezeni(Omezeni o)
        {
            omezeni.Add(o);
        }

        public bool NaMoste()
        {
            foreach ( var o in omezeni )
            {
                if (o.typOmezeni == TypOmezeni.Most &&
                    o.zacatek < Ujeto &&
                    Ujeto < o.konec)
                    return true;
            }
            return false;
        }

        public bool VTunelu()
        {
            foreach (var o in omezeni)
            {
                if (o.typOmezeni == TypOmezeni.Tunel &&
                    o.zacatek < Ujeto &&
                    Ujeto < o.konec)
                    return true;
            }
            return false;
        }

        public bool NaTrase()
        {
            return !VTunelu() && !NaMoste() && Ujeto < delkaTrasy;
        }

        /*          Start           Trasa           Tunel           Most            Stop
         * Start                    StartTrasa
         * Trasa                                    TrasaTunel                      TrasaStop
         * Tunel                    TunelTrasa                      TunelMost
         * Most                     MostTrasa       MostTunel
         * Stop
         */
        public AktualniStavAuta AktualniStav(AktualniStavAuta minulyStav)
        {
            if (Ujeto >= delkaTrasy)
                return AktualniStavAuta.Stop;

            if (minulyStav == AktualniStavAuta.Start && Ujeto < delkaTrasy && NaTrase())
            {
                return AktualniStavAuta.Trasa;
            }

            if (VTunelu())
            {
                return AktualniStavAuta.Tunel;
            }

            if (NaMoste())
            {
                return AktualniStavAuta.Most;
            }

            if (Ujeto < delkaTrasy && NaTrase())
                return AktualniStavAuta.Trasa;

            return minulyStav;
        }

        public Auto GenerujNahodnaOmezeni(int pocet, Random rnd)
        {
            for (int i = 0; i < pocet; i++)
            {
                double start = 0.0;
                double delka = 200 + 2000 * rnd.NextDouble();
                start = this.delkaTrasy * rnd.NextDouble();
                if (start + delka > delkaTrasy) // omezení končí za koncem trasy
                    break;
                omezeni.Add(new Omezeni((TypOmezeni)rnd.Next(1, Enum.GetValues(typeof(TypOmezeni)).Length),
                    start, start + delka));
            }
            return this;
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
            if (svetla == false)
            {
                svetla = true;
            }
        }

        public void ZhasniSvetla()
        {
            if (svetla == true)
            {
                svetla = false;
            }
        }

        public AktualniZmenaAuta NajdiAktualniZmenu(AktualniStavAuta minulyStav)
        {
            AktualniStavAuta a;
            AktualniZmenaAuta zmenaNaTrase = AktualniZmenaAuta.BezeZmeny;

            if ((a = AktualniStav(minulyStav)) != minulyStav)
            {
                this.stav = a;
                if (minulyStav == AktualniStavAuta.Start)
                    zmenaNaTrase = AktualniZmenaAuta.StartTrasa;

                if (minulyStav == AktualniStavAuta.Trasa)
                {
                    if (a == AktualniStavAuta.Stop)
                        zmenaNaTrase = AktualniZmenaAuta.TrasaStop;
                    if (a == AktualniStavAuta.Most)
                        zmenaNaTrase = AktualniZmenaAuta.TrasaMost;
                    if (a == AktualniStavAuta.Tunel)
                        zmenaNaTrase = AktualniZmenaAuta.TrasaTunel;
                }
                
                if (minulyStav == AktualniStavAuta.Most)
                {
                    if (a == AktualniStavAuta.Stop)
                        zmenaNaTrase = AktualniZmenaAuta.TrasaStop;
                    if (a == AktualniStavAuta.Trasa)
                        zmenaNaTrase = AktualniZmenaAuta.MostTrasa;
                    if (a == AktualniStavAuta.Tunel)
                        zmenaNaTrase = AktualniZmenaAuta.MostTunel;
                }
                
                if (minulyStav == AktualniStavAuta.Tunel)
                {
                    if (a == AktualniStavAuta.Stop)
                        zmenaNaTrase = AktualniZmenaAuta.TrasaStop;
                    if (a == AktualniStavAuta.Trasa)
                        zmenaNaTrase = AktualniZmenaAuta.TunelTrasa;
                    if (a == AktualniStavAuta.Most)
                        zmenaNaTrase = AktualniZmenaAuta.TunelMost;
                }
                
                if (a == AktualniStavAuta.Stop)
                {
                    zmenaNaTrase = AktualniZmenaAuta.KonecRegistrace;
                    Console.WriteLine($"Auto ID:{ this.id.ToString().Substring(0, 3)} je v cíli.");
                }
                Debug.WriteLine(this);
            }
            this.minulyStav = a;
            
            return zmenaNaTrase;
        }

        public void AktualizujStav(object sender, ElapsedEventArgs e)
        {
            if (stav == AktualniStavAuta.Stop)
                return;

            AutoInfo autoInfo = new AutoInfo() 
            { 
                aktualRychlost = aktualniRychlost, 
                cestRychlost = beznaRychlost, 
                poloha = Ujeto 
            };
            Ujeto += aktualniRychlost / 3.6 * 5.0;
            autoInfo.zmenaNaTrase = NajdiAktualniZmenu(minulyStav);
            Console.WriteLine(this);
            ZmenaStavu(this, autoInfo);
        }

        public override string ToString()
        {
            string sv = "";
            if (this.svetla)
                sv = "*";
            return sv + $"Auto ID:{this.id.ToString().Substring(0, 3)}"+
                $" l={this.Ujeto:F3} v={this.aktualniRychlost:f3} stav {this.stav.ToString()}";
        }
    }

}
