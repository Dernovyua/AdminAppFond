using AdminPanelApp.Logic;
using DevExpress.XtraTreeList.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class StatisticDisplayModel : ObservableObject
    {
        //public Client ClientName { get; set; }

        /// <summary>
        /// Счет клиента
        /// </summary>
        public AccountModel Account { get => _account; set { _account = value; OnPropertyChanged(nameof(Account)); } }
        private AccountModel _account = new();

        /// <summary>
        /// Последний закрытый день (последняя дата в статистике)
        /// </summary>
        public DateTime LastClosedDate => Account.Statistics.Any()
            ? Account.Statistics.Max(s => s.Date)
            : DateTime.MinValue;

        private decimal CalculateReturn(DateTime fromDate)
        {
            var lastDate = LastClosedDate;

            if (LastClosedDate.Year == 1 || !Account.Statistics.Any())
                return 0m;

            // Берём первую запись после fromDate и последнюю на LastClosedDate
            var start = Account.Statistics
                .Where(s => s.Date >= fromDate && s.Date <= lastDate)
                .OrderBy(s => s.Date)
                .FirstOrDefault();

            var end = Account.Statistics
                .Where(s => s.Date == lastDate)
                .FirstOrDefault();

            if (start == null || end == null || start.Deposit == 0)
                return 0m;

            var trans = BuhCalc.GetFinRez(Account.Transaction, fromDate, lastDate);

            return (end.Deposit - trans);// / start.Deposit * 100m;
        }

        public decimal TotalReturn
        {
            get
            {
                if (!Account.Statistics.Any())
                    return 0m;

                var start = Account.Statistics.OrderBy(s => s.Date).First().Deposit;
                var end = Account.Statistics.FirstOrDefault(s => s.Date == LastClosedDate)?.Deposit ?? 0m;
                var trans = BuhCalc.GetFinRez(Account.Transaction, new DateTime(), LastClosedDate);

                return start == 0 ? 0m : end - trans;// / start * 100m;
            }
        }
        public decimal Return6Months
        {
            get
            {
                if (LastClosedDate.Year == 1) // или if (LastClosedDate == DateTime.MinValue)
                    return 0m;

                return CalculateReturn(LastClosedDate.AddMonths(-6));
            }
        }

        public decimal Return3Months
        {
            get
            {
                if (LastClosedDate.Year == 1)
                    return 0m;

                return CalculateReturn(LastClosedDate.AddMonths(-3));
            }
        }

        public decimal Return1Month
        {
            get
            {
                if (LastClosedDate.Year == 1)
                    return 0m;

                return CalculateReturn(LastClosedDate.AddMonths(-1));
            }
        }

        public decimal Return1Week
        {
            get
            {
                if (LastClosedDate.Year == 1)
                    return 0m;

                return CalculateReturn(LastClosedDate.AddDays(-7));
            }
        }

        public decimal AnnualReturn
        {
            get
            {
                if (LastClosedDate.Year == 1)
                    return 0m;

                return CalculateReturn(LastClosedDate.AddYears(-1));
            }
        }
        public decimal Balance => GetBalance();


        private decimal GetBalance()
        {
            var lastDate = LastClosedDate;

            var end = Account.Statistics
                .Where(s => s.Date == lastDate)
                .FirstOrDefault();

            if (end == null )
                return 0m;

            return end.Deposit;
        }



        public void Statistics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Когда в статистику добавляется или меняется элемент — уведомляем о смене доходностей
            RaiseAllReturnsPropertiesChanged();
        }

        private void RaiseAllReturnsPropertiesChanged()
        {
            OnPropertyChanged(nameof(Return6Months));
            OnPropertyChanged(nameof(Return3Months));
            OnPropertyChanged(nameof(Return1Month));
            OnPropertyChanged(nameof(Return1Week));
            OnPropertyChanged(nameof(AnnualReturn));
            OnPropertyChanged(nameof(TotalReturn));
            OnPropertyChanged(nameof(Balance));
            OnPropertyChanged(nameof(LastClosedDate));
        }
    }
}
