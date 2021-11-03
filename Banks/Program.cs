using System;
using Banks.Entities;
using Banks.Services;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            var centralBank = new CentralBank();
            Bank bank1 = centralBank.AddBank("Sber");
            Client testClient1 = bank1.AddClient("TestFN", "TestLN");
            IBankAccount bankAccount1 = testClient1.CreateBankAccount(BankAccountType.Debit);

            Bank bank2 = centralBank.AddBank("Spb");
            Client testClient2 = bank2.AddClient("TestFN", "TestLN");
            IBankAccount bankAccount2 = testClient2.CreateBankAccount(BankAccountType.Debit);

            Console.WriteLine(bankAccount1.Credits);
            ITransaction putTransaction = bankAccount1.PutCredits(1000);
            Console.WriteLine(bankAccount1.Credits);
            ITransaction transferTransaction = bankAccount1.TransferCredits(200, bankAccount2);
            Console.WriteLine(bankAccount1.Credits);
            Console.WriteLine(bankAccount2.Credits);
            transferTransaction.Cancel();
            Console.WriteLine(bankAccount1.Credits);
            Console.WriteLine(bankAccount2.Credits);
        }
    }
}
