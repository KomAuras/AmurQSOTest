using System;
using System.Collections.Generic;
using System.IO;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// каталог с файлами для расчета
    /// </summary>
    public class ContestFolder
    {
        /// <summary>
        /// настройки  папки
        /// </summary>
        public FolderConfig cfg;
        /// <summary>
        /// список файлов для расчета
        /// </summary>
        public List<ContestFile> Files = new List<ContestFile>();

        /// <summary>
        /// создать экземпляр с настройками из Config
        /// </summary>
        /// <param name="conf"></param>
        public ContestFolder(FolderConfig conf)
        {
            this.cfg = conf;
        }

        /// <summary>
        /// создать экземпляры файлов
        /// </summary>
        public void Load()
        {
            DirectoryInfo d = new DirectoryInfo(Config.AppFolder() + Config.ContestPath + @"\" + cfg.Name);
            if (!d.Exists)
            {
                throw new Exception(string.Format("Папка {0} не найдена!", d.FullName));
            }
            FileInfo[] Files = d.GetFiles("*.cbr");
            foreach (FileInfo file in Files)
            {
                this.Files.Add(new ContestFile(this, file.FullName));
            }
        }

        /// <summary>
        /// расчет по одному файлу
        /// </summary>
        public void Calculate()
        {
            foreach (ContestFile file in this.Files)
            {
                file.Calculate();
            }
        }

        /// <summary>
        /// запись ГИТ и т.д. одному файлу
        /// </summary>
        public void Save()
        {
            foreach (ContestFile file in this.Files)
            {
                file.Save();
            }
        }

        /// <summary>
        /// получить лог корреспондента
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public ContestFile Get(string call)
        {
            foreach (ContestFile f in Files)
            {
                if (f.Call == call)
                    return f;
            }
            return null;
        }
    }
}