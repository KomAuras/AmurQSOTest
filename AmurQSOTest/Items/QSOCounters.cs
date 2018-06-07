namespace AmurQSOTest.Items
{
    /// <summary>
    /// счетчики QSO, внутренние
    /// </summary>
    public class QSOCounters
    {
        /// <summary>
        /// номер подтура
        /// </summary>
        public int SubTourNumber;
        /// <summary>
        /// связь отфильтрована по параметрам каталога
        /// </summary>
        public bool Filtered;
        /// <summary>
        /// установится в TRUE если были ошибки при предварительноя проверке
        /// </summary>
        public bool ErrorOnCheck;
        /// <summary>
        /// установится в TRUE если свчзь есть
        /// </summary>
        public bool OK;
        /// <summary>
        /// дистанция до корреспондента
        /// </summary>
        public double Distantion;
        /// <summary>
        /// очки по локатору
        /// </summary>
        public double ByLocator;
        /// <summary>
        /// всего очков
        /// </summary>
        public double Total;
    }
}