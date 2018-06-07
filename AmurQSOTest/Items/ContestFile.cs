using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// файл с QSO связями
    /// </summary>
    public class ContestFile
    {
        /// <summary>
        /// ссылка на папку с файлами
        /// </summary>        
        public ContestFolder ContestFolder;
        /// <summary>
        /// полный путь к файлу
        /// </summary>
        private string FullName;
        /// <summary>
        /// позывной файла
        /// </summary>
        public string Call;
        /// <summary>
        /// заголовок файла с данными
        /// </summary>
        public ContestFileHeader header = new ContestFileHeader();
        /// <summary>
        /// ошибки в файле
        /// </summary>
        public List<string> Errors = new List<string>();
        /// <summary>
        /// связи QSO
        /// </summary>
        public List<QSO> items = new List<QSO>();

        // счетчики внутренние

        /// <summary>
        /// заявлено записей
        /// </summary>
        public int ClaimQty;
        /// <summary>
        /// посчитанно записей
        /// </summary>
        public int OKQty;
        /// <summary>
        /// всего
        /// </summary>
        public int AllQty;
        /// <summary>
        /// всего очков
        /// </summary>
        public double TotalPoints;

        private bool CabrilloExist = false;
        private int qso_number = 0;

        /// <summary>
        /// создать экземпляр класса и выполнить загрузку
        /// </summary>
        /// <param name="FullName"></param>
        public ContestFile(ContestFolder cf, string FullName)
        {
            ContestFolder = cf;
            Load(FullName);
        }

        /// <summary>
        /// расчет
        /// </summary>
        public void Calculate()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// запись
        /// </summary>
        public void Save()
        {
            SaveCheck();
            SaveClaim();
            SaveUBN();
        }
        
        /// <summary>
        /// запись файла Claim
        /// </summary>
        private void SaveClaim()
        {
            string path = Path.GetDirectoryName(FullName);
            Directory.CreateDirectory(path + @"\" + Config.folder_ubn);
            string filename = path + @"\" + Config.folder_ubn + @"\" + Path.GetFileNameWithoutExtension(FullName) + " UBN.txt";
            File.Delete(filename);
            StreamWriter fw = new StreamWriter(filename, true);
            foreach(QSO q in items) {
                string s = q.ToString();
                fw.WriteLine(s, Encoding.GetEncoding("Windows-1251"));
            }
            fw.Close();
        }

        /// <summary>
        /// запись файла UBN
        /// </summary>
        private void SaveUBN()
        {
        }

        /// <summary>
        /// запись файла предварительной проверки
        /// </summary>
        private void SaveCheck()
        {
            string path = Path.GetDirectoryName(FullName);
            Directory.CreateDirectory(path + @"\" + Config.folder_check);
            string filename = path + @"\" + Config.folder_check + @"\" + Path.GetFileNameWithoutExtension(FullName) + " check.txt";
            File.Delete(filename);
            List<string> temp = new List<string>();
            foreach (string s in Errors)
            {
                temp.Add(s);
            }
            foreach (QSO q in items)
            {
                foreach (string s in q.Errors)
                {
                    temp.Add(s);
                }
            }
            if (temp.Count > 0)
            {
                StreamWriter fw = new StreamWriter(filename, true);
                foreach (string s in temp)
                    fw.WriteLine(s, Encoding.GetEncoding("Windows-1251"));
                fw.Close();
            }
        }

        /// <summary>
        /// загрузить данные
        /// </summary>
        /// <param name="FullName"></param>
        private void Load(string FullName)
        {
            Console.WriteLine("Файл: " + Path.GetFileName(FullName));
            this.FullName = FullName;
            StreamReader fs = new StreamReader(FullName, Encoding.GetEncoding("windows-1251"));
            string s = "";
            while (s != null)
            {
                s = fs.ReadLine();
                if (s != null)
                    ParseLine(s.Trim());
            }
        }

        /// <summary>
        /// разбор одной строки файла
        /// </summary>
        /// <param name="s"></param>
        private void ParseLine(string s)
        {
            // начало файла
            if (Util.StrExists(s, "start-of-log:"))
                CabrilloExist = true;
            if (CabrilloExist)
            {
                // строка одной связи
                if (Util.StrExists(s, "qso:"))
                {
                    qso_number++;
                    items.Add(new QSO(this, s, qso_number));
                }

                // несколько строчные 
                if (Util.StrExists(s, "address:"))
                {
                    header.address.Add(s.Substring(s.IndexOf(':') + 1).Trim());
                }
                if (Util.StrExists(s, "operators:"))
                {
                    header.operators.Add(s.Substring(s.IndexOf(':') + 1).Trim());
                }
                if (Util.StrExists(s, "soapbox:"))
                {
                    header.soapbox.Add(s.Substring(s.IndexOf(':') + 1).Trim());
                }

                // однострочные
                if (Util.StrExists(s, "callsign:"))
                {
                    header.callsign = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-assisted:"))
                {
                    header.category_assisted = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-band:"))
                {
                    header.category_band = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-mode:"))
                {
                    header.category_mode = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-operator:"))
                {
                    header.category_operator = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-overlay:"))
                {
                    header.category_overlay = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-power:"))
                {
                    header.category_power = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-station:"))
                {
                    header.category_station = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-time:"))
                {
                    header.category_time = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "category-transmitter:"))
                {
                    header.category_transmitter = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "claimed-score:"))
                {
                    header.claimed_score = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "club:"))
                {
                    header.club = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "contest:"))
                {
                    header.contest = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "email:"))
                {
                    header.email = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "location:"))
                {
                    header.location = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "name:"))
                {
                    header.name = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "offtime:"))
                {
                    header.offtime = s.Substring(s.IndexOf(':') + 1).Trim();
                }
                if (Util.StrExists(s, "created-by:"))
                {
                    header.created_by = s.Substring(s.IndexOf(':') + 1).Trim();
                }
            }
        }
    }
}
