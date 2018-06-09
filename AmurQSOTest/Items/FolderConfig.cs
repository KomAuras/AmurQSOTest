using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmurQSOTest.Items
{
    /// <summary>
    /// настройки папки
    /// </summary>
    public class FolderConfig
    {
        /// <summary>
        /// название папки
        /// </summary>
        public string Name;
        /// <summary>
        /// описание папки
        /// </summary>
        public string Desc;
        /// <summary>
        /// разрешенные диапазоны
        /// </summary>
        public AllowBands AllowBands;
        /// <summary>
        /// разрешенные
        /// </summary>
        public AllowModes AllowModes;

        public FolderConfig(string name)
        {
            Name = name;
        }
    }
}
