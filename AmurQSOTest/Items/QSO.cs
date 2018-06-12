using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// Одна запись QSO
    /// </summary>
    public class QSO
    {
        /// <summary>
        /// ссылка на класс файла
        /// </summary>
        public ContestFile ContestFile;
        /// <summary>
        /// сырые данные по QSO
        /// </summary>
        public RawQSO Raw = new RawQSO();
        /// <summary>
        /// ошибки в QSO
        /// </summary>
        public List<string> Errors = new List<string>();
        /// <summary>
        /// ошибки разбора QSO
        /// </summary>
        public List<string> CheckErrors = new List<string>();
        /// <summary>
        /// частота числом - band!
        /// </summary>
        public int Feq;
        /// <summary>
        /// Дата и время связи
        /// </summary>
        public DateTime DateTime;
        /// <summary>
        /// счетчики QSO
        /// </summary>
        public QSOCounters Counters = new QSOCounters();
        /// <summary>
        /// ссылка на QSO корреспондента
        /// </summary>        
        public QSO LinkedQSO;

        string[] fields;

        /// <summary>
        /// создать экземпляр с парсингом данных из строки
        /// </summary>
        /// <param name="s"></param>
        public QSO(ContestFile cf, string s, int qso_number)
        {
            ContestFile = cf;
            Raw.Number = qso_number;
            Parse(s);
        }

        /// <summary>
        /// проверить локатор в поле
        /// </summary>
        /// <param name="sendExch1"></param>
        /// <param name="v"></param>
        private void CheckLocator(string sendExch1, string v)
        {
            if (Coordinate.ValidateLocator(sendExch1))
            {
                CheckErrors.Add("Error in " + v);
                Counters.ErrorOnCheck = true;
            }
        }

        /// <summary>
        /// получить поле из разобранного массива. позиция поля и имя для ошибки если поле пустое
        /// </summary>
        /// <param name="field_position"></param>
        /// <param name="field_name"></param>
        /// <returns></returns>
        private string GetField(int field_position, string field_name)
        {
            if (fields.Count() > field_position)
            {
                return fields[field_position];
            }
            else
            {
                if (field_name.Length > 0)
                {
                    CheckErrors.Add("Empty " + field_name);
                    Counters.ErrorOnCheck = true;
                }
            }
            return "";
        }

        /// <summary>
        /// разобрать строку и выбрать все данные о связи
        /// <para>0    1     2  3          4    5      6   7   8   9      10  11  12 13</para>    
        /// <para>QSO: 28009 CW 2008-09-23 0711 UA1XYZ 599 001 123 UA2XYZ 599 001 AF 1</para> 
        /// </summary>
        /// <param name="s"></param>
        private void Parse(string s)
        {
            fields = s.ToUpper().Split(new char[] { ' ', '\t' }).Where(z => z != string.Empty).ToArray();
            int field_position = 0;

            // частота
            field_position++;
            Raw.Feq = GetField(field_position, "feq");
            if (!Int32.TryParse(Raw.Feq, out int temp_feq))
            {
                CheckErrors.Add("Bad feq [" + Raw.Feq + "]");
                Counters.ErrorOnCheck = true;
            }
            // проверка соответствия диапазона и частоты
            if (Standards.Bands.Check(temp_feq, out temp_feq))
            {
                CheckErrors.Add("Not specified feq [" + temp_feq + "]");
                Counters.ErrorOnCheck = true;
            }
            Feq = temp_feq;

            // вид работы
            field_position++;
            Raw.Mode = GetField(field_position, "mode");
            if (Standards.Modes.Check(Raw.Mode))
            {
                CheckErrors.Add("Not specified mode [" + Raw.Mode + "]");
                Counters.ErrorOnCheck = true;
            }

            // дата и время
            field_position++;
            Raw.Date = GetField(field_position, "date");
            field_position++;
            Raw.Time = GetField(field_position, "time");
            bool t = DateTime.TryParseExact(Raw.Date.Trim() + " " + Raw.Time.Trim(), "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime);
            if (DateTime == DateTime.MinValue)
            {
                CheckErrors.Add("Not specified DateTime [" + Raw.Date.Trim() + " " + Raw.Time.Trim() + "]");
                Counters.ErrorOnCheck = true;
            }

            // отправленные данные
            field_position++;
            Raw.SendCall = GetField(field_position, "send call");
            if (ContestFile.Call == "")
                ContestFile.Call = Raw.SendCall;
            field_position++;
            Raw.SendRST = GetField(field_position, "send RST");
            if (Config.control > 0)
            {
                field_position++;
                Raw.SendExch1 = GetField(field_position, "send Exch1");
                if (Config.control_loc == 1)
                    CheckLocator(Raw.SendExch1, "send Locator");
            }
            if (Config.control > 1)
            {
                field_position++;
                Raw.SendExch2 = GetField(field_position, "send Exch2");
                if (Config.control_loc == 2)
                    CheckLocator(Raw.SendExch1, "send Locator");
            }
            if (Config.control > 2)
            {
                field_position++;
                Raw.SendExch3 = GetField(field_position, "send Exch3");
                if (Config.control_loc == 3)
                    CheckLocator(Raw.SendExch1, "send Locator");
            }

            // принятые данные
            field_position++;
            Raw.RecvCall = GetField(field_position, "recv call");
            field_position++;
            Raw.RecvRST = GetField(field_position, "recv RST");
            if (Config.control > 0)
            {
                field_position++;
                Raw.RecvExch1 = GetField(field_position, "recv Exch1");
                if (Config.control_loc == 1)
                    CheckLocator(Raw.SendExch1, "recv Locator");
            }
            if (Config.control > 1)
            {
                field_position++;
                Raw.RecvExch2 = GetField(field_position, "recv Exch2");
                if (Config.control_loc == 2)
                    CheckLocator(Raw.SendExch1, "recv Locator");
            }
            if (Config.control > 2)
            {
                field_position++;
                Raw.RecvExch3 = GetField(field_position, "recv Exch3");
                if (Config.control_loc == 3)
                    CheckLocator(Raw.SendExch1, "recv Locator");
            }
            field_position++;
            Raw.Mo2t = GetField(field_position, "");

            //if (Counters.ErrorOnCheck)
            //    Folder.Contest.FormatOK = false
        }

        public override string ToString()
        {
            Type type = this.Raw.GetType();

            string s = "";
            // вывести номер тура
            //string s="" + Counters.SubTour + "< ";

            s = string.Concat(s, string.Format("QSO {0,-" + ContestFile.Width[0] + "} : ", Raw.Number));

            // первые 6 полей.
            for (int i = 1; i < 7; i++) {
                s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[i] + "} ", Util.GetProperty(this.Raw, type.GetProperties()[i].Name).ToString()));
            }

            if (Config.control > 0)
                s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[7] + "} ", Raw.SendExch1));

            if (Config.control > 1)
                s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[8] + "} ", Raw.SendExch2));

            if (Config.control > 2)
                s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[9] + "} ", Raw.SendExch3));

            s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[10] + "} ", Raw.RecvCall));
            s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[11] + "} ", Raw.RecvRST));

            if (Config.control > 0)
                s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[12] + "} ", Raw.RecvExch1));

            if (Config.control > 1)
                s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[13] + "} ", Raw.RecvExch2));

            if (Config.control > 2)
                s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[14] + "} ", Raw.RecvExch3));

            s = string.Concat(s, string.Format("{0,-" + ContestFile.Width[15] + "} ", Raw.Mo2t));

            /// TODO: Очки по строке QSO
            s = string.Concat(s, string.Format("{0,5} ", Counters.Distantion));
            s = string.Concat(s, string.Format("{0,5} ", Counters.ByLocator));
            s = string.Concat(s, string.Format("{0,5}", Counters.Total));

            s = string.Concat(s, string.Format(" err:{0} ", Counters.ErrorOnCheck ? "1" : "0"));
            s = string.Concat(s, string.Format("ok:{0} ", Counters.OK ? "1" : "0"));

            return s;
        }
    }
}
