namespace Banks.Entities
{
    public enum TransactionStatus
    {
        /// <summary>
        /// Operation success
        /// </summary>
        Success = 0,

        /// <summary>
        /// Operation failed
        /// </summary>
        Fail = 1,

        /// <summary>
        /// Operation cancelled
        /// </summary>
        Cancel = 2,

        /// <summary>
        /// Operation is pending
        /// </summary>
        Pending = 3,
    }
}