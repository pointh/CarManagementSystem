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
            a.Ujeto = 15.0;

           

        }

        
    }
}
