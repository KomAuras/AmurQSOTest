using System;
using System.Collections.Generic;
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
        /// сырые данные по QSO
        /// </summary>
        public RawQSO Raw;
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
        public QSOCounters Counters;
        /// <summary>
        /// ссылка на QSO корреспондента
        /// </summary>        
        public QSO RecvQSO;
    }
}
