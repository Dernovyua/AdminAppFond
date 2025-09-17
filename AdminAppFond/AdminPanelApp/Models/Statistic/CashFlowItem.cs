using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models.Statistic
{
    public class CashFlowItem
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public CashFlowItem(DateTime date, decimal amount)
        {
            Date = date;
            Amount = amount;
        }
    }
}
