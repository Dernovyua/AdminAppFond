using AdminPanelApp.Models;
using AdminPanelApp.Models.Statistic;
using AdminPanelApp.View;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraTreeList.Data;
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

        private static decimal GetFinRez(IEnumerable<TransactionModel> allTransactions,
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


            return (decimal)(deposits.Sum(t => t.Amount) - withdrawals.Sum(t => t.Amount));
        }



        public static void SetPnl(AccountModel acc)
        {
            DateTime lastClosedDate = acc.Statistics.Any()
            ? acc.Statistics.Max(s => s.Date)
            : DateTime.MinValue;


            acc.StatResult.Balance = GetBalance(acc);
            acc.StatResult.TotalReturn = GetTotalPnl(acc, lastClosedDate);
            acc.StatResult.Return1Week = GetPnl(acc, lastClosedDate.AddDays(-6), lastClosedDate);
            acc.StatResult.Return1Month = GetPnl(acc, lastClosedDate.AddMonths(-1), lastClosedDate);
            acc.StatResult.Return3Months = GetPnl(acc, lastClosedDate.AddMonths(-3), lastClosedDate);
            acc.StatResult.Return6Months = GetPnl(acc, lastClosedDate.AddMonths(-6), lastClosedDate);
            acc.StatResult.AnnualReturn = GetPnl(acc, lastClosedDate.AddYears(-1), lastClosedDate);

        }

        private static decimal GetPnl(AccountModel acc, DateTime fromDate, DateTime lastDate)
        {
            if (lastDate.Year == 1 || !acc.Statistics.Any())
                return 0m;

            var start = acc.Statistics
            .Where(s => s.Date >= fromDate && s.Date <= lastDate)
            .OrderBy(s => s.Date)
            .FirstOrDefault();

            var end = acc.Statistics
                .Where(s => s.Date == lastDate)
                .FirstOrDefault();

            if (start == null || end == null || start.Deposit == 0)
                return 0m;

            var trans = BuhCalc.GetFinRez(acc.Transaction, fromDate, lastDate);

            return (end.Deposit - trans);// / start.Deposit * 100m;

        }

        private static decimal GetTotalPnl(AccountModel acc, DateTime lastTime)
        {
            if (!acc.Statistics.Any())
                return 0m;

            var start = acc.Statistics.OrderBy(s => s.Date).First().Deposit;
            var end = acc.Statistics.FirstOrDefault(s => s.Date == lastTime)?.Deposit ?? 0m;
            var trans = BuhCalc.GetFinRez(acc.Transaction, new DateTime(), lastTime);

            return start == 0 ? 0m : end - trans;// / start * 100m;
        }

        private static decimal GetBalance(AccountModel acc)
        {
            var end = acc.Statistics.OrderBy(s => s.Date).Last();

            if (end == null)
                return 0m;

            return end.Deposit;
        }
    }
}