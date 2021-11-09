using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            ITransaction putTransaction = bankAccount1.PutCredits(100000);
            Console.WriteLine(bankAccount1.Credits);

            BankAccountId id2 = bankAccount2.Id;
            try
            {
                bankAccount1.TransferCredits(100, id2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine(bankAccount1.Credits);
            Console.WriteLine(bankAccount2.Credits);

            for (int i = 0; i < 365; i++)
            {
                centralBank.ChargeInterest();
            }

            centralBank.AddInterest();

            Console.WriteLine(bankAccount1.Credits);
            Console.WriteLine(bankAccount2.Credits);

            testClient1.ReceiveNotification(false);
            bank1.SetPercents(new DefaultDepositPercentStrategy(), 0.1M);
            bank2.SetPercents(new DefaultDepositPercentStrategy(), 0.01M);

            Console.WriteLine(testClient1.Notifications.FirstOrDefault());
            Console.WriteLine(testClient2.Notifications.FirstOrDefault());

            bankAccount2.WithdrawCredits(2);

            for (int i = 0; i < 365; i++)
            {
                centralBank.ChargeInterest();
            }

            centralBank.AddInterest();

            Console.WriteLine(bankAccount1.Credits);
            Console.WriteLine(bankAccount2.Credits);
        }
    }
}
