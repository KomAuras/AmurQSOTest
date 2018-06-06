using System;

namespace AmurQSOTest
{
    public class Manager
    {
        private const string Title = "AmurQSOTest ver 1.0";

        public void Run()
        {
            Console.WriteLine(Title);
            if (Config.Load()) {
                Console.WriteLine("Не задана папка с соревнованием!");
                return;
            }
            Config.Dump();
        }
    }
}