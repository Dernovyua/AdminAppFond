using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class SettingCrmModel
    {
        /// <summary>
        /// Токен Телеграмм для настроек
        /// </summary>
        public string TgTokenCrm { get; set; }

        /// <summary>
        /// Сообщение отправляемое при нажатии меню в бухгалтерии
        /// </summary>
        public string MessageHandBuh { get; set; }

    }
}
