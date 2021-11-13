namespace Banks.Entities
{
    public interface IDepositPercent
    {
        decimal FirstPercent { get; }
        decimal SecondPercent { get; }
        decimal ThirdPercent { get; }
        decimal Calculate(decimal startDeposit);
        string ToString();
    }
}