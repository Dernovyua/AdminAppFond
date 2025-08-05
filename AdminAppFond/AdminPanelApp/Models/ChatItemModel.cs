using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class ChatItemModel : ObservableObject
    {
        /// <summary>
        /// Текст последнего сообщения в чате.
        /// </summary>
        public string LastMessage
        {
            get => _lastMessage;
            set { _lastMessage = value; OnPropertyChanged(nameof(LastMessage)); }
        }
        private string _lastMessage;

        /// <summary>
        /// Время последнего сообщения (в строковом виде, например "15:32").
        /// </summary>
        public DateTime LastMessageTime
        {
            get => _lastMessageTime;
            set { _lastMessageTime = value; OnPropertyChanged(nameof(LastMessageTime)); }
        }
        private DateTime _lastMessageTime;

        /// <summary>
        /// Флаг, указывающий, есть ли непрочитанные сообщения.
        /// </summary>
        public bool IsUnread
        {
            get => _isUnread;
            set { _isUnread = value; OnPropertyChanged(nameof(IsUnread)); }
        }
        private bool _isUnread;

        /// <summary>
        /// Флаг закрепленного чата (если true — отображается выше обычных).
        /// </summary>
        public bool IsPinned
        {
            get => _isPinned;
            set { _isPinned = value; OnPropertyChanged(nameof(IsPinned)); }
        }
        private bool _isPinned;

        /// <summary>
        /// Статус клиента или дополнительная подпись (например: "онлайн", "в сети").
        /// </summary>
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }
        private string _status;

        /// <summary>
        /// Коллекция всех сообщений, относящихся к данному чату.
        /// </summary>
        public ObservableCollection<MessageItemModel> Messages
        {
            get => _messages;
            set { _messages = value; OnPropertyChanged(nameof(Messages)); }
        }
        private ObservableCollection<MessageItemModel> _messages = new();
    }
}
