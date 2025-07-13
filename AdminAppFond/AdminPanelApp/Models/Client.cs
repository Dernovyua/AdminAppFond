using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class Client : ObservableObject
    {

        /// <summary>
        /// Уникальный идентификатор клиента.
        /// </summary>
        public int Id { get => _id; set { _id = value; OnPropertyChanged(nameof(Id)); } }
        private int _id;

        /// <summary>
        /// Полное имя клиента.
        /// </summary>
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(nameof(FullName)); } }
        private string _fullName;

        /// <summary>
        /// Дата открытия клиента.
        /// </summary>
        public string OpenedAt { get => _openedAt; set { _openedAt = value; OnPropertyChanged(nameof(OpenedAt)); } }
        private string _openedAt;

        /// <summary>
        /// Текущий статус клиента (active, blocked, closed).
        /// </summary>
        public string Status { get => _status; set { _status = value; OnPropertyChanged(nameof(Status)); } }
        private string _status;

        /// <summary>
        /// Дополнительные заметки по клиенту.
        /// </summary>
        public string Notes { get => _notes; set { _notes = value; OnPropertyChanged(nameof(Notes)); } }
        private string _notes;

        /// <summary>
        /// Телефон клиента.
        /// </summary>
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(nameof(Phone)); } }
        private string _phone;

        /// <summary>
        /// Email клиента.
        /// </summary>
        public string Email { get => _email; set { _email = value; OnPropertyChanged(nameof(Email)); } }
        private string _email;

        /// <summary>
        /// Telegram-аккаунт клиента.
        /// </summary>
        public string Telegram { get => _telegram; set { _telegram = value; OnPropertyChanged(nameof(Telegram)); } }
        private string _telegram;

        /// <summary>
        /// Город клиента.
        /// </summary>
        public string City { get => _city; set { _city = value; OnPropertyChanged(nameof(City)); } }
        private string _city;

        /// <summary>
        /// Дата создания записи.
        /// </summary>
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(nameof(CreatedAt)); } }
        private DateTime _createdAt;

        /// <summary>
        /// Дата последнего обновления.
        /// </summary>
        public DateTime UpdatedAt { get => _updatedAt; set { _updatedAt = value; OnPropertyChanged(nameof(UpdatedAt)); } }
        private DateTime _updatedAt;

        /// <summary>
        /// Список аккаунтов, принадлежащих клиенту.
        /// </summary>
        public ObservableCollection<AccountModel> Accounts { get => _accounts; set { _accounts = value; OnPropertyChanged(nameof(Accounts)); } }
        private ObservableCollection<AccountModel> _accounts = new();
    }
}
