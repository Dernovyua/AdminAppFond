using AdminPanelApp.Models;
using AdminPanelApp.Models.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Logic
{

    public class PortfolioReturnCalculator
    {
        /// <summary>
        /// Рассчитывает годовую доходность по методу XIRR
        /// </summary>
        /// <param name="statistics">Ежедневная статистика по депозиту</param>
        /// <param name="transactions">Транзакции пополнений/выводов</param>
        /// <param name="endDate">Дата окончания периода расчета</param>
        /// <param name="endBalance">Баланс на конец периода</param>
        /// <returns>Годовая доходность в десятичном формате (0.1 = 10%)</returns>
        public decimal CalculateXIRR(
            List<StatisticModel> statistics,
            List<TransactionModel> transactions,
            DateTime endDate,
            decimal endBalance)
        {
            if (statistics == null || !statistics.Any())
                return 0;

            // Собираем все денежные потоки
            var cashFlows = new List<CashFlowItem>();

            // 1. Начальный депозит (первая запись статистики)
            var initialDeposit = statistics.OrderBy(s => s.Date).First();
            cashFlows.Add(new CashFlowItem(initialDeposit.Date, -initialDeposit.Deposit));

            // 2. Добавляем транзакции
            foreach (var transaction in transactions.Where(t => t.ProcessedAt.HasValue))
            {
                decimal amount = 0;

                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                        amount = +transaction.Amount; // Положительный - ввод средств
                        break;
                    case TransactionType.ManagementFee:
                    case TransactionType.SeccessFee:
                    case TransactionType.Withdrawal:
                        amount = -transaction.Amount; // Отрицательный - вывод средств
                        break;
                }

                cashFlows.Add(new CashFlowItem(transaction.ProcessedAt.Value, amount));
            }

            // 3. Добавляем конечную стоимость (положительный cash flow)
            cashFlows.Add(new CashFlowItem(endDate, endBalance));

            // Сортируем по дате
            cashFlows = cashFlows.OrderBy(cf => cf.Date).ToList();

            // Рассчитываем XIRR
            return CalculateXIRRInternal(cashFlows);
        }

        /// <summary>
        /// Внутренняя реализация расчета XIRR методом Ньютона-Рафсона
        /// </summary>
        private decimal CalculateXIRRInternal(List<CashFlowItem> cashFlows, double guess = 0.1, double tolerance = 1e-6, int maxIterations = 100)
        {
            if (cashFlows.Count == 0)
                return 0;

            DateTime firstDate = cashFlows[0].Date;
            double[] amounts = cashFlows.Select(cf => (double)cf.Amount).ToArray();
            double[] days = cashFlows.Select(cf => (cf.Date - firstDate).TotalDays).ToArray();

            double x = guess;

            for (int i = 0; i < maxIterations; i++)
            {
                double fValue = 0;
                double fDerivative = 0;

                for (int j = 0; j < amounts.Length; j++)
                {
                    double exponent = days[j] / 365.0;
                    double factor = Math.Pow(1 + x, exponent);
                    fValue += amounts[j] / factor;

                    if (Math.Abs(x + 1) > tolerance)
                    {
                        fDerivative -= amounts[j] * exponent * Math.Pow(1 + x, exponent - 1) / (factor * factor);
                    }
                }

                if (Math.Abs(fDerivative) < tolerance)
                    break;

                double xNew = x - fValue / fDerivative;

                if (Math.Abs(xNew - x) < tolerance)
                    return (decimal)xNew;

                x = xNew;
            }

            return (decimal)x;
        }

        /// <summary>
        /// Упрощенный расчет по методу Modified Dietz (если XIRR слишком сложен)
        /// </summary>
        public decimal CalculateModifiedDietz(
            List<StatisticModel> statistics,
            List<TransactionModel> transactions,
            DateTime startDate,
            DateTime endDate,
            decimal endBalance)
        {
            if (statistics == null || !statistics.Any())
                return 0;

            // Начальный баланс
            decimal startBalance = statistics.OrderBy(s => s.Date).First().Deposit;

            // Сумма всех пополнений и выводов
            decimal netCashFlow = 0;
            decimal weightedCashFlow = 0;

            double totalDays = (endDate - startDate).TotalDays;

            foreach (var transaction in transactions.Where(t => t.ProcessedAt.HasValue))
            {
                double daysInPeriod = (endDate - transaction.ProcessedAt.Value).TotalDays;
                double weight = daysInPeriod / totalDays;

                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                        netCashFlow += transaction.Amount;
                        weightedCashFlow += transaction.Amount * (decimal)weight;
                        break;
                    case TransactionType.Withdrawal:
                    case TransactionType.ManagementFee:
                    case TransactionType.SeccessFee:
                        netCashFlow -= transaction.Amount;
                        weightedCashFlow -= transaction.Amount * (decimal)weight;
                        break;
                }
            }

            // Расчет доходности за период
            decimal periodReturn = (endBalance - startBalance - netCashFlow) /
                                  (startBalance + weightedCashFlow);

            // Приведение к годовой доходности
            double years = totalDays / 365.0;
            decimal annualizedReturn = (decimal)Math.Pow(1 + (double)periodReturn, 1 / years) - 1;

            return annualizedReturn;
        }
    }
}
