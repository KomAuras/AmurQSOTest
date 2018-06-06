using System;
using System.IO;

namespace AmurQSOTest
{
    public static class Util
    {
        private static Random gen = new Random();

        public static DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        public static string AppPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class FieldFormat : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class FieldSort : System.Attribute
    {
    }
}
