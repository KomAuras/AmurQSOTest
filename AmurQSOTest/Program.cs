using AmurQSOTest.Items;
using System;

namespace AmurQSOTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Config.SetArgs(args)) { return; }
            Standards.Build();
            Console.WriteLine(Config.ProgrammTitle);
            Manager manager = new Manager();
            manager.Run();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("================= END ==================");
                Console.ReadKey();
            }
        }
    }
}
