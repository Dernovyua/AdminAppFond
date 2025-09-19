using AdminPanelApp.Models;
using AdminPanelApp.Models.Statistic;
using AdminPanelApp.View;
using DevExpress.XtraScheduler.Printing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Logic
{


    public static class BuhCalc
    {

        public static decimal GetFinRez(IEnumerable<TransactionModel> allTransactions,
                DateTime periodStart,
                DateTime periodEnd)
        {

            // Фильтруем транзакции по периоду
            var periodTransactions = allTransactions
                .Where(t => t.ProcessedAt >= periodStart && t.ProcessedAt.Date <= periodEnd && 
                (t.Type == TransactionType.Deposit || t.Type == TransactionType.Withdrawal))
                .ToList();

            // Рассчитываем пополнения (положительные Amount)
            var deposits = periodTransactions.Where(t => t.Type == TransactionType.Deposit);

            // Рассчитываем выводы (отрицательные Amount, берем по модулю)
            var withdrawals = periodTransactions.Where(t => t.Type == TransactionType.Withdrawal);


            return (decimal)( deposits.Sum(t => t.Amount)- withdrawals.Sum(t => t.Amount));
        }
    }
}