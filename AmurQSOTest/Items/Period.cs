using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// период даты/времени. начало и конец
    /// </summary>
    public class Period
    {
        /// <summary>
        /// начало периода 
        /// </summary>
        public DateTime BeginDateTime;
        /// <summary>
        /// окончание периода
        /// </summary>
        public DateTime EndDateTime;

        public Period()
        {
        }

        public Period(DateTime beginDateTime, DateTime endDateTime)
        {
            BeginDateTime = beginDateTime;
            EndDateTime = endDateTime;
        }

        /// <summary>
        /// проверка вхождения переданного периода в границы периода
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Inside(Period p)
        {
            if (p.BeginDateTime >= BeginDateTime && p.EndDateTime < EndDateTime)
                return true;
            return false;
        }

        /// <summary>
        /// проверка вхождения переданной DateTime в границы периода. может быть равна окончанию
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool InsideWithEnd(DateTime dt)
        {
            if (dt >= BeginDateTime && dt <= EndDateTime)
                return true;
            return false;
        }

        /// <summary>
        /// проверка вхождения переданной DateTime в границы периода. должна быть меньше окончания
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Inside(DateTime dt)
        {
            if (dt >= BeginDateTime && dt < EndDateTime)
                return true;
            return false;
        }
    }
}
