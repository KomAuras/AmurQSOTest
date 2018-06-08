using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZone
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define two locator positions
            string homeLoc = "JN59MN";
            string destLoc = "JO63DH";

            // Compute distance [km] and direction [°]
            double distance = MaidenheadLocator.Distance(homeLoc, destLoc);
            Console.WriteLine(distance);
            double azimuth = MaidenheadLocator.Azimuth(homeLoc, destLoc);

            // Convert locator to latitude/longitude
            LatLng homeLL = MaidenheadLocator.LocatorToLatLng(homeLoc);

            // Convert latitude/longitude to locator
            string myLoc = MaidenheadLocator.LatLngToLocator(49.57, 11.08);


            //Test test = new Test("1", "2", "3", "4");
            //SetProperty(test, "Var1", "STR!");
            //Console.WriteLine(GetProperty(test, "Var1"));
            Console.ReadKey();
        }

        public static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        public static void SetProperty(object target, string name, object value)
        {
            Type type = target.GetType();
            PropertyInfo prop = type.GetProperty(name);
            prop.SetValue(target, value, null);
        }
    }

    public class Test
    {
        public string Var1 { get; set; }
        public string Var2 { get; set; }
        public string Var3 { get; set; }
        public string Var4 { get; set; }

        public Test(string var1, string var2, string var3, string var4)
        {
            Var1 = var1;
            Var2 = var2;
            Var3 = var3;
            Var4 = var4;
        }
    }
}
