using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    public class BandPoints : List<BandPoint>
    {
        public double GetPoints(int band)
        {
            foreach (BandPoint point in this)
            {
                if (point.Band == band)
                    return point.Point;
            }
            throw new Exception("Не найдены очки для диапазона " + band);
        }
    }

    public class BandPoint
    {
        public int Band;
        public double Point;

        public BandPoint(int band, double point)
        {
            Band = band;
            Point = point;
        }
    }
}
