namespace Banks.Entities
{
    public enum TransactionType
    {
        /// <summary>
        /// Withdraw money
        /// </summary>
        Withdraw = 0,

        /// <summary>
        /// Put money on bank account
        /// </summary>
        Put = 1,

        /// <summary>
        /// Transfer credits to another account
        /// </summary>
        Transfer = 2,
    }
}