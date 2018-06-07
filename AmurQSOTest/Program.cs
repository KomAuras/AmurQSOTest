using System;

namespace AmurQSOTest
{
    class Program
    {
        private const string Title = "AmurQSOTest ver 1.0";
        static void Main(string[] args)
        {
            if (Config.SetArgs(args)) { return; }
            Console.WriteLine(Title);
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
