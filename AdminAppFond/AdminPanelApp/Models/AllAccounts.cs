using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    /// <summary>
    /// Получаем все данные по счетам
    /// </summary>
    public class AllAccounts
    {
        public string Account { get; set; }
        public string Exchange { get; set; }
        public string Name { get; set; }
        public string AccString { get; set; }
    }
}
