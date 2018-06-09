using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// список связей
    /// </summary>
    public class QSOList : List<QSO>
    {
        /// <summary>
        /// вернем QSO с предыдущей связью и количество минут прошедших с предыдущей связи
        /// </summary>
        /// <param name="item"></param>
        /// <param name="found_qso"></param>
        /// <returns></returns>
        public int GetOffsetMinutes(QSO item, out QSO found_qso)
        {
            if (this.Count() > 0)
                for (int i = this.Count() - 1; i >= 0; i--)
                {
                    if (item.Raw.RecvCall == this[i].Raw.RecvCall &&
                        item.Feq == this[i].Feq)
                    {
                        found_qso = this[i];
                        return (int)(item.DateTime - this[i].DateTime).TotalMinutes;
                    }
                }
            found_qso = null;
            return 0;
        }

        /// <summary>
        /// вернем QSO с предыдущей связью
        /// </summary>
        /// <param name="item"></param>
        /// <param name="found_qso"></param>
        /// <returns></returns>
        public bool GetPrevious(QSO item, out QSO found_qso)
        {
            if (this.Count() > 0)
                for (int i = this.Count() - 1; i >= 0; i--)
                {
                    if (item.Raw.RecvCall == this[i].Raw.RecvCall &&
                        item.Feq == this[i].Feq)
                    {
                        found_qso = this[i];
                        return true;
                    }
                }
            found_qso = null;
            return false;
        }
    }
}


