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
        int citacOmezeni = 0;
        public event ZmenaStavuAuta ZmenaStavu;
        public double trasa, aktualniRychlost, beznaRychlost;
        bool svetla;
        Omezeni[] omezeni;
        AktualniStavAuta stav, minulyStav;
        private double CasNaCeste { get; set; }
        public double ujeto { get; set; }

        public Auto(double rychlost, double trasa)
        {
            omezeni = new Omezeni[100];
            id = Guid.NewGuid();
            ujeto = 0.0;
            svetla = false;
            this.trasa = trasa * 1000; // na metry
            beznaRychlost = rychlost;
            aktualniRychlost = 0.0;
            stav = minulyStav = AktualniStavAuta.Start;

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
                    omezeni[i].zacatek < ujeto &&
                    ujeto < omezeni[i].konec)
                    return true;
            }
            return false;
        }

        public bool VTunelu()
        {
            for (int i = 0; i < citacOmezeni; i++)
            {
                if (omezeni[i].typOmezeni == TypOmezeni.Tunel &&
                    omezeni[i].zacatek < ujeto &&
                    ujeto < omezeni[i].konec)
                    return true;
            }
            return false;
        }

        public bool NaTrase()
        {
            return !VTunelu() && !NaMoste() && ujeto < trasa;
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
            if (ujeto >= trasa)
                return AktualniStavAuta.Stop;

            if (minulyStav == AktualniStavAuta.Start && ujeto < trasa && NaTrase())
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

            if (ujeto < trasa && NaTrase())
                return AktualniStavAuta.Trasa;

            return minulyStav;
        }

        public Auto GenerujNahodnaOmezeni(int pocet, Random rnd)
        {
            double start = 0.0;
            for (int i = 0; i < pocet; i++)
            {
                double delka = 0.05 + 5 * rnd.NextDouble();
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
                if (minulyStav == AktualniStavAuta.Trasa && a == AktualniStavAuta.Stop)
                    zmenaNaTrase = AktualniZmenaAuta.TrasaStop;
                Debug.WriteLine(this);
            }
            this.minulyStav = a;
            
            return zmenaNaTrase;
        }

        public void Check(object sender, ElapsedEventArgs e)
        {
            AutoInfo autoInfo = new AutoInfo() { aktualRychlost = aktualniRychlost, 
                cestRychlost = beznaRychlost, 
                poloha = ujeto };
            CasNaCeste += 1.0;
            ujeto += aktualniRychlost / 3.6 * 1.0;
            autoInfo.zmenaNaTrase = NajdiAktualniZmenu(minulyStav);
            
            ZmenaStavu(this, autoInfo);
        }

        public override string ToString()
        {
            string sv = "";
            if (this.svetla)
                sv = "*";
            return sv + $"ID:{this.id.ToString().Substring(0, 3)} l={this.ujeto:F3} v={this.aktualniRychlost:f3} stav {this.stav.ToString()}";
        }
    }

}
