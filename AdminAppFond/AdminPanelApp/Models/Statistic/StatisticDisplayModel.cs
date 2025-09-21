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
        public decimal TotalReturn
        {
            get => _totalReturn;
            set { _totalReturn = value; OnPropertyChanged(nameof(TotalReturn)); }
        }
        private decimal _totalReturn;

        public decimal Return6Months
        {
            get => _return6Months;
            set { _return6Months = value; OnPropertyChanged(nameof(Return6Months)); }
        }
        private decimal _return6Months;

        public decimal Return3Months
        {
            get => _return3Months;
            set { _return3Months = value; OnPropertyChanged(nameof(Return3Months)); }
        }
        private decimal _return3Months;

        public decimal Return1Month
        {
            get => _return1Month;
            set { _return1Month = value; OnPropertyChanged(nameof(Return1Month)); }
        }
        private decimal _return1Month;

        public decimal Return1Week
        {
            get => _return1Week;
            set { _return1Week = value; OnPropertyChanged(nameof(Return1Week)); }
        }
        private decimal _return1Week;

        public decimal AnnualReturn
        {
            get => _annualReturn;
            set { _annualReturn = value; OnPropertyChanged(nameof(AnnualReturn)); }
        }
        private decimal _annualReturn;

        public decimal Balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(nameof(Balance)); }
        }
        private decimal _balance;
    }
}
