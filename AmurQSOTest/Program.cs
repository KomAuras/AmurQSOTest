using System;

namespace AmurQSOTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.SetArgs(args);
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
