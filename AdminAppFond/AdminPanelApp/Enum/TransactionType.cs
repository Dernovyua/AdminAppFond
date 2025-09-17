using System.ComponentModel;

public enum TransactionType
{
    [Description("Пополнение")]
    Deposit,

    [Description("Снятие")]
    Withdrawal,

    [Description("Комиссия за управление")]
    ManagementFee,

    [Description("Плата за успех")]
    SeccessFee
}

