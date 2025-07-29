using System.ComponentModel;

public enum TransactionType
{
    [Description("Пополнение")]
    Deposit,

    [Description("Снятие")]
    Withdrawal,

    [Description("Комиссия")]
    ManagementFee
}