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
        public DateTime PreviousQSODatetTime;

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
        /// предварительный расчет
        /// </summary>
        public void PreCalculate()
        {
            Calculate_Tours();
            Calculate_FolderFilter();
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
                    item.Counters.SetError(ErrorType.datetimeerror);
                }
            }
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
                    item.Counters.SetError(ErrorType.filteredband);
                }
                if (!ContestFolder.cfg.AllowModes.Check(item.Raw.Mode))
                {
                    item.Errors.Add("Filtered [mode]");
                    item.Counters.Filtered = true;
                    item.Counters.SetError(ErrorType.filteredmode);
                }
            }
        }

        /// <summary>
        /// расчет
        /// </summary>
        public void Calculate()
        {
            Calculate_Loop();
            Calculate_Double();
            Calculate_LastStand();
            Calculate_Points();
        }

        /// <summary>
        /// поиск повторов
        /// </summary>
        private void Calculate_Double()
        {
            string previous_callsign = "";
            foreach (QSO item in items)
            {
                //if (Call == "RN0CW" && item.Raw.Number == 32)
                //{
                //    Console.WriteLine(item.Dump());
                //}
                if (!item.Counters.Filtered && (!item.Counters.ErrorOnCheck ||
                    item.Counters.ErrorOnCheck &&
                    (item.Counters.ErrorType == ErrorType.doublebymode ||
                    item.Counters.ErrorType == ErrorType.doublebytime ||
                    item.Counters.ErrorType == ErrorType.similarqso))
                    )
                {
                    /// TODO: решить тут все вопросы по проверкам
                    /// В каждом подтуре, на каждом диапазоне с одним и тем же корреспондентом 
                    /// разрешается провести по две радиосвязи: одну радиосвязь – телеграфом CW, 
                    /// одну – телефоном (FM или PH)
                    if (Config.double_check == 1)
                    {
                        /// получили все связи с этим позывным в текущем туре 
                        /// ограничение: такой же ражим работы
                        QSOList list = items.GetPreviousInTour(item, true);
                        if (list.Count > 0)
                        {
                            QSO r = list[0];
                            // если предыдущая QSO подтверждена 
                            if (r.Counters.OK || r.Counters.ErrorType == ErrorType.doublebymode || r.Counters.ErrorType == ErrorType.doublebytime)
                            {
                                item.Counters.OK = false;
                                item.Errors.Clear();
                                item.Errors.Add("Subtour double [mode] with QSO: " + r.Raw.Number);
                                item.Counters.SetError(ErrorType.doublebymode);
                                item.LinkedQSO = r;
                            }
                            // если предыдущая QSO не подтверждена и она не предыдущий дубль то текущая QSO не может быть дублем
                            else if (r.Counters.OK == false && r.Counters.ErrorType != ErrorType.doublebymode && r.Counters.ErrorType != ErrorType.doublebytime)
                            {
                                // обнуляем флаг что текущая QSO дубль
                                item.Counters.OK = false;
                                item.Errors.Clear();
                                item.Counters.SetError(ErrorType.clear);
                                item.LinkedQSO = null;
                            }
                            #region debug
                            //if (list.Count > 1)
                            //{
                            //    Console.WriteLine("=2===============================================================");
                            //    Console.WriteLine(item.Dump());
                            //    foreach (QSO l in list)
                            //    {
                            //        Console.WriteLine(l.Dump());
                            //    }
                            //    //Console.ReadKey();
                            //}
                            #endregion 
                        }
                    }

                    ///// Повторную радиосвязь разными видами модуляции с одним и тем же корреспондентом 
                    ///// разрешается проводить не ранее, чем через 3 минуты после предыдущей связи, или 
                    ///// через одну радиосвязь, проведенную с другим корреспондентом.
                    if (Config.repeat_call > 0)
                    {
                        QSOList list = items.GetPreviousInTour(item, false);
                        if (list.Count > 0)
                        {
                            QSO r = list[0];
                            if (previous_callsign == item.Raw.RecvCall && r.Counters.Offset < Config.repeat_call && r.Counters.OK)
                            {
                                item.Counters.OK = false;
                                item.Errors.Clear();
                                item.Errors.Add("Subtour double [time] with QSO: " + r.Raw.Number);
                                item.Counters.SetError(ErrorType.doublebytime);
                                item.LinkedQSO = r;
                            }
                        }
                    }
                }
                previous_callsign = item.Raw.RecvCall;
            }
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
                    {
                        item.Errors.Add("No log [" + item.Raw.RecvCall + "]");
                        item.Counters.SetError(ErrorType.nolog);
                        ContestFolder.LostFiles.Add(item.Raw.RecvCall);
                    }
                    else
                    {
                        bool found = false;
                        // TODO: сделать нормальную связь
                        foreach (QSO q in file.items)
                        {
                            if (q.Counters.ErrorOnCheck == false &&
                                q.Counters.Filtered == false &&
                                q.Counters.OK == false)
                            {
                                if (Calculate_One(item, q, CalculateMode.withdate))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found)
                            Calculate_AI(item, file.items);
                    }
                }
            }
        }

        /// <summary>
        /// интелектуальное сравнение связи. сначала с оффсетом а потом с магией
        /// </summary>
        /// <param name="l"></param>
        /// <param name="qso_list"></param>
        private void Calculate_AI(QSO l, QSOList qso_list)
        {
            QSOList new_qso_list = new QSOList();
            foreach (QSO item in qso_list)
            {
                if (item.Counters.ErrorOnCheck == false && item.Counters.Filtered == false && item.Counters.OK == false)
                {
                    if (l.DateTime.AddMinutes(-Config.offset_min) <= item.DateTime && item.DateTime <= l.DateTime.AddMinutes(Config.offset_min))
                    {
                        new_qso_list.Add(item);
                    }
                }
            }
            bool found = false;
            foreach (QSO item in new_qso_list)
            {
                found = Calculate_One(l, item, CalculateMode.withoutdate);
                if (found)
                {
                    new_qso_list.Clear();
                    return;
                }
            }
            /// TODO: доделать ебаное интелектуальное сравнение
            /// вроде мысли появились, сравнивать блоками.
            /// типа сравнить дату время и позывные, потом моду, потом контрольные номера и локаторы
            if (!found)
            {
                foreach (QSO r in new_qso_list)
                {
                    /// странное сравнивание №1
                    /// сравниваем по дата время уже в возможных границах
                    /// диапазон, позывные, RST
                    ///              @           @       @     @                 @
                    /// <para>QSO: 28009 CW 2008-09-23 0711 UA1XYZ 599 001 123 UA2XYZ 599 001 AF 1</para> 
                    if (l.Raw.RecvCall == r.Raw.SendCall &&
                        l.Raw.SendCall == r.Raw.RecvCall &&
                        l.Feq == r.Feq)
                    {
                        l.Errors.Add(BuildErrorStr(l, r));
                        l.Counters.SetError(ErrorType.similarqso);
                        l.LinkedQSO = r;
                        if (!r.Counters.ErrorOnCheck)
                            r.LinkedQSO = l;
                        break;
                    }

                    #region странное сравнивание №2
                    ///// сравниваем по дата время уже в возможных границах
                    ///// диапазон, локатор в одну сторону и позывной в одну сторону
                    /////              @    @      @       @     
                    ///// <para>QSO: 28009 CW 2008-09-23 0711 UA1XYZ 599 001 LOCATOR UA2XYZ 599 001 LOCATOR 1</para> 
                    //if (((l.Raw.Mode == "CW" && r.Raw.Mode == "CW") || (l.Raw.Mode != "CW" && r.Raw.Mode != "CW")) &&
                    //    Util.AsNumeric(l.Raw.SendExch2) == Util.AsNumeric(r.Raw.RecvExch2) &&
                    //    l.Raw.RecvCall != r.Raw.SendCall &&
                    //    l.Feq == r.Feq)
                    //{
                    //    string ltext = "@[" + r.Raw.SendCall + " QSO:" + r.Raw.Number;

                    //    if (l.Raw.SendCall == r.Raw.RecvCall)
                    //    {
                    //        // переданный позывной и полученный
                    //        ltext = string.Concat(ltext, " [recv call: " + l.Raw.RecvCall + "/" + r.Raw.SendCall + "]");
                    //    }

                    //    if (Util.AsNumeric(l.Raw.SendRST) != Util.AsNumeric(r.Raw.RecvRST) ||
                    //        Util.AsNumeric(l.Raw.RecvRST) != Util.AsNumeric(r.Raw.SendRST))
                    //    {
                    //        // RST не совпало
                    //        if (Util.AsNumeric(l.Raw.SendRST) != Util.AsNumeric(r.Raw.RecvRST))
                    //            ltext = string.Concat(ltext, " [send rst: " + l.Raw.SendRST + "/" + r.Raw.RecvRST + "]");
                    //        if (Util.AsNumeric(l.Raw.RecvRST) != Util.AsNumeric(r.Raw.SendRST))
                    //            ltext = string.Concat(ltext, " [recv rst: " + l.Raw.RecvRST + "/" + r.Raw.SendRST + "]");
                    //    }

                    //    if (Util.AsNumeric(l.Raw.SendExch1) != Util.AsNumeric(r.Raw.RecvExch1) ||
                    //        Util.AsNumeric(l.Raw.RecvExch1) != Util.AsNumeric(r.Raw.SendExch1))
                    //    {
                    //        // не совпал 1 контрольный
                    //        if (Util.AsNumeric(l.Raw.SendExch1) != Util.AsNumeric(r.Raw.RecvExch1))
                    //            ltext = string.Concat(ltext, " [send exch1: " + l.Raw.SendExch1 + "/" + r.Raw.RecvExch1 + "]");
                    //        if (Util.AsNumeric(l.Raw.RecvExch1) != Util.AsNumeric(r.Raw.SendExch1))
                    //            ltext = string.Concat(ltext, " [recv exch1: " + l.Raw.RecvExch1 + "/" + r.Raw.SendExch1 + "]");
                    //    }

                    //    if (Util.AsNumeric(l.Raw.RecvExch2) != Util.AsNumeric(r.Raw.SendExch2))
                    //    {
                    //        // не совпал 2 контрольный
                    //        if (Util.AsNumeric(l.Raw.RecvExch2) != Util.AsNumeric(r.Raw.SendExch2))
                    //            ltext = string.Concat(ltext, " [recv exch2: " + l.Raw.RecvExch2 + "/" + r.Raw.SendExch2 + "]");
                    //    }

                    //    if (Util.AsNumeric(l.Raw.SendExch3) != Util.AsNumeric(r.Raw.RecvExch3) ||
                    //        Util.AsNumeric(l.Raw.RecvExch3) != Util.AsNumeric(r.Raw.SendExch3))
                    //    {
                    //        // не совпал 3 контрольный
                    //        if (Util.AsNumeric(l.Raw.SendExch3) != Util.AsNumeric(r.Raw.RecvExch3))
                    //            ltext = string.Concat(ltext, " [send exch3: " + l.Raw.SendExch3 + "/" + r.Raw.RecvExch3 + "]");
                    //        if (Util.AsNumeric(l.Raw.RecvExch3) != Util.AsNumeric(r.Raw.SendExch3))
                    //            ltext = string.Concat(ltext, " [recv exch3: " + l.Raw.RecvExch3 + "/" + r.Raw.SendExch3 + "]");
                    //    }
                    //    l.Errors.Add(ltext + "]");
                    //    l.Counters.ErrorOnCheck = true;
                    //    break;
                    //}
                    #endregion
                }
            }
        }

        public string BuildErrorStr(QSO l, QSO r)
        {
            string ltext = "@[" + r.Raw.SendCall + " QSO:" + r.Raw.Number;

            //if (l.Raw.Mode != r.Raw.Mode) {
            //    // мода не совпала
            //    ltext = string.Concat(ltext, " [mode: " + l.Raw.Mode + "/" + r.Raw.Mode + "]");
            //}

            if ((l.Raw.Mode == "CW" && r.Raw.Mode != "CW") ||
                (l.Raw.Mode != "CW" && r.Raw.Mode == "CW"))
            {
                // мода не совпала
                ltext = string.Concat(ltext, " [mode: " + l.Raw.Mode + "/" + r.Raw.Mode + "]");
            }

            if (Util.AsNumeric(l.Raw.SendRST) != Util.AsNumeric(r.Raw.RecvRST) ||
                Util.AsNumeric(l.Raw.RecvRST) != Util.AsNumeric(r.Raw.SendRST))
            {
                // RST не совпало
                if (Util.AsNumeric(l.Raw.SendRST) != Util.AsNumeric(r.Raw.RecvRST))
                    ltext = string.Concat(ltext, " [send rst: " + l.Raw.SendRST + "/" + r.Raw.RecvRST + "]");
                if (Util.AsNumeric(l.Raw.RecvRST) != Util.AsNumeric(r.Raw.SendRST))
                    ltext = string.Concat(ltext, " [recv rst: " + l.Raw.RecvRST + "/" + r.Raw.SendRST + "]");
            }

            if (Util.AsNumeric(l.Raw.SendExch1) != Util.AsNumeric(r.Raw.RecvExch1) ||
                Util.AsNumeric(l.Raw.RecvExch1) != Util.AsNumeric(r.Raw.SendExch1))
            {
                // не совпал 1 контрольный
                if (Util.AsNumeric(l.Raw.SendExch1) != Util.AsNumeric(r.Raw.RecvExch1))
                    ltext = string.Concat(ltext, " [send exch1: " + l.Raw.SendExch1 + "/" + r.Raw.RecvExch1 + "]");
                if (Util.AsNumeric(l.Raw.RecvExch1) != Util.AsNumeric(r.Raw.SendExch1))
                    ltext = string.Concat(ltext, " [recv exch1: " + l.Raw.RecvExch1 + "/" + r.Raw.SendExch1 + "]");
            }

            if (Util.AsNumeric(l.Raw.SendExch2) != Util.AsNumeric(r.Raw.RecvExch2) ||
                Util.AsNumeric(l.Raw.RecvExch2) != Util.AsNumeric(r.Raw.SendExch2))
            {
                // не совпал 2 контрольный
                if (Util.AsNumeric(l.Raw.SendExch2) != Util.AsNumeric(r.Raw.RecvExch2))
                    ltext = string.Concat(ltext, " [send exch2: " + l.Raw.SendExch2 + "/" + r.Raw.RecvExch2 + "]");
                if (Util.AsNumeric(l.Raw.RecvExch2) != Util.AsNumeric(r.Raw.SendExch2))
                    ltext = string.Concat(ltext, " [recv exch2: " + l.Raw.RecvExch2 + "/" + r.Raw.SendExch2 + "]");
            }

            if (Util.AsNumeric(l.Raw.SendExch3) != Util.AsNumeric(r.Raw.RecvExch3) ||
                Util.AsNumeric(l.Raw.RecvExch3) != Util.AsNumeric(r.Raw.SendExch3))
            {
                // не совпал 3 контрольный
                if (Util.AsNumeric(l.Raw.SendExch3) != Util.AsNumeric(r.Raw.RecvExch3))
                    ltext = string.Concat(ltext, " [send exch3: " + l.Raw.SendExch3 + "/" + r.Raw.RecvExch3 + "]");
                if (Util.AsNumeric(l.Raw.RecvExch3) != Util.AsNumeric(r.Raw.SendExch3))
                    ltext = string.Concat(ltext, " [recv exch3: " + l.Raw.RecvExch3 + "/" + r.Raw.SendExch3 + "]");
            }
            ltext = string.Concat(ltext, "]");
            return ltext;
        }

        public enum CalculateMode { withoutdate, withdate }

        /// <summary>
        /// проверка двух связей и возможно связывание
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="CompareDateTime"></param>
        /// <returns></returns>
        private bool Calculate_One(QSO l, QSO r, CalculateMode mode)
        {
            // совпадение всех полей кроме RST
            if (l.Raw.RecvCall == r.Raw.SendCall &&
              l.Raw.SendCall == r.Raw.RecvCall &&
              l.Feq == r.Feq &&
              //(l.Raw.Mode == r.Raw.Mode) &&
              ((l.Raw.Mode == r.Raw.Mode && l.Raw.Mode == "CW") || (l.Raw.Mode != "CW" && r.Raw.Mode != "CW")) &&
              Util.AsNumeric(l.Raw.SendExch1) == Util.AsNumeric(r.Raw.RecvExch1) &&
              Util.AsNumeric(l.Raw.SendExch2) == Util.AsNumeric(r.Raw.RecvExch2) &&
              Util.AsNumeric(l.Raw.SendExch3) == Util.AsNumeric(r.Raw.RecvExch3) &&
              Util.AsNumeric(l.Raw.RecvExch1) == Util.AsNumeric(r.Raw.SendExch1) &&
              Util.AsNumeric(l.Raw.RecvExch2) == Util.AsNumeric(r.Raw.SendExch2) &&
              Util.AsNumeric(l.Raw.RecvExch3) == Util.AsNumeric(r.Raw.SendExch3) &&
              (mode == CalculateMode.withoutdate || (mode == CalculateMode.withdate && l.DateTime == r.DateTime)) &&
              (Config.rst_check == 0 || (Config.rst_check == 1 &&
              Util.AsNumeric(l.Raw.SendRST) == Util.AsNumeric(r.Raw.RecvRST) &&
              Util.AsNumeric(l.Raw.RecvRST) == Util.AsNumeric(r.Raw.SendRST))) &&
              Coordinate.ValidateLocator(l.Raw.SendExch2) == true &&
              Coordinate.ValidateLocator(l.Raw.RecvExch2) == true)
            // TODO: где то нужно сделать правильно проверку локаторов выше две строки
            {
                //l.Errors.Clear();
                //r.Errors.Clear();
                if (mode == CalculateMode.withdate)
                {
                    l.Errors.Add("=[" + r.Raw.SendCall + " QSO:" + r.Raw.Number + " " + r.DateTime.ToString("HHmm") + "]");
                    r.Errors.Add("=[" + l.Raw.SendCall + " QSO:" + l.Raw.Number + " " + l.DateTime.ToString("HHmm") + "]");
                }
                else
                {
                    l.Errors.Add("±[" + r.Raw.SendCall + " QSO:" + r.Raw.Number + " " + r.DateTime.ToString("HHmm") + "]");
                    r.Errors.Add("±[" + l.Raw.SendCall + " QSO:" + l.Raw.Number + " " + l.DateTime.ToString("HHmm") + "]");
                }
                if (Util.AsNumeric(l.Raw.SendRST) != Util.AsNumeric(r.Raw.RecvRST))
                {
                    l.Errors.Add("Partner warning (" + l.Raw.SendRST + "/" + r.Raw.RecvRST + ")");
                    r.Errors.Add("Receive warning (" + l.Raw.SendRST + "/" + r.Raw.RecvRST + ")");
                }
                if (Util.AsNumeric(l.Raw.RecvRST) != Util.AsNumeric(r.Raw.SendRST))
                {
                    l.Errors.Add("Receive warning (" + r.Raw.SendRST + "/" + l.Raw.RecvRST + ")");
                    r.Errors.Add("Partner warning (" + r.Raw.SendRST + "/" + l.Raw.RecvRST + ")");
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
        /// последний расчет. закрываем пустышки по возможности.
        /// </summary>
        private void Calculate_LastStand()
        {
            foreach (QSO l in items)
            {
                if (l.Counters.ErrorOnCheck == false &&
                    l.Counters.OK == false &&
                    l.Errors.Count == 0)
                {
                    // получить лог корреспондента
                    ContestFile file = ContestFolder.Get(l.Raw.RecvCall);
                    if (file != null)
                    {
                        foreach (QSO r in file.items)
                        {
                            if (r.Counters.ErrorOnCheck == true &&
                                r.Counters.OK == false)
                            {
                                if (r.LinkedQSO != null && r.LinkedQSO.Equals(l) && r.Errors.Count > 0)
                                {
                                    l.Errors.Add(BuildErrorStr(l, r));
                                    l.Counters.SetError(ErrorType.correrror);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// расчет очков
        /// </summary>
        private void Calculate_Points()
        {
            AllQty = 0;
            ClaimQty = 0;
            OKQty = 0;
            TotalPoints = 0;
            foreach (QSO item in items)
            {
                if (!item.Counters.Filtered)
                    ClaimQty++;
                if (item.Counters.OK)
                {
                    // TODO: в этом месте локатор должен проверяться в связи с настройками 
                    item.Counters.Distantion = Math.Round(Coordinate.GetDistance(item.Raw.SendExch2, item.Raw.RecvExch2), 0);
                    if (item.Counters.Distantion == 0) { item.Counters.Distantion = 1; }
                    item.Counters.ByLocator = Math.Round(item.Counters.Distantion * Config.ContestBandPoints.GetPoints(item.Feq), 0);
                    item.Counters.Total = item.Counters.ByLocator;
                    OKQty++;
                }
                else
                {
                    item.Counters.Distantion = 0;
                    item.Counters.ByLocator = 0;
                    item.Counters.Total = 0;
                }
                TotalPoints += item.Counters.Total;
                AllQty++;
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
            string filename = path + @"\" + Config.folder_ubn + @"\" + Path.GetFileNameWithoutExtension(FullName) + " Claim.txt";
            File.Delete(filename);
            StreamWriter fw = new StreamWriter(filename, true);

            fw.WriteLine("CALLSIGN: " + header.callsign, Encoding.GetEncoding("Windows-1251"));
            fw.WriteLine("NAME: " + header.name, Encoding.GetEncoding("Windows-1251"));
            foreach (string s in header.address)
            {
                fw.WriteLine("ADDRESS: " + s, Encoding.GetEncoding("Windows-1251"));
            }
            fw.WriteLine("\n" + string.Concat(Enumerable.Repeat("-", 84)) + "\n", Encoding.GetEncoding("Windows-1251"));

            foreach (QSO q in items)
            {
                string s = q.ToReport() + " : ";
                if (q.Errors.Count > 0)
                {
                    s = string.Concat(s, string.Join(", ", q.Errors));
                }
                if (q.CheckErrors.Count > 0)
                {
                    s = string.Concat(s, string.Join(", ", q.CheckErrors));
                }
                fw.WriteLine(s, Encoding.GetEncoding("Windows-1251"));
            }
            fw.WriteLine("Total points: " + TotalPoints, Encoding.GetEncoding("Windows-1251"));
            fw.Close();
        }

        /// <summary>
        /// запись файла UBN
        /// </summary>
        private void SaveUBN()
        {
            string path = Path.GetDirectoryName(FullName);
            Directory.CreateDirectory(path + @"\" + Config.folder_ubn);
            string filename = path + @"\" + Config.folder_ubn + @"\" + Path.GetFileNameWithoutExtension(FullName) + " UBN.txt";
            File.Delete(filename);
            StreamWriter fw = new StreamWriter(filename, true);

            fw.WriteLine("CALLSIGN: " + header.callsign, Encoding.GetEncoding("Windows-1251"));
            fw.WriteLine("NAME: " + header.name, Encoding.GetEncoding("Windows-1251"));
            foreach (string s in header.address)
            {
                fw.WriteLine("ADDRESS: " + s, Encoding.GetEncoding("Windows-1251"));
            }
            fw.WriteLine("\n" + string.Concat(Enumerable.Repeat("-", 84)) + "\n", Encoding.GetEncoding("Windows-1251"));
            fw.WriteLine("UBN reports:\n", Encoding.GetEncoding("Windows-1251"));

            int in_ubn = 0;
            foreach (QSO q in items)
            {
                if (q.Counters.OK == false)
                {
                    string s = q.ToReport(false) + " : ";
                    if (q.Errors.Count > 0)
                    {
                        s = string.Concat(s, string.Join(", ", q.Errors));
                    }
                    if (q.CheckErrors.Count > 0)
                    {
                        s = string.Concat(s, string.Join(", ", q.CheckErrors));
                    }
                    fw.WriteLine(s, Encoding.GetEncoding("Windows-1251"));
                    in_ubn++;
                }
            }
            fw.WriteLine("\nTotal QSO with errors: " + in_ubn, Encoding.GetEncoding("Windows-1251"));
            fw.Close();
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
                foreach (string s in q.CheckErrors)
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
