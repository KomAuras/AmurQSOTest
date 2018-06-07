using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        /// частота числом
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
        public QSO RecvQSO;

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
        /// разобрать строку и выбрать все данные о связи
        /// <para>QSO: 28009 CW 2008-09-23 0711 UA1XYZ 599 001 123 UA2XYZ 599 001 AF 1</para> 
        /// </summary>
        /// <param name="s"></param>
        private void Parse(string s)
        {
            string[] fields = s.Split(new char[] { ' ', '\t' }).Where(z => z != string.Empty).ToArray();

            // частота
            try
            {
                Raw.Feq = fields[1];
            }
            catch
            {
                Errors.Add("Empty feq");
                Counters.ErrorOnCheck = true;
            }
            if (!Int32.TryParse(Raw.Feq, out int temp_feq))
            {
                Errors.Add("Bad feq [" + Raw.Feq + "]");
                Counters.ErrorOnCheck = true;
            }
            if (Standards.Bands.Check(temp_feq, out temp_feq))
            {
                Errors.Add("Not specified feq [" + temp_feq + "]");
                Counters.ErrorOnCheck = true;
            }
            Feq = temp_feq;

            // вид работы
            try
            {
                Raw.Mode = fields[2];
            }
            catch
            {
                Errors.Add("Empty mode");
                Counters.ErrorOnCheck = true;
            }
            if (Standards.Modes.Check(Raw.Mode))
            {
                Errors.Add("Not specified mode [" + Raw.Mode + "]");
                Counters.ErrorOnCheck = true;
            }

            // дата и время
            try
            {
                Raw.Date = fields[3];
            }
            catch
            {
                Errors.Add("Empty date");
                Counters.ErrorOnCheck = true;
            }
            try
            {
                Raw.Time = fields[4];
            }
            catch
            {
                Errors.Add("Empty time");
                Counters.ErrorOnCheck = true;
            }
            try
            {
                bool t = DateTime.TryParseExact(Raw.Date.Trim() + " " + Raw.Time.Trim(), "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime);
            }
            catch
            {
                Errors.Add("Error on DateTime [" + Raw.Date.Trim() + " " + Raw.Time.Trim() + "]");
                Counters.ErrorOnCheck = true;
            }
            if (DateTime == DateTime.MinValue)
            {
                Errors.Add("Not specified DateTime [" + Raw.Date.Trim() + " " + Raw.Time.Trim() + "]");
                Counters.ErrorOnCheck = true;
            }

            // отправленные данные
            int field_position = 5;
            try
            {
                Raw.SendCall = fields[field_position];
            }
            catch
            {
                Errors.Add("Empty send call");
                Counters.ErrorOnCheck = true;
            }
            field_position++;
            try
            {
                Raw.SendRST = fields[field_position];
            }
            catch
            {
                Errors.Add("Empty send RST");
                Counters.ErrorOnCheck = true;
            }
            if (Config.control > 0)
            {
                field_position++;
                try
                {
                    Raw.SendExch1 = fields[field_position];
                }
                catch
                {
                    Errors.Add("Empty send Exch1");
                    Counters.ErrorOnCheck = true;
                }
            }
            if (Config.control > 1)
            {
                field_position++;
                try
                {
                    Raw.SendExch2 = fields[field_position];
                }
                catch
                {
                    Errors.Add("Empty send Exch2");
                    Counters.ErrorOnCheck = true;
                }
            }
            if (Config.control > 2)
            {
                field_position++;
                try
                {
                    Raw.SendExch3 = fields[field_position];
                }
                catch
                {
                    Errors.Add("Empty send Exch3");
                    Counters.ErrorOnCheck = true;
                }
            }

            // принятые данные
            field_position++;
            try
            {
                Raw.RecvCall = fields[field_position];
            }
            catch
            {
                Errors.Add("Empty recv call");
                Counters.ErrorOnCheck = true;
            }
            field_position++;
            try
            {
                Raw.RecvRST = fields[field_position];
            }
            catch
            {
                Errors.Add("Empty recv RST");
                Counters.ErrorOnCheck = true;
            }
            if (Config.control > 0)
            {
                field_position++;
                try
                {
                    Raw.RecvExch1 = fields[field_position];
                }
                catch
                {
                    Errors.Add("Empty recv Exch1");
                    Counters.ErrorOnCheck = true;
                }
            }
            if (Config.control > 1)
            {
                field_position++;
                try
                {
                    Raw.RecvExch2 = fields[field_position];
                }
                catch
                {
                    Errors.Add("Empty recv Exch2");
                    Counters.ErrorOnCheck = true;
                }
            }
            if (Config.control > 2)
            {
                field_position++;
                try
                {
                    Raw.RecvExch3 = fields[field_position];
                }
                catch
                {
                    Errors.Add("Empty recv Exch3");
                    Counters.ErrorOnCheck = true;
                }
            }
            field_position++;
            try
            {
                Raw.Mo2t = fields[field_position];
            }
            catch { }

            //if (Counters.ErrorOnCheck)
            //    Folder.Contest.FormatOK = false
        }
        public override string ToString()
        {
            string s = string.Format("QSO {0} : {1} {2} {3} {4} {5} {6} ", Raw.Number, Raw.Feq, Raw.Mode, Raw.Date, Raw.Time, Raw.SendCall, Raw.SendRST);

            if (Config.control > 0)
                s = string.Concat(s, string.Format("{0} ", Raw.SendExch1));

            if (Config.control > 1)
                s = string.Concat(s, string.Format("{0} ", Raw.SendExch2));

            if (Config.control > 2)
                s = string.Concat(s, string.Format("{0} ", Raw.SendExch3));

            s = string.Concat(s, string.Format("{0} {1} ", Raw.RecvCall, Raw.RecvRST));

            if (Config.control > 0)
                s = string.Concat(s, string.Format("{0} ", Raw.RecvExch1));

            if (Config.control > 1)
                s = string.Concat(s, string.Format("{0} ", Raw.RecvExch2));

            if (Config.control > 2)
                s = string.Concat(s, string.Format("{0} ", Raw.RecvExch3));

            s = string.Concat(s, string.Format("{0} ", Raw.Mo2t));

            return s;
        }
    }
}
