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
        public string Call = "";
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
        public QSOList items = new QSOList();

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

        /// <summary>
        /// ширины всех полей QSO.Raw
        /// </summary>
        public int[] Width { get; set; }

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
            Calculate_Tours();
            Calculate_FolderFilter();
            Calculate_Double();
            Calculate_Loop();
            //Calculate_Points();
        }

        /// <summary>
        /// основное связывание радиосвязей
        /// </summary>
        private void Calculate_Loop()
        {
            foreach (QSO item in items)
            {
                if (item.Counters.ErrorOnCheck == false &&
                    item.Counters.Filtered == false &&
                    item.Counters.OK == false)
                {
                    // получить лог корреспондента
                    ContestFile file = ContestFolder.Get(item.Raw.RecvCall);
                    if (file == null)
                        item.Errors.Add("No log [" + item.Raw.RecvCall + "]");
                    else
                    {
                        // TODO: сделать нормальную связь
                        foreach (QSO q in file.items)
                        {
                            if (q.Counters.ErrorOnCheck == false &&
                                q.Counters.Filtered == false &&
                                q.Counters.OK == false)
                            {
                                if (Calculate_One(item, q))
                                    break;
                            }
                        }
                        //Calculate_AI(item, file);
                    }
                }
            }
        }

        /// <summary>
        /// проверка двух связей и возможно связывание
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="CompareDateTime"></param>
        /// <returns></returns>
        private bool Calculate_One(QSO l, QSO r, bool CompareDateTime = true)
        {
            // совпадение всех полей кроме RST
            if (l.Raw.RecvCall == r.Raw.SendCall &&
              l.Raw.SendCall == r.Raw.RecvCall &&
              l.Feq == r.Feq &&
              l.Raw.Mode == r.Raw.Mode &&
              Util.AsNumeric(l.Raw.SendExch1) == Util.AsNumeric(r.Raw.RecvExch1) &&
              Util.AsNumeric(l.Raw.SendExch2) == Util.AsNumeric(r.Raw.RecvExch2) &&
              Util.AsNumeric(l.Raw.SendExch3) == Util.AsNumeric(r.Raw.RecvExch3) &&
              Util.AsNumeric(l.Raw.RecvExch1) == Util.AsNumeric(r.Raw.SendExch1) &&
              Util.AsNumeric(l.Raw.RecvExch2) == Util.AsNumeric(r.Raw.SendExch2) &&
              Util.AsNumeric(l.Raw.RecvExch3) == Util.AsNumeric(r.Raw.SendExch3) &&
              (CompareDateTime == false || (CompareDateTime == true && l.DateTime == r.DateTime)) &&
              (Config.rst_check == 0 || (Config.rst_check == 1 &&
              Util.AsNumeric(l.Raw.SendRST) == Util.AsNumeric(r.Raw.RecvRST) &&
              Util.AsNumeric(l.Raw.RecvRST) == Util.AsNumeric(r.Raw.SendRST))) &&
              Coordinate.ValidateLocator(l.Raw.SendExch2) == true &&
              Coordinate.ValidateLocator(l.Raw.RecvExch2) == true)
            // TODO: где то нужно сделать правильно проверку локаторов выше две строки
            {
                l.Errors.Clear();
                r.Errors.Clear();
                if (CompareDateTime)
                {
                    l.Errors.Add("=[" + r.Raw.SendCall + " QSO:" + r.Raw.Number + "]");
                    r.Errors.Add("=[" + l.Raw.SendCall + " QSO:" + l.Raw.Number + "]");
                }
                else
                {
                    l.Errors.Add("±[" + r.Raw.SendCall + " QSO:" + r.Raw.Number + "]");
                    r.Errors.Add("±[" + l.Raw.SendCall + " QSO:" + l.Raw.Number + "]");
                }
                if (Int32.Parse(l.Raw.SendRST) != Int32.Parse(r.Raw.RecvRST))
                {
                    l.Errors.Add("Partner warning (" + l.Raw.SendRST + " QSO:" + r.Raw.RecvRST + ")");
                    r.Errors.Add("Receive warning (" + l.Raw.SendRST + " QSO:" + r.Raw.RecvRST + ")");
                }
                if (Int32.Parse(l.Raw.RecvRST) != Int32.Parse(r.Raw.SendRST))
                {
                    l.Errors.Add("Receive warning (" + r.Raw.SendRST + " QSO:" + l.Raw.RecvRST + ")");
                    r.Errors.Add("Partner warning (" + r.Raw.SendRST + " QSO:" + l.Raw.RecvRST + ")");
                }
                l.LinkedQSO = r;
                l.Counters.OK = true;
                r.LinkedQSO = l;
                r.Counters.OK = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// поиск повторов. самое крутое гавно в программе!!!
        /// </summary>
        private void Calculate_Double()
        {
            int subtour_number = 0;
            string previous_callsign = "";
            QSOList this_subtour = new QSOList();
            QSO found_qso;
            foreach (QSO item in items)
            {
                if (!item.Counters.Filtered && !item.Counters.ErrorOnCheck)
                {
                    if (subtour_number != item.Counters.SubTour)
                        this_subtour.Clear();
                    subtour_number = item.Counters.SubTour;

                    /// TODO: Решить тут все вопросы по проверкам
                    /// т.к. некоторых нет в настройках!

                    /// В каждом подтуре, на каждом диапазоне с одним и тем же корреспондентом 
                    /// разрешается провести по две радиосвязи: одну радиосвязь – телеграфом CW, 
                    /// одну – телефоном (FM или SSB)

                    // находим связь с этим корреспондентом в этом туре на этом диапазоне(частоте)
                    found_qso = null;
                    if (Config.double_check == 1)
                    {
                        if (this_subtour.GetPrevious(item, out found_qso))
                        {
                            // если текущая CW и была связь на CW
                            if ((item.Raw.Mode == "CW" && item.Raw.Mode == found_qso.Raw.Mode) ||
                                // если текущая не CW и была связь не CW
                                (item.Raw.Mode != "CW" && found_qso.Raw.Mode != "CW"))
                                item.Errors.Add("Subtour double [mode] with QSO: " + found_qso.Raw.Number);
                        }
                    }

                    ///// Повторную радиосвязь разными видами модуляции с одним и тем же корреспондентом 
                    ///// разрешается проводить не ранее, чем через 3 минуты после предыдущей связи, или 
                    ///// через одну радиосвязь, проведенную с другим корреспондентом.

                    //if (Config.repeat_call > 0)
                    //{
                    //    // сколько прошло времени с предыдущей связи этого позывного
                    //    int Minutes = this_subtour.GetOffsetMinutes(item, out found_qso);
                    //    if (found_qso != null)
                    //        // предыдущая связь, по времени можно или не предыдущая связь
                    //        if (!(((previous_callsign == item.Raw.RecvCall && Minutes >= Config.repeat_call) || previous_callsign != item.Raw.RecvCall)
                    //            // текущий CW и был не CW
                    //            && ((item.Raw.Mode == "CW" && found_qso.Raw.Mode != "CW") ||
                    //            // текущий не CW и был не CW
                    //            (item.Raw.Mode != "CW" && found_qso.Raw.Mode != "CW"))))
                    //        {
                    //            item.Errors.Add("Subtour double [time] with QSO:" + found_qso.Raw.Number);
                    //        }
                    //}
                }
                this_subtour.Add(item);
                previous_callsign = item.Raw.RecvCall;
            }
            this_subtour.Clear();
        }

        /// <summary>
        /// фильтрация QSO по настройкам папки. диапазоны и режимы работы
        /// </summary>
        private void Calculate_FolderFilter()
        {
            foreach (QSO item in items)
            {
                if (!ContestFolder.cfg.AllowBands.Check(item.Feq))
                {
                    item.Errors.Add("Filtered [band]");
                    item.Counters.Filtered = true;
                }
                if (!ContestFolder.cfg.AllowModes.Check(item.Raw.Mode))
                {
                    item.Errors.Add("Filtered [mode]");
                    item.Counters.Filtered = true;
                }
            }
        }

        /// <summary>
        /// запись номеров туров в QSO
        /// </summary>
        private void Calculate_Tours()
        {
            foreach (QSO item in items)
            {
                int subtour_number = Config.SubTours.Inside(item.DateTime);
                if (subtour_number > 0)
                    item.Counters.SubTour = subtour_number;
                else
                {
                    item.Errors.Add("Error in date/time [" + item.Raw.Date + " " + item.Raw.Time + "]");
                    item.Counters.ErrorOnCheck = true;
                }
            }
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
            foreach (QSO q in items)
            {
                string s = q.ToString();
                if (q.Errors.Count > 0)
                {
                    s = string.Concat(s, " ::: " + string.Join(", ", q.Errors));
                }
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
                if (s != null && s.Length > 0)
                    ParseLine(s.Trim());
            }
            GetQSOWidth();
        }

        /// <summary>
        /// получить ширины всех НУЖНЫХ полей в QSO
        /// </summary>
        private void GetQSOWidth()
        {
            if (items.Count > 0)
            {
                Width = items[0].Raw.GetWidths();
                foreach (QSO item in items)
                {
                    for (int i = 0; i < Width.Length; i++)
                    {
                        if (item.Raw.GetWidths()[i] > Width[i])
                            Width[i] = item.Raw.GetWidths()[i];
                    }
                }
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
                    Call = header.callsign;
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
