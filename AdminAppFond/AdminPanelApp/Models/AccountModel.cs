using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class AccountModel : ObservableObject
    {
        /// <summary>
        /// Уникальный идентификатор аккаунта.
        /// </summary>
        public int Id { get => _id; set { _id = value; OnPropertyChanged(nameof(Id)); } }
        private int _id;

        /// <summary>
        /// Внешний ключ — идентификатор клиента, которому принадлежит аккаунт.
        /// </summary>
        public int ClientId { get => _clientId; set { _clientId = value; OnPropertyChanged(nameof(ClientId)); } }
        private int _clientId;

        /// <summary>
        /// Название биржи (например, Binance, Bybit).
        /// </summary>
        public string Exchange { get => _exchange; set { _exchange = value; OnPropertyChanged(nameof(Exchange)); } }
        private string _exchange;

        /// <summary>
        /// Имя или описание аккаунта.
        /// </summary>
        public string AccountName { get => _accountName; set { _accountName = value; OnPropertyChanged(nameof(AccountName)); } }
        private string _accountName;

        /// <summary>
        /// Уникальный номер счета (публичный ключ).
        /// </summary>
        public string AccountNumber { get => _accountNumber; set { _accountNumber = value; OnPropertyChanged(nameof(AccountNumber)); } }
        private string _accountNumber;

        /// <summary>
        /// Валюта счета.
        /// </summary>
        public CurrencyType Currency { get => _currency; set { _currency = value; OnPropertyChanged(nameof(Currency)); } }
        private CurrencyType _currency;

        /// <summary>
        /// Дата создания записи об аккаунте.
        /// </summary>
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(nameof(CreatedAt)); } }
        private DateTime _createdAt;



        /// <summary>
        /// Дата создания записи об аккаунте.
        /// </summary>
        public DateTime LastDateAddBalanceToStat { get => _lastDateAddBalanceToStat; set { _lastDateAddBalanceToStat = value; OnPropertyChanged(nameof(LastDateAddBalanceToStat)); } }
        private DateTime _lastDateAddBalanceToStat;


        /// <summary>
        /// Статистика по счетам, выгружается в память
        /// </summary>
        public ObservableCollection<StatisticModel> Statistics { get => _statistics; set { _statistics = value; OnPropertyChanged(nameof(Statistics)); } }
        private ObservableCollection<StatisticModel> _statistics = new ();




    }
}
