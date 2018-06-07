using AmurQSOTest.Items;
using System;
using System.Collections.Generic;

namespace AmurQSOTest
{
    public class Manager
    {
        private ContestFolders Folders;

        public void Run()
        {
            Config.Load();
            Config.Dump();
            Folders = new ContestFolders(Config.ContestFolders);
            Folders.Load();
        }
    }
}