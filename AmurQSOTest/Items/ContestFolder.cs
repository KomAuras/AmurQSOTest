using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public List<ContestFile> Files = new List<ContestFile>();
        /// <summary>
        /// потерянные файлы/позывные - нет отчетов
        /// </summary>
        public LostFiles LostFiles = new LostFiles();

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
            SaveLostFiles();
            SaveReports();
        }

        /// <summary>
        /// основной отчет
        /// </summary>
        private void SaveReports()
        {
            List<ReportMain> rep = new List<ReportMain>();
            Directory.CreateDirectory(Config.AppFolder() + Config.ContestPath + @"\" + cfg.Name + @"\" + Config.folder_report);
            string filename = Config.AppFolder() + Config.ContestPath + @"\" + cfg.Name + @"\" + Config.folder_report + @"\" + "MainReport.txt";
            File.Delete(filename);
            StreamWriter fw = new StreamWriter(filename, true);
            foreach (ContestFile f in Files)
            {
                ReportMain item = new ReportMain();
                item.Call = f.Call;
                item.AllQty = f.AllQty;
                item.ClaimQty = f.ClaimQty;
                item.OKQty = f.OKQty;
                item.TotalPoints = f.TotalPoints;
                rep.Add(item);
            }
            fw.WriteLine("Folder: " + cfg.Name);
            fw.WriteLine("Description: " + cfg.Desc);
            fw.WriteLine("Band: " + cfg.AllowBands.ToString());
            fw.WriteLine("Modes: " + cfg.AllowModes.ToString());
            fw.WriteLine(string.Format("\n{0,-10} {1,10} {2,10} {3,10} {4,10}", new string[5] { "Call", "All", "Claim", "OK", "Total" }));
            fw.WriteLine(string.Concat(Enumerable.Repeat("-", 54)));
            foreach (ReportMain item in rep.OrderByDescending(o => o.TotalPoints).ToList())
            {
                fw.WriteLine(string.Format("{0,-10} {1,10} {2,10} {3,10} {4,10}", item.Call, item.AllQty, item.ClaimQty, item.OKQty, item.TotalPoints), Encoding.GetEncoding("Windows-1251"));
            }
            fw.Close();
        }

        /// <summary>
        /// записать отчет по потерянным файлам
        /// </summary>
        private void SaveLostFiles()
        {
            Directory.CreateDirectory(Config.AppFolder() + Config.ContestPath + @"\" + cfg.Name + @"\" + Config.folder_report);
            string filename = Config.AppFolder() + Config.ContestPath + @"\" + cfg.Name + @"\" + Config.folder_report + @"\" + "LostFiles.txt";
            File.Delete(filename);
            StreamWriter fw = new StreamWriter(filename, true);
            foreach (LostFile item in LostFiles.OrderByDescending(o => o.Count).ToList())
            {
                fw.WriteLine(string.Format("{0,-" + LostFiles.Width + "} = {1}", item.Call, item.Count), Encoding.GetEncoding("Windows-1251"));
            }
            fw.Close();
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