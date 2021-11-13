using Banks.Tools;

namespace Banks.Entities
{
    public class Passport
    {
        public Passport(int number)
        {
            if (number < 100000 || number > 999999)
                throw new BanksException("Incorrect passport");
            Number = number;
        }

        public int Number { get; }
        public override string ToString()
        {
            return Number.ToString();
        }
    }
}