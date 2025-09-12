using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models.Scenario
{
    public class TgMenuModel : ObservableObject
    {
        private int _id;
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int Id { get => _id; set { _id = value; OnPropertyChanged(nameof(Id)); } }

        private bool _isRun;
        /// <summary>
        /// Запущен или нет
        /// </summary>
        public bool IsRun { get => _isRun; set { _isRun = value; OnPropertyChanged(nameof(IsRun)); } }

        private string _level;
        /// <summary>
        /// Уровень вложенности меню
        /// </summary>
        public string Level { get => _level; set { _level = value; OnPropertyChanged(nameof(Level)); } }

        private string _name;
        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

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
