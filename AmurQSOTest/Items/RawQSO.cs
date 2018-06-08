using System;
using System.Collections.Generic;
using System.Reflection;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// сырое QSO
    /// </summary>
    public class RawQSO
    {
        /// <summary>
        /// номер QSO
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// частота:
        /// 1800 или фактическая частота в KHz и т.д.
        /// </summary>
        public string Feq { get; set; }
        /// <summary>
        /// вид работы	
        /// AM (AMTOR)
        /// AX (PACKET AX25)
        /// CO (CONTESTIA)
        /// CW (CW)
        /// DO (DOMINO)
        /// FM (FM)
        /// HE (HELLSCHREIBER)
        /// MF (MFSK16)
        /// OL (OLIVIA)
        /// PH (SSB)
        /// PM (PSK63)
        /// PO (PSK125)
        /// PS (PSK31)
        /// PT (PACTOR)
        /// RM (RTTYM)
        /// RY (RTTY)
        /// TH (THROB)
        /// TV (SSTV)
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// дата UTC в формате ГГГГ-ММ-ДД
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// время UTC в формате ччмм
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// отправленный позывной
        /// </summary>
        public string SendCall { get; set; }
        /// <summary>
        /// отправленный RST (RS, RSQ, RSV)
        /// </summary>
        public string SendRST { get; set; }
        /// <summary>
        /// отправленный контрольный номер 1
        /// </summary>
        public string SendExch1 { get; set; }
        /// <summary>
        /// отправленный контрольный номер 2
        /// </summary>
        public string SendExch2 { get; set; }
        /// <summary>
        /// отправленный контрольный номер 3
        /// </summary>
        public string SendExch3 { get; set; }
        /// <summary>
        /// принятый позывной
        /// </summary>
        public string RecvCall { get; set; }
        /// <summary>
        /// принятый RST (RS, RSQ, RSV)
        /// </summary>
        public string RecvRST { get; set; }
        /// <summary>
        /// принятый контрольный номер 1
        /// </summary>
        public string RecvExch1 { get; set; }
        /// <summary>
        /// принятый контрольный номер 2
        /// </summary>
        public string RecvExch2 { get; set; }
        /// <summary>
        /// принятый контрольный номер 3
        /// </summary>
        public string RecvExch3 { get; set; }
        /// <summary>
        /// Номер передатчика: 0, 1
        /// </summary>
        public string Mo2t { get; set; }

        public RawQSO()
        {
            Number = 0;
            Date = "";
            Feq = "";
            Mo2t = "";
            Mode = "";
            RecvCall = "";
            RecvExch1 = "";
            RecvExch2 = "";
            RecvExch3 = "";
            RecvRST = "";
            SendCall = "";
            SendExch1 = "";
            SendExch2 = "";
            SendExch3 = "";
            SendRST = "";
            Time = "";
        }

        public int[] GetWidths()
        {
            List<int> widths = new List<int>();
            Type type = this.GetType();
            foreach (PropertyInfo p in type.GetProperties())
            {
                widths.Add(Util.GetProperty(this,p.Name).ToString().Length);
            }
            return widths.ToArray();
        }
    }
}