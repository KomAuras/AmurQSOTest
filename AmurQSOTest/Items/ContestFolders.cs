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
                f.Load();
            }
        }
    }
}
