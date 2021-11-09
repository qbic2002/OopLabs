namespace Banks.Entities
{
    public interface IDepositPercentStrategy
    {
        decimal Calculate(decimal startDeposit);
    }
}