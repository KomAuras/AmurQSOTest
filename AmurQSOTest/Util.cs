using System;
using System.IO;
using System.Reflection;

namespace AmurQSOTest
{
    public static class Util
    {
        private static Random gen = new Random();

        /// <summary>
        /// генерация случайной даты и времени
        /// </summary>
        /// <returns></returns>
        public static DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        /// <summary>
        /// папка с программой
        /// </summary>
        /// <returns></returns>
        public static string AppPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// проверка совпадения НАЧАЛА строки с паттерном
        /// </summary>
        /// <param name="s"></param>
        /// <param name="param_name"></param>
        /// <returns></returns>
        public static bool StrExists(string s, string param_name)
        {
            s = s.ToUpper().TrimStart();
            if (param_name.Length > s.Length) {
                return false;
            }
            return s.Substring(0, param_name.Length) == param_name.ToUpper();
        }

        /// <summary>
        /// получить значение свойства объекта
        /// </summary>
        /// <param name="target"></param>
        /// <param name="property_name"></param>
        /// <returns></returns>
        public static object GetProperty(object target, string property_name)
        {
            return target.GetType().GetProperty(property_name).GetValue(target, null);
        }

        /// <summary>
        /// установить значение свойства объекта
        /// </summary>
        /// <param name="target"></param>
        /// <param name="property_name"></param>
        /// <param name="value"></param>
        public static void SetProperty(object target, string property_name, object value)
        {
            //Type type = target.GetType();
            PropertyInfo prop = (target.GetType()).GetProperty(property_name);
            prop.SetValue(target, value, null);
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
