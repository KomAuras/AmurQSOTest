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
            try
            {
                return s.ToUpper().TrimStart().Substring(0, param_name.Length) == param_name.ToUpper();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// получить значение свойства объекта
        /// </summary>
        /// <param name="target"></param>
        /// <param name="property_name"></param>
        /// <returns></returns>
        public static object GetProperty(object target, string property_name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, property_name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        /// <summary>
        /// установить значение свойства объекта
        /// </summary>
        /// <param name="target"></param>
        /// <param name="property_name"></param>
        /// <param name="value"></param>
        public static void SetProperty(object target, string property_name, object value)
        {
            Type type = target.GetType();
            PropertyInfo prop = type.GetProperty(property_name);
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
