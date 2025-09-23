using AdminPanelApp.Models;
using AdminPanelApp.Models.Statistic;
using AdminPanelApp.Requests;
using AdminPanelApp.View;
using ClassControlsAndStyle.Dialogs;
using DevExpress.Utils.CommonDialogs.Internal;
using DevExpress.Xpf;
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
                .Where(t => t.ProcessedAt.Date > periodStart.Date && t.ProcessedAt.Date <= periodEnd.Date &&
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
            DateTime lastClosedDate = DateTime.Now;

            acc.StatResult.Balance = GetBalance(acc);
            acc.StatResult.TotalReturn = GetTotalPnl(acc, lastClosedDate);
            acc.StatResult.AnnualReturn = GetPnl(acc, lastClosedDate.AddYears(-1), lastClosedDate);
            if (acc.StatResult.AnnualReturn == Decimal.MinValue)
                acc.StatResult.AnnualReturn = acc.StatResult.TotalReturn;

            acc.StatResult.Return6Months = GetPnl(acc, lastClosedDate.AddMonths(-6), lastClosedDate);
            if (acc.StatResult.Return6Months == Decimal.MinValue)
                acc.StatResult.Return6Months = acc.StatResult.AnnualReturn;

            acc.StatResult.Return3Months = GetPnl(acc, lastClosedDate.AddMonths(-3), lastClosedDate);
            if (acc.StatResult.Return3Months == Decimal.MinValue)
                acc.StatResult.Return3Months = acc.StatResult.Return6Months;

            acc.StatResult.Return1Month = GetPnl(acc, lastClosedDate.AddMonths(-1), lastClosedDate);
            if (acc.StatResult.Return1Month == Decimal.MinValue)
                acc.StatResult.Return1Month = acc.StatResult.Return3Months;

            acc.StatResult.Return1Week = GetPnl(acc, lastClosedDate.AddDays(-6), lastClosedDate);
            if (acc.StatResult.Return1Week == Decimal.MinValue)
                acc.StatResult.Return1Week = acc.StatResult.Return1Month;

        }

        private static decimal GetPnl(AccountModel acc, DateTime fromDate, DateTime lastDate)
        {
            if (lastDate.Year == 1 || !acc.Statistics.Any())
                return 0m;

            var start = acc.Statistics
                .Where(s => s.Date <= fromDate.Date)
                .OrderByDescending(s => s.Date)
                .FirstOrDefault();

            //if (start == null)
            //{
            //    start = acc.Statistics.OrderBy(s => s.Date)
            //    .Where(s => s.Date <= lastDate.Date && s.Date>fromDate)
            //    .FirstOrDefault();
            //}

            if (start == null)
                return Decimal.MinValue;

            var end = acc.Statistics.OrderBy(s => s.Date)
                .Where(s => s.Date <= lastDate.Date)
                .LastOrDefault();

            if (start == null || end == null)// || start.Deposit == 0)
                return 0m;

            var trans = BuhCalc.GetFinRez(acc.Transaction, fromDate, lastDate);

            return (end.Deposit - start.Deposit - trans);// / start.Deposit * 100m;

        }

        private static decimal GetTotalPnl(AccountModel acc, DateTime lastTime)
        {
            if (!acc.Statistics.Any())
                return 0m;

            var start = acc.Statistics.OrderBy(s => s.Date).First().Deposit;
            var end = acc.Statistics.OrderBy(s => s.Date).LastOrDefault(s => s.Date <= lastTime)?.Deposit ?? 0m;
            var trans = BuhCalc.GetFinRez(acc.Transaction, new DateTime(), lastTime);

            return start == 0 ? 0m : end - trans;// / start * 100m;
        }

        private static decimal GetBalance(AccountModel acc)
        {
            if (!acc.Statistics.Any())
                return 0m;

            var end = acc.Statistics.OrderBy(s => s.Date).Last();

            if (end == null)
                return 0m;

            return end.Deposit;
        }

        public static void UpdateResultClient(Client client)
        {
            decimal deposit = 0m;
            decimal withdrawal = 0m;
            decimal balance = 0m;
            decimal pnl = 0m;
            decimal comis = 0m;
            decimal paid = 0m;

            foreach (var item in client.Accounts)
            {
                deposit += item.Transaction.Where(t => t.Type == TransactionType.Deposit).Sum(a => a.Amount);
                withdrawal += item.Transaction.Where(t => t.Type == TransactionType.Withdrawal).Sum(a => a.Amount);
                comis += item.Transaction.Where(t => t.Type == TransactionType.SeccessFee).Sum(a => a.Amount);
                paid += item.Transaction.Where(t => t.Type == TransactionType.SeccessFee).Sum(a => a.Paid);
                comis += item.Transaction.Where(t => t.Type == TransactionType.ManagementFee).Sum(a => a.Amount);
                paid += item.Transaction.Where(t => t.Type == TransactionType.ManagementFee).Sum(a => a.Paid);
                balance += item.StatResult.Balance;
                pnl += item.StatResult.TotalReturn;
            }

            client.DepositAmount = deposit;
            client.WithdrawalAmount = withdrawal;
            client.Balance = balance;
            client.ProfitLoss = pnl;
            client.PaidAmount = paid;
            client.AccruedAmount = comis;
        }

        public static void CalcHandSuccessFee(Client client, AccountModel acc)
        {
            decimal pnl = 0m;
            decimal comis = 0m;
            comis += acc.Transaction.Where(t => t.Type == TransactionType.SeccessFee).Sum(a => a.Amount);
            pnl += acc.StatResult.TotalReturn;

            var calc = pnl / 100 * (decimal)client.SuccessFee;

            if (calc > comis)
            {
                TransactionModel transaction = new TransactionModel();

                transaction.AccountId = acc.Id;
                transaction.Type = TransactionType.SeccessFee;


                transaction.Amount = Math.Round(calc - comis, 2);
                transaction.ProcessedAt = DateTime.Now;
                transaction.Comment = "Ручной расчет платы за успех";
                transaction.Status = "completed"; // или другое значение по умолчанию

                TransactionRequests.AddTransaction(transaction);
                acc.Transaction.Insert(0, transaction);
                SetPnl(acc);
                UpdateResultClient(client);
                CalcSuccessFee(acc);
            }
        }


        public static void CalcSuccessFee(AccountModel acc)
        {
            decimal comis = 0m;
            decimal paid = 0m;
            comis += acc.Transaction.Where(t => t.Type == TransactionType.SeccessFee).Sum(a => a.Amount);
            paid += acc.Transaction.Where(t => t.Type == TransactionType.SeccessFee).Sum(a => a.Paid);

            acc.StatResult.Fee = Math.Round(comis, 2);
            acc.StatResult.Paid = Math.Round(paid, 2);

        }
    }
}