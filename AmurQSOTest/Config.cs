using AmurQSOTest.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AmurQSOTest
{
    public static class Config
    {
        /// <summary>
        /// заголовок программы
        /// </summary>
        public const string ProgrammTitle = "AmurQSOTest ver 1.0";
        /// <summary>
        /// периоды времени теста
        /// </summary>        
        public static List<Period> ContestPeriod = new List<Period>();
        /// <summary>
        /// подтуры в соревновании
        /// </summary>
        public static SubTours SubTours = new SubTours();
        /// <summary>
        /// диапазоны теста с очками за них
        /// </summary>
        public static BandPoints ContestBandPoints = new BandPoints();
        /// <summary>
        /// папки с файлами
        /// </summary>
        public static List<FolderConfig> ContestFolders = new List<FolderConfig>();

        public static string folder_check;
        public static string folder_ubn;
        public static string folder_report;
        public static int subtour_min;
        public static int offset_min;
        public static int repeat_call;
        public static int double_check;
        public static int rst_check;
        public static int band_changes;
        public static int control;
        public static int control_loc;

        private static string[] args;
        private static FolderConfig temp_folder;

        /// <summary>
        /// папка с соревнованием
        /// </summary>
        public static string ContestPath { get; private set; }

        /// <summary>
        /// папка с программой
        /// </summary>
        public static string AppFolder() { return Environment.CurrentDirectory.TrimEnd('\\') + @"\"; ; }

        /// <summary>
        /// загрузка конфигурации теста из contest.ini
        /// </summary>
        public static void Load()
        {
            if (File.Exists(AppFolder() + ContestPath + "\\contest.ini"))
            {
                StreamReader fs = new StreamReader(AppFolder() + ContestPath + "\\contest.ini", Encoding.GetEncoding("windows-1251"));
                string s = "";
                while (s != null)
                {
                    s = fs.ReadLine();
                    if (s != null && s.Length > 0)
                    {
                        if (s.TrimStart().Substring(0, 1) != ";")
                            ParseLine(s.Substring(0, s.IndexOf(';')).Trim());
                    }
                }
                AddFolderIfPrepared();
                BuildTourList();
            }
        }

        /// <summary>
        /// формирование списка подтуров
        /// </summary>
        private static void BuildTourList()
        {
            int subtour_number = 0;
            int offset = 0;
            DateTime dt = DateTime.MinValue;
            foreach (Period p in ContestPeriod)
            {
                offset = 0;
                do
                {
                    SubTour subtour = new SubTour();
                    subtour_number++;
                    subtour.Number = subtour_number;
                    subtour.Period.BeginDateTime = p.BeginDateTime.AddMinutes(offset);
                    offset += Config.subtour_min;
                    subtour.Period.EndDateTime = p.BeginDateTime.AddMinutes(offset);
                    dt = subtour.Period.EndDateTime;
                    if (dt <= p.EndDateTime)
                    {
                        if (dt == p.EndDateTime)
                            subtour.End = true;
                        SubTours.Add(subtour);
                    }
                } while (dt < p.EndDateTime);
            }
        }

        /// <summary>
        /// разбор одной строки из настроек
        /// </summary>
        /// <param name="s"></param>
        private static void ParseLine(string s)
        {
            // списки
            if (Util.StrExists(s, "period"))
            {
                string[] data = s.Substring(s.IndexOf('=') + 1).Split(',');
                if (data.Count() != 2)
                    throw new Exception("Ошибка в периоде!");
                ContestPeriod.Add(new Period(DateTime.ParseExact(data[0].TrimStart(), "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture), DateTime.ParseExact(data[1].TrimStart(), "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture)));
            }
            if (Util.StrExists(s, "band"))
            {
                string[] data = s.Substring(s.IndexOf('=') + 1).Split(',');
                if (data.Count() != 2)
                    throw new Exception("Ошибка в очках за диапазон!");
                int band = Int32.Parse(data[0]);
                if (!Standards.Bands.Check(band))
                    throw new Exception("Ошибка в частоте " + data[0]);
                ContestBandPoints.Add(new BandPoint(band, Double.Parse(data[1], CultureInfo.InvariantCulture)));
            }

            // папки 
            if (Util.StrExists(s, "name"))
            {
                AddFolderIfPrepared();
                temp_folder = new FolderConfig(s.Substring(s.IndexOf('=') + 1).Trim());
            }
            else if (Util.StrExists(s, "desc"))
            {
                if (temp_folder != null)
                    temp_folder.Desc = s.Substring(s.IndexOf('=') + 1).Trim();
            }
            else if (Util.StrExists(s, "freq"))
            {
                if (temp_folder != null)
                {
                    temp_folder.AllowBands = new AllowBands();
                    foreach (string str in s.Substring(s.IndexOf('=') + 1).Split(','))
                    {
                        int band = Int32.Parse(str);
                        if (!Standards.Bands.Check(band))
                            throw new Exception("Ошибка в частоте " + str);
                        temp_folder.AllowBands.Add(band);
                    }
                }
            }
            else if (Util.StrExists(s, "mode"))
            {
                if (temp_folder != null)
                {
                    temp_folder.AllowModes = new AllowModes();
                    foreach (string str in s.Substring(s.IndexOf('=') + 1).Split(',').Select(p => p.Trim()).ToList())
                    {
                        if (Standards.Modes.Check(str))
                            throw new Exception("Ошибка в режиме работы " + str);
                        temp_folder.AllowModes.Add(str);
                    }
                }
            }
            else
            {
                AddFolderIfPrepared();
            }

            // штучные параметры
            if (Util.StrExists(s, "folder_check"))
            {
                folder_check = s.Substring(s.IndexOf('=') + 1).Trim();
            }
            if (Util.StrExists(s, "folder_ubn"))
            {
                folder_ubn = s.Substring(s.IndexOf('=') + 1).Trim();
            }
            if (Util.StrExists(s, "folder_report"))
            {
                folder_report = s.Substring(s.IndexOf('=') + 1).Trim();
            }
            if (Util.StrExists(s, "subtour_min"))
            {
                subtour_min = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (Util.StrExists(s, "offset_min"))
            {
                offset_min = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (Util.StrExists(s, "repeat_call"))
            {
                repeat_call = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (Util.StrExists(s, "double_check"))
            {
                double_check = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (Util.StrExists(s, "rst_check"))
            {
                rst_check = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (Util.StrExists(s, "band_changes"))
            {
                band_changes = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (Util.StrExists(s, "control"))
            {
                control = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
            if (Util.StrExists(s, "control:loc"))
            {
                control_loc = Int32.Parse(s.Substring(s.IndexOf('=') + 1));
            }
        }

        /// <summary>
        /// если был создан каталог - добавляем его
        /// </summary>
        private static void AddFolderIfPrepared()
        {
            if (temp_folder != null)
            {
                ContestFolders.Add(temp_folder);
                temp_folder = null;
            }
        }

        /// <summary>
        /// сохрание параметров запуска
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool SetArgs(string[] args)
        {
            Config.args = args;
            if (args.Count() == 0)
            {
                Console.WriteLine("Не задана папка с соревнованием!");
                return true;
            }
            Config.ContestPath = Config.args[0];
            return false;
        }

        /// <summary>
        /// вывод дампа настроек
        /// </summary>
        public static void Dump()
        {
            foreach (Period DT in ContestPeriod)
            {
                Console.WriteLine(DT.BeginDateTime.ToString() + " -> " + DT.EndDateTime.ToString());
            }
            foreach (SubTour st in SubTours)
            {
                Console.WriteLine(st.Number + " -> " + st.Period.BeginDateTime.ToString() + " - " + st.Period.EndDateTime.ToString());
            }
            foreach (BandPoint BP in ContestBandPoints)
            {
                Console.WriteLine(BP.Band.ToString() + " -> " + BP.Point.ToString());
            }
            foreach (FolderConfig F in ContestFolders)
            {
                Console.WriteLine(F.Name.ToString() + " -> " + F.Desc.ToString());
                foreach (int freq in F.AllowBands)
                {
                    Console.WriteLine("Freq: " + freq.ToString());
                }
                foreach (string mode in F.AllowModes)
                {
                    Console.WriteLine("Mode: " + mode);
                }
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
    }
}
