using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest
{
    public static class Config
    {
        // периоды времени теста
        public static List<Period> ContestPeriod = new List<Period>();
        public static List<Band> Bands = new List<Band>();
        public static List<Folder> Folders = new List<Folder>();

        private static string[] args;
        private static string folder_check;
        private static string folder_ubn;
        private static string folder_report;
        private static int subtour_min;
        private static int offset_min;
        private static int repeat_call;
        private static int double_check;
        private static int rst_check;
        private static int band_changes;
        private static int control;

        public static bool Load()
        {
            if (args.Count() == 0) { return true; }
            if (File.Exists(args[0] + "\\contest.ini"))
            {
                StreamReader fs = new StreamReader(args[0] + "\\contest.ini");
                string s = "";
                while (s != null)
                {
                    s = fs.ReadLine();
                    try
                    {
                        if (s.TrimStart().Substring(0, 1) != ";")
                            ParseLine(s.Substring(0, s.IndexOf(';')).Trim());
                    }
                    catch { }
                }
            }
            return false;
        }

        public static void Dump()
        {
            foreach (Period DT in ContestPeriod)
            {
                Console.WriteLine(DT.BeginDateTime.ToString() + " -> " + DT.EndDateTime.ToString());
            }
            Console.WriteLine(folder_check);
            Console.WriteLine(folder_ubn);
            Console.WriteLine(folder_report);
            Console.WriteLine(subtour_min);
            Console.WriteLine(offset_min);
            Console.WriteLine(repeat_call);
            Console.WriteLine(double_check);
            Console.WriteLine(rst_check);
            Console.WriteLine(band_changes);
            Console.WriteLine(control);
        }


        private static void ParseLine(string s)
        {
            if (CheckLine(s, "period"))
            {
                string[] data = s.Substring(s.IndexOf('=') + 1).Split(',');
                if (data.Count() != 2)
                    throw new Exception("Ошибка в периоде!");
                ContestPeriod.Add(new Period(DateTime.ParseExact(data[0].TrimStart(), "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture), DateTime.ParseExact(data[1].TrimStart(), "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture)));
            }
            if (CheckLine(s, "folder_check"))
            {
                folder_check = s.Substring(s.IndexOf('=') + 1).Trim();
            }
            if (CheckLine(s, "folder_ubn"))
            {
                folder_ubn = s.Substring(s.IndexOf('=') + 1).Trim();
            }
            if (CheckLine(s, "folder_report"))
            {
                folder_report = s.Substring(s.IndexOf('=') + 1).Trim();
            }
            if (CheckLine(s, "subtour_min"))
            {
                subtour_min = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (CheckLine(s, "offset_min"))
            {
                offset_min = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (CheckLine(s, "repeat_call"))
            {
                repeat_call = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (CheckLine(s, "double_check"))
            {
                double_check = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (CheckLine(s, "rst_check"))
            {
                rst_check = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (CheckLine(s, "band_changes"))
            {
                band_changes = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (CheckLine(s, "control"))
            {
                control = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
        }

        private static bool CheckLine(string s, string param_name)
        {
            return s.ToUpper().TrimStart().Substring(0, param_name.Length) == param_name.ToUpper();
        }

        public static void SetArgs(string[] args)
        {
            Config.args = args;
        }
    }

    public class Folder
    {
        public string Name;
        public string Desc;
        public int Freq;
        public int Mode;
    }

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

    public class Band
    {
        public int Freq;
        public int Price;

        public Band(int freq, int price)
        {
            Freq = freq;
            Price = price;
        }
    }
}
