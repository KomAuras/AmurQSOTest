namespace AmurQSOTest.Items
{
    public enum ErrorType
    {
        clear = 0,
        nolog,
        doublebytime,
        doublebymode,
        filteredband,
        filteredmode,
        datetimeerror,
        similarqso,
        correrror
    }
    /// <summary>
    /// счетчики QSO, внутренние
    /// </summary>
    public class QSOCounters
    {
        /// <summary>
        /// номер подтура
        /// </summary>
        public int SubTour;
        /// <summary>
        /// связь отфильтрована по параметрам каталога
        /// </summary>
        public bool Filtered;
        /// <summary>
        /// установится в TRUE если были ошибки
        /// </summary>
        public bool ErrorOnCheck;
        /// <summary>
        /// тип ошибки
        /// </summary>
        public ErrorType ErrorType;
        /// <summary>
        /// установится в TRUE если связь есть
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

        /// <summary>
        /// оффсет от какого нибудь времени
        /// для внутреннего использования.
        /// </summary>
        public int Offset;

        /// <summary>
        /// установить ошибку по QSO
        /// </summary>
        /// <param name="ErrorType"></param>
        public void SetError(ErrorType ErrorType)
        {
            if (ErrorType != ErrorType.clear)
                ErrorOnCheck = true;
            this.ErrorType = ErrorType;
        }

    }
}