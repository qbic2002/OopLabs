using Banks.Tools;

namespace Banks.Entities
{
    public class BankAccountId
    {
        public BankAccountId(int id)
        {
            if (id < 0)
                throw new BanksException("Incorrect id");
            Id = id;
        }

        public int Id { get; }

        public override bool Equals(object obj)
        {
            return (obj is BankAccountId id) && id.Id.Equals(Id);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}