using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AmurQSOTest
{
    /// <summary>
    /// класс расчета для QTH локаторов
    /// </summary>
    public static class Coordinate
    {
        public static void Test()
        {
            Console.WriteLine(GetDistance("PN78MO", "PO80MN"));
        }

        /// <summary>
        /// проверить локатор на валидность
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>true если корректный локатор</returns>
        public static bool ValidateLocator(string locator)
        {
            return Regex.IsMatch(ExpandLocator(locator), "^[A-R]{2}[0-9]{2}[A-X]{2}[0-9]{2}[A-X]{2}$");
        }

        /// <summary>
        /// расширить локаторы до 10 символов
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        private static string ExpandLocator(string locator)
        {
            if (locator.Length == 6)
                return locator + "55AA";
            if (locator.Length == 8)
                return locator + "LL";
            return locator;
        }

        /// <summary>
        /// получить дистанцию в километрах между локаторами
        /// </summary>
        /// <param name="locator1">локатор 1</param>
        /// <param name="locator2">локатор 2</param>
        /// <returns></returns>
        public static double GetDistance(string locator1, string locator2)
        {
            if (!ValidateLocator(locator1))
                return -1;
            if (!ValidateLocator(locator2))
                return -1;
            LatLng geo1 = Locator2Degress(locator1);
            LatLng geo2 = Locator2Degress(locator2);
            LatLng geo11 = new LatLng(geo1.Latitude * (Math.PI / 180), geo1.Longitude * (Math.PI / 180));
            LatLng geo22 = new LatLng(geo2.Latitude * (Math.PI / 180), geo2.Longitude * (Math.PI / 180));
            return GetDistance(geo11, geo22);
        }

        /// <summary>
        /// получить дистанцию в километрах между коррдинатами
        /// </summary>
        /// <param name="Latitude1"></param>
        /// <param name="Longitude1"></param>
        /// <param name="Latitude2"></param>
        /// <param name="Longitude2"></param>
        /// <returns></returns>
        public static double GetDistance(double Latitude1, double Longitude1, double Latitude2, double Longitude2)
        {
            return GetDistance(new LatLng(Latitude1 * (Math.PI / 180), Longitude1 * (Math.PI / 180)), new LatLng(Latitude2 * (Math.PI / 180), Longitude2 * (Math.PI / 180)));
        }

        /// <summary>
        /// получить дистанцию в километрах между коррдинатами
        /// </summary>
        /// <param name="geo1"></param>
        /// <param name="geo2"></param>
        /// <returns></returns>
        public static double GetDistance(LatLng geo1, LatLng geo2)
        {
            double d;
            double gc_d;
            d = Math.Acos(Math.Sin(geo1.Latitude) * Math.Sin(geo2.Latitude) + Math.Cos(geo1.Latitude) * Math.Cos(geo2.Latitude) * Math.Cos((-geo1.Longitude) - (-geo2.Longitude)));
            gc_d = Math.Round(((180.0 / Math.PI) * d) * 60 * 10) / 10;
            return Math.Round(1.852 * gc_d * 10 / 10, 3);
        }

        /// <summary>
        /// локаторы в градусы
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static LatLng Locator2Degress(string locator)
        {
            LatLng geo = new LatLng();
            if (Regex.IsMatch(locator, "^[A-R]{2}[0-9]{2}[A-X]{2}$"))
            {
                geo.Longitude = (locator[0] - 'A') * 20 + (locator[2] - '0') * 2 + (locator[4] - 'A' + 0.5) / 12 - 180;
                geo.Latitude = (locator[1] - 'A') * 10 + (locator[3] - '0') + (locator[5] - 'A' + 0.5) / 24 - 90;
                return geo;
            }
            else if (Regex.IsMatch(locator, "^[A-R]{2}[0-9]{2}[A-X]{2}[0-9]{2}$"))
            {
                geo.Longitude = (locator[0] - 'A') * 20 + (locator[2] - '0') * 2 + (locator[4] - 'A' + 0.0) / 12 + (locator[6] - '0' + 0.5) / 120 - 180;
                geo.Latitude = (locator[1] - 'A') * 10 + (locator[3] - '0') + (locator[5] - 'A' + 0.0) / 24 + (locator[7] - '0' + 0.5) / 240 - 90;
                return geo;
            }
            else if (Regex.IsMatch(locator, "^[A-R]{2}[0-9]{2}[A-X]{2}[0-9]{2}[A-X]{2}$"))
            {
                geo.Longitude = (locator[0] - 'A') * 20 + (locator[2] - '0') * 2 + (locator[4] - 'A' + 0.0) / 12 + (locator[6] - '0' + 0.0) / 120 + (locator[8] - 'A' + 0.5) / 120 / 24 - 180;
                geo.Latitude = (locator[1] - 'A') * 10 + (locator[3] - '0') + (locator[5] - 'A' + 0.0) / 24 + (locator[7] - '0' + 0.0) / 240 + (locator[9] - 'A' + 0.5) / 240 / 24 - 90;
                return geo;
            }
            else
            {
                throw new FormatException("Invalid locator format");
            }
        }

        /// <summary>
        /// координаты
        /// </summary>
        public class LatLng
        {
            /// <summary>
            /// широта
            /// </summary>
            public double Latitude;
            /// <summary>
            /// долгота
            /// </summary>
            public double Longitude;

            /// <summary>
            /// пустой экземпляр
            /// </summary>
            public LatLng()
            {
            }

            /// <summary>
            /// экземпляр с координатами
            /// </summary>
            /// <param name="latitude"></param>
            /// <param name="longitude"></param>
            public LatLng(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }
    }
}
