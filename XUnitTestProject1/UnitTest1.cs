using System;
using Xunit;
using CarManagementSystem;
using System.Timers;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void AktualniStav()
        {
            Auto a = new Auto(60, 120);

            a.PridejOmezeni(new Omezeni(TypOmezeni.Most, 100, 200));
            a.PridejOmezeni(new Omezeni(TypOmezeni.Tunel, 300, 400));
            a.ujeto = 15.0;

            AktualniStavAuta b = a.AktualniStav(AktualniStavAuta.Start);
            Assert.True(b == AktualniStavAuta.Trasa);
            AktualniZmenaAuta za = a.NajdiAktualniZmenu(AktualniStavAuta.Start);
            Assert.True(za == AktualniZmenaAuta.StartTrasa);

            a.ujeto = 150;
            Assert.True(a.NaMoste());

            b = a.AktualniStav(AktualniStavAuta.Trasa);
            Assert.True(b == AktualniStavAuta.Most);
            za = a.NajdiAktualniZmenu(AktualniStavAuta.Trasa);
            Assert.True(za == AktualniZmenaAuta.TrasaMost);

            a.ujeto = 210.0;
            Assert.True(a.NaTrase());

            b = a.AktualniStav(AktualniStavAuta.Most);
            Assert.True(b == AktualniStavAuta.Trasa);
            za = a.NajdiAktualniZmenu(AktualniStavAuta.Most);
            Assert.True(za == AktualniZmenaAuta.MostTrasa);

            a.ujeto = 310.0;
            Assert.True(a.VTunelu());

            b = a.AktualniStav(AktualniStavAuta.Trasa);
            Assert.True(b == AktualniStavAuta.Tunel);
            za = a.NajdiAktualniZmenu(AktualniStavAuta.Trasa);
            Assert.True(za == AktualniZmenaAuta.TrasaTunel);

            a.ujeto = 410.0;
            Assert.False(a.VTunelu());

            b = a.AktualniStav(AktualniStavAuta.Tunel);
            Assert.True(b == AktualniStavAuta.Trasa);
            za = a.NajdiAktualniZmenu(AktualniStavAuta.Tunel);
            Assert.True(za == AktualniZmenaAuta.TunelTrasa);

            a.ujeto = 120010.0;
            Assert.False(a.NaTrase());

            b = a.AktualniStav(AktualniStavAuta.Trasa);
            Assert.True(b == AktualniStavAuta.Stop);

        }

        
    }
}
