using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models.Scenario
{
    public class TgMenuModel : ObservableObject, IScenarioModel
    {
        private bool _isRun;
        /// <summary>
        /// Название.
        /// </summary>
        public bool IsRun { get => _isRun; set { _isRun = value; OnPropertyChanged(nameof(IsRun)); } }

        private string _name;
        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        private string _type;
        /// <summary>
        /// Тип.
        /// </summary>
        public string Type { get => _type; set { _type = value; OnPropertyChanged(nameof(Type)); } }

        private string _nameMenu;
        /// <summary>
        /// Название меню.
        /// </summary>
        public string NameMenu { get => _nameMenu; set { _nameMenu = value; OnPropertyChanged(nameof(NameMenu)); } }

        private string _icon;
        /// <summary>
        /// Иконка.
        /// </summary>
        public string Icon { get => _icon; set { _icon = value; OnPropertyChanged(nameof(Icon)); } }

        private int _column;
        /// <summary>
        /// Номер колонки.
        /// </summary>
        public int Column { get => _column; set { _column = value; OnPropertyChanged(nameof(Column)); } }

        private int _row;
        /// <summary>
        /// Номер строки.
        /// </summary>
        public int Row { get => _row; set { _row = value; OnPropertyChanged(nameof(Row)); } }

        private string _text;
        /// <summary>
        /// Текст.
        /// </summary>
        public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }

        private string _pathToDocument;
        /// <summary>
        /// Путь к документу.
        /// </summary>
        public string PathToDocument { get => _pathToDocument; set { _pathToDocument = value; OnPropertyChanged(nameof(PathToDocument)); } }

    }
}
