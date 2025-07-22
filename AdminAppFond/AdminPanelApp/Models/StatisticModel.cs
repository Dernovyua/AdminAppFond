using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class StatisticModel : ObservableObject
    {
        /// <summary>
        /// Уникальный идентификатор записи статистики.
        /// </summary>
        public int Id { get => _id; set { _id = value; OnPropertyChanged(nameof(Id)); } }
        private int _id;

        /// <summary>
        /// Дата статистики.
        /// </summary>
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(nameof(Date)); } }
        private DateTime _date;

        /// <summary>
        /// Сумма депозита.
        /// </summary>
        public decimal Deposit { get => _deposit; set { _deposit = value; OnPropertyChanged(nameof(Deposit)); } }
        private decimal _deposit;

        /// <summary>
        /// Внешний ключ — идентификатор счёта.
        /// </summary>
        public int AccountId { get => _accountId; set { _accountId = value; OnPropertyChanged(nameof(AccountId)); } }
        private int _accountId;

        /// <summary>
        /// Комментарий к записи.
        /// </summary>
        public string Comment { get => _comment; set { _comment = value; OnPropertyChanged(nameof(Comment)); } }
        private string _comment;

        /// <summary>
        /// Дата и время создания записи.
        /// </summary>
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(nameof(CreatedAt)); } }
        private DateTime _createdAt;
    }
}
