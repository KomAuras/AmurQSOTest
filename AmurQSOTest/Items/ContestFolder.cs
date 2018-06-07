using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public List<ContestFile> Files;

        /// <summary>
        /// создать экземпляр с настройками из Config
        /// </summary>
        /// <param name="conf"></param>
        public ContestFolder(FolderConfig conf)
        {
            this.cfg = conf;
        }

        /// <summary>
        /// загрузить все файлы с данными
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
                StreamReader fs = new StreamReader(file.FullName, Encoding.GetEncoding("windows-1251"));
                string s = "";
                while (s != null)
                {
                    s = fs.ReadLine();
                    try
                    {
                        ParseLine(s.Trim());
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// разбор одной строки файла
        /// </summary>
        /// <param name="s"></param>
        private void ParseLine(string s)
        {
            
        }
    }
}