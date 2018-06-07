using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    public class Period
    {
        public DateTime BeginDateTime;
        public DateTime EndDateTime;

        public Period(DateTime beginDateTime, DateTime endDateTime)
        {
            BeginDateTime = beginDateTime;
            EndDateTime = endDateTime;
        }
    }
}
