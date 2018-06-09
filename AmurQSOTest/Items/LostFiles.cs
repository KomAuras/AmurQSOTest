using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    public class LostFiles : List<LostFile>
    {
        public int Width;
        public void Add(string Call)
        {
            bool added = false;
            foreach (LostFile item in this)
            {
                if (item.Call == Call)
                {
                    if (Width < Call.Length)
                        Width = Call.Length;
                    item.Count++;
                    added = true;
                    break;
                }
            }
            if (added == false)
                Add(new LostFile(Call));
        }
    }

    public class LostFile
    {
        public string Call { get; set; }
        public int Count { get; set; }

        public LostFile(string call)
        {
            Call = call;
            Count = 1;
        }
    }
}
