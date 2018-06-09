using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// класс подтуров
    /// </summary>
    public class SubTours : List<SubTour>
    {
        /// <summary>
        /// возвращает номер тура если переданный период попадает в него. 0 если не попадает ни в один подтур
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public int Inside(Period p)
        {
            foreach (SubTour s in this)
            {
                if (s.Period.Inside(p))
                    return s.Number;
            }
            return 0;
        }

        /// <summary>
        /// возвращает номер тура если переданная DateTime попадает в него. 0 если не попадает ни в один подтур
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int Inside(DateTime dt)
        {
            foreach (SubTour s in this)
            {
                if (s.Period.Inside(dt) || (s.End && s.Period.InsideWithEnd(dt)))
                    return s.Number;
            }
            return 0;
        }
    }
}
