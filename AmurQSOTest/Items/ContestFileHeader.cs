using System.Collections.Generic;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// заголовок файла с данными
    /// </summary>
    public class ContestFileHeader
    {
        /// <summary>
        /// Содержат почтовый адрес. Допускается не более одной строки NAME и не более трех строк ADDRESS.При использовании кириллицы допустимые кодировки: Windows-1251, CP-866, KOI-8R
        /// </summary>
        public List<string> address = new List<string>();
        /// <summary>
        /// Содержит позывной, который использовался в соревнованиях.
        /// </summary>
        public string callsign;
        /// <summary>
        /// Содержит указание на использование помощи. Допустимые значения:
        /// ASSISTED, NON-ASSISTED
        /// </summary>
        public string category_assisted;
        /// <summary>
        /// Содержит указание на диапазоны, использованные в соревновании.
        /// Допустимые значения: ALL, 160M, 80M, 40M, 20M, 15M, 10M, 6M, 2M, 222, 432, 902, 1.2G
        /// </summary>
        public string category_band;
        /// <summary>
        /// Содержит указание на вид работы. Допустимые значения:
        /// SSB(телефон), CW(телеграф), DIGI(телетайп, цифровые виды), MIXED(несколько из вышеперечисленных)
        /// </summary>
        public string category_mode;
        /// <summary>
        /// Содержит указание на число операторов и тип отчета. Допустимые значения:
        /// SINGLE-OP(один оператор), MULTI-OP(несколько операторов), MULTI-OP-2  (два оператора), CHECKLOG(отчет для контроля)
        /// </summary>
        public string category_operator;
        /// <summary>
        /// Содержит указание на подкатегорию. Допустимые значения:
        /// YL          (женщины)
        /// JR          (молодежь)
        /// JR-13       (возраст до 13 лет)
        /// JR-15       (возраст до 15 лет)
        /// JR-18       (возраст до 18 лет)
        /// JR-19       (возраст до 19 лет)
        /// VETERAN     (ветераны)
        /// SPECIAL     (специальные станции; операторы с ограниченными физическими возможностями)
        /// OVER-50     (радиолюбительский стаж 50 и более лет)
        /// OVER-100    (сумма возраста и стажа 100 и более лет)
        /// POLAR       (станция расположена за полярным кругом)
        /// </summary>
        public string category_overlay;
        /// <summary>
        /// Содержит указание на мощность. Допустимые значения:
        /// HIGH
        /// LOW
        /// QRP
        /// Границы между HIGH, LOW и QRP определяются положением соревнования.
        /// </summary>
        public string category_power;
        /// <summary>
        /// Содержит указание на тип станции. Допустимые значения:
        /// FIXED 
        /// MOBILE 
        /// PORTABLE 
        /// ROVER 
        /// EXPEDITION 
        /// HQ 
        /// SCHOOL 
        /// </summary>
        public string category_station;
        /// <summary>
        /// Содержит указание на зачетное время. Допустимые значения:
        /// 6-HOURS 
        /// 12-HOURS 
        /// 24-HOURS 
        /// </summary>
        public string category_time;
        /// <summary>
        /// Содержит указание на количество передатчиков. Допустимые значения:
        /// ONE 
        /// TWO 
        /// LIMITED 
        /// UNLIMITED 
        /// SWL 
        /// </summary>
        public string category_transmitter;
        /// <summary>
        /// Содержит заявляемый результат (очки).
        /// </summary>
        public string claimed_score;
        /// <summary>
        /// Содержит название клуба. 
        /// </summary>
        public string club;
        /// <summary>
        /// Содержит обозначение соревнования.
        /// </summary>
        public string contest;
        /// <summary>
        /// Содержит название и версию программы, с помощью которой был создан этот файл отчета.
        /// </summary>
        public string created_by;
        /// <summary>
        /// Содержит адрес электронной почты участника.
        /// </summary>
        public string email;
        /// <summary>
        /// Содержит указание на географическое положение участника или на
        /// принадлежность к клубу для правильного определения категории участия.
        /// В зависимости от соревнования это может быть:
        /// - номер по списку диплома RDA (для российских станций)
        /// - название клуба, членский номер клуба и др., согласно положению соревнования
        /// - ARRL/RAC section (все соревнования ARRL) 
        /// - название острова (RSGB-IOTA) 
        /// - Штат/провинция (CQ-VHF) 
        /// - ARRL/RAC section (все остальные соревнования CQ)
        /// </summary>
        public string location;
        /// <summary>
        /// Содержат почтовый адрес. Допускается не более одной строки NAME и не более трех строк ADDRESS.При использовании кириллицы допустимые кодировки: Windows-1251, CP-866, KOI-8R
        /// </summary>
        public string name;
        /// <summary>
        /// Содержит указание на длительность перерыва в работе.
        /// </summary>
        public string offtime;
        /// <summary>
        /// Содержит данные об операторе (операторах) станции. Допускается более одной строки OPERATORS.
        /// </summary>
        public List<string> operators = new List<string>();
        /// <summary>
        /// Содержит замечания, отзывы и т.д. в произвольном формате. Допускается более одной строки SOAPBOX.
        /// </summary>
        public List<string> soapbox = new List<string>();
    }
}