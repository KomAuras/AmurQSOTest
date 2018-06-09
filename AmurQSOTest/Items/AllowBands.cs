using System.Collections.Generic;

namespace AmurQSOTest.Items
{
    public class AllowBands : List<int>
    {
        public bool Check(int band)
        {
            foreach (int b in this)
            {
                if (b == band)
                    return true;
            }
            return false;
        }
    }
}