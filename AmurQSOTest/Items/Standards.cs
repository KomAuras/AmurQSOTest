using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    public static class Standards
    {
        public static Bands Bands = new Bands();
        public static Modes Modes = new Modes();

        public static void Build()
        {
            Bands.Add(new OneBand(144, 144, 146));
            Bands.Add(new OneBand(144, 144000, 146000));
            Bands.Add(new OneBand(430, 430, 440));
            Bands.Add(new OneBand(430, 430000, 440000));
            Bands.Add(new OneBand(1260, 1260, 1300));
            Bands.Add(new OneBand(1260, 1260000, 1300000));
            Modes.Add(new OneMode("AM"));
            Modes.Add(new OneMode("AX"));
            Modes.Add(new OneMode("CO"));
            Modes.Add(new OneMode("CW"));
            Modes.Add(new OneMode("DO"));
            Modes.Add(new OneMode("FM"));
            Modes.Add(new OneMode("HE"));
            Modes.Add(new OneMode("MF"));
            Modes.Add(new OneMode("OL"));
            Modes.Add(new OneMode("PH"));
            Modes.Add(new OneMode("PM"));
            Modes.Add(new OneMode("PO"));
            Modes.Add(new OneMode("PS"));
            Modes.Add(new OneMode("PT"));
            Modes.Add(new OneMode("RM"));
            Modes.Add(new OneMode("RY"));
            Modes.Add(new OneMode("TH"));
            Modes.Add(new OneMode("TV"));
        }
    }

    public class OneBand
    {
        public int Band;
        public int FromFreq;
        public int ToFreq;

        public OneBand(int band, int fromFreq, int toFreq)
        {
            Band = band;
            FromFreq = fromFreq;
            ToFreq = toFreq;
        }
    }

    public class Bands : List<OneBand>
    {
        public bool Check(int Freq, out int ResultFeq)
        {
            foreach (OneBand b in this)
            {
                if (Freq >= b.FromFreq && Freq <= b.ToFreq)
                {
                    ResultFeq = b.Band;
                    return false;
                }
            }
            ResultFeq = 0;
            return true;
        }
    }

    public class OneMode
    {
        public string Mode;

        public OneMode(string Mode)
        {
            this.Mode = Mode;
        }
    }

    public class Modes : List<OneMode>
    {
        public bool Check(string Mode)
        {
            return this.FindIndex(x => x.Mode.ToUpper() == Mode.Trim().ToUpper()) > 0 ? false : true;
        }
    }
}
