using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class TransactionModel : ObservableObject
    {
        /// <summary>
        /// Привязываем транзакцию к клиенту
        /// </summary>
        public Client ClientLink { get => _clientLink; set { _clientLink = value; OnPropertyChanged(nameof(ClientLink)); } }
        private Client _clientLink;

        /// <summary>
        /// Название счета
        /// </summary>
        public string AccountName { get => _accountName; set { _accountName = value; OnPropertyChanged(nameof(AccountName)); } }
        private string _accountName;

        /// <summary>
        /// Уникальный идентификатор транзакции.
        /// </summary>
        public int Id { get => _id; set { _id = value; OnPropertyChanged(nameof(Id)); } }
        private int _id;

        /// <summary>
        /// Внешний ключ — идентификатор счёта.
        /// </summary>
        public int AccountId { get => _accountId; set { _accountId = value; OnPropertyChanged(nameof(AccountId)); } }
        private int _accountId;

        /// <summary>
        /// Тип транзакции: 'deposit', 'withdrawal', 'management_fee'.
        /// </summary>
        public TransactionType Type { get => _type; set { _type = value; OnPropertyChanged(nameof(Type)); } }
        private TransactionType _type;

        /// <summary>
        /// Сумма транзакции.
        /// </summary>
        public decimal Amount { get => _amount; set { _amount = value; OnPropertyChanged(nameof(Amount)); } }
        private decimal _amount;

        /// <summary>
        /// Статус транзакции (по умолчанию 'completed').
        /// </summary>
        public string Status { get => _status; set { _status = value; OnPropertyChanged(nameof(Status)); } }
        private string _status;

        /// <summary>
        /// Дата и время обработки транзакции.
        /// </summary>
        public DateTime? ProcessedAt { get => _processedAt; set { _processedAt = value; OnPropertyChanged(nameof(ProcessedAt)); } }
        private DateTime? _processedAt;

        /// <summary>
        /// Дата и время создания записи.
        /// </summary>
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(nameof(CreatedAt)); } }
        private DateTime _createdAt;

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get => _comment; set { _comment = value; OnPropertyChanged(nameof(Comment)); } }
        private string _comment;
    }
}
