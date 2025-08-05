using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class MessageItemModel : ObservableObject
    {
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }
        private string _text;

        /// <summary>
        /// Время отправки сообщения.
        /// </summary>
        public DateTime SentAt
        {
            get => _sentAt;
            set { _sentAt = value; OnPropertyChanged(nameof(SentAt)); }
        }
        private DateTime _sentAt;

        /// <summary>
        /// True, если сообщение отправлено администратором.
        /// </summary>
        public bool IsOwn
        {
            get => _isOwn;
            set { _isOwn = value; OnPropertyChanged(nameof(IsOwn)); }
        }
        private bool _isOwn;
    }
}
