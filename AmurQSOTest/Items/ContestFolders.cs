using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// папка с файлами для расчета
    /// </summary>
    class ContestFolders : List<ContestFolder>
    {
        /// <summary>
        /// создание экземпляра с папками из конфига
        /// </summary>
        /// <param name="contestFolders"></param>
        public ContestFolders(List<FolderConfig> contestFolders)
        {
            foreach (FolderConfig config in contestFolders)
            {
                Add(new ContestFolder(config));
            }
        }

        /// <summary>
        /// выполнить загрузку папок из всех папкок
        /// </summary>
        public void Load()
        {
            foreach (ContestFolder f in this)
            {
                Console.WriteLine("\nКаталог: " + f.cfg.Name);
                f.Load();
                Console.WriteLine("Обработано: " + f.Files.Count);
            }
        }

        /// <summary>
        /// выполнить расчет
        /// </summary>
        public void Calculate()
        {
            foreach (ContestFolder f in this)
            {
                f.Calculate();
            }
        }

        /// <summary>
        /// выполнить запись UBN и т.д.
        /// </summary>
        public void Save()
        {
            foreach (ContestFolder f in this)
            {
                f.Save();
            }
        }
    }
}
