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
        public int Number;
        /// <summary>
        /// диапазон
        /// </summary>
        public string Feq;
        /// <summary>
        /// мода
        /// </summary>
        public string Mode;
        /// <summary>
        /// дата
        /// </summary>
        public string Date;
        /// <summary>
        /// время
        /// </summary>
        public string Time;
        /// <summary>
        /// отправленный позывной
        /// </summary>
        public string SendCall;
        /// <summary>
        /// отправленный RST
        /// </summary>
        public string SendRST;
        /// <summary>
        /// отправленный контрольный номер 1
        /// </summary>
        public string SendExch1;
        /// <summary>
        /// отправленный контрольный номер 2
        /// </summary>
        public string SendExch2;
        /// <summary>
        /// отправленный контрольный номер 3
        /// </summary>
        public string SendExch3;
        /// <summary>
        /// полученный позывной
        /// </summary>
        public string RecvCall;
        /// <summary>
        /// полученный RST
        /// </summary>
        public string RecvRST;
        /// <summary>
        /// полученный контрольный номер 1
        /// </summary>
        public string RecvExch1;
        /// <summary>
        /// полученный контрольный номер 2
        /// </summary>
        public string RecvExch2;
        /// <summary>
        /// полученный контрольный номер 3
        /// </summary>
        public string RecvExch3;
    }
}