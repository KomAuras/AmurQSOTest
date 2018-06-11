using System.Collections.Generic;

namespace AmurQSOTest.Items
{
    public class AllowModes : List<string>
    {
        public bool Check(string band)
        {
            foreach (string b in this)
            {
                if (b == band.ToUpper())
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Join(", ", this);
        }
    }
}