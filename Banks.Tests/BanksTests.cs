using System;
using System.Linq;
using Banks.Entities;
using Banks.Services;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;

namespace Banks.Tests
{
    [TestFixture]
    public class BanksTests
    {
        private CentralBank _centralBank;
        private TimeManager _timeManager;

        [SetUp]
        public void SetUp()
        {
            _centralBank = new CentralBank();
            _timeManager = new TimeManager(_centralBank);
        }
        
        [TestCase(100)]
        [TestCase(20)]
        [TestCase(1)]
        [TestCase(5.6)]
        public void PutMoneyTest(decimal putMoney)
        {
            Bank bank1 = _centralBank.AddBank("Sperbank");
            Client testClient = bank1.AddClient("TestFN", "TestLN");
            IBankAccount debitAccount = testClient.CreateBankAccount(BankAccountType.Debit);
            IBankAccount creditAccount = testClient.CreateBankAccount(BankAccountType.Credit);
            IBankAccount depositAccount = testClient.CreateBankAccount(BankAccountType.Deposit);
            
            debitAccount.PutCredits(putMoney);
            creditAccount.PutCredits(putMoney);
            depositAccount.PutCredits(putMoney);
            
            Assert.AreEqual(putMoney, debitAccount.Credits);
            Assert.AreEqual(putMoney, creditAccount.Credits);
            Assert.AreEqual(putMoney, depositAccount.Credits);
        }

        [TestCase(100)]
        [TestCase(20)]
        [TestCase(1)]
        [TestCase(5.6)]
        public void WithdrawMoneyTest(decimal withdrawMoney)
        {
            Bank bank1 = _centralBank.AddBank("Sperbank");
            Client testClient = bank1.AddClient("TestFN", "TestLN");
            IBankAccount debitAccount = testClient.CreateBankAccount(BankAccountType.Debit);
            IBankAccount creditAccount = testClient.CreateBankAccount(BankAccountType.Credit);
            IBankAccount depositAccount = testClient.CreateBankAccount(BankAccountType.Deposit, 0, 0);
            depositAccount.AddOneDay();
            
            debitAccount.PutCredits(decimal.MaxValue);
            creditAccount.PutCredits(decimal.MaxValue);
            depositAccount.PutCredits(decimal.MaxValue);

            debitAccount.WithdrawCredits(withdrawMoney);
            creditAccount.WithdrawCredits(withdrawMoney);
            depositAccount.WithdrawCredits(withdrawMoney);
            
            Assert.AreEqual(decimal.MaxValue - withdrawMoney, debitAccount.Credits);
            Assert.AreEqual(decimal.MaxValue - withdrawMoney, creditAccount.Credits);
            Assert.AreEqual(decimal.MaxValue - withdrawMoney, depositAccount.Credits);
        }

        [TestCase(100)]
        [TestCase(20)]
        [TestCase(1)]
        [TestCase(5.6)]
        public void TransferMoneyToTheSameTest(decimal transferMoney)
        {
            Bank bank1 = _centralBank.AddBank("Sperbank");
            Client testClient = bank1.AddClient("TestFN", "TestLN");
            IBankAccount debitAccount = testClient.CreateBankAccount(BankAccountType.Debit);
            IBankAccount creditAccount = testClient.CreateBankAccount(BankAccountType.Credit);
            IBankAccount depositAccount = testClient.CreateBankAccount(BankAccountType.Deposit, 0, 0);
            depositAccount.AddOneDay();
            IBankAccount receiver = bank1.AddClient("1", "2").CreateBankAccount(BankAccountType.Debit);
            IBankAccount secondReceiver = _centralBank.AddBank("BSPB").AddClient("1", "2").CreateBankAccount(BankAccountType.Debit);

            debitAccount.PutCredits(transferMoney);
            debitAccount.TransferCredits(transferMoney, receiver.Id);
            Assert.AreEqual(transferMoney, receiver.Credits);
            Assert.AreEqual(0, debitAccount.Credits);
            debitAccount.Transactions[1].Cancel();
            
            creditAccount.PutCredits(transferMoney);
            creditAccount.TransferCredits(transferMoney, receiver.Id);
            Assert.AreEqual(transferMoney, receiver.Credits);
            Assert.AreEqual(0, creditAccount.Credits);
            creditAccount.Transactions[1].Cancel();
            
            depositAccount.PutCredits(transferMoney);
            depositAccount.TransferCredits(transferMoney, receiver.Id);
            Assert.AreEqual(transferMoney, receiver.Credits);
            Assert.AreEqual(0, depositAccount.Credits);
            depositAccount.Transactions[1].Cancel();
        }
        
        [TestCase(100)]
        [TestCase(20)]
        [TestCase(1)]
        [TestCase(5.6)]
        public void TransferMoneyToOtherTest(decimal transferMoney)
        {
            Bank bank1 = _centralBank.AddBank("Sperbank");
            Client testClient = bank1.AddClient("TestFN", "TestLN");
            IBankAccount debitAccount = testClient.CreateBankAccount(BankAccountType.Debit);
            IBankAccount creditAccount = testClient.CreateBankAccount(BankAccountType.Credit);
            IBankAccount depositAccount = testClient.CreateBankAccount(BankAccountType.Deposit, 0, 0);
            depositAccount.AddOneDay();
            IBankAccount receiver = _centralBank.AddBank("BSPB").AddClient("1", "2").CreateBankAccount(BankAccountType.Debit);

            debitAccount.PutCredits(transferMoney);
            debitAccount.TransferCredits(transferMoney, receiver.Id);
            Assert.AreEqual(transferMoney, receiver.Credits);
            Assert.AreEqual(0, debitAccount.Credits);
            debitAccount.Transactions[1].Cancel();
            
            creditAccount.PutCredits(transferMoney);
            creditAccount.TransferCredits(transferMoney, receiver.Id);
            Assert.AreEqual(transferMoney, receiver.Credits);
            Assert.AreEqual(0, creditAccount.Credits);
            creditAccount.Transactions[1].Cancel();
            
            depositAccount.PutCredits(transferMoney);
            depositAccount.TransferCredits(transferMoney, receiver.Id);
            Assert.AreEqual(transferMoney, receiver.Credits);
            Assert.AreEqual(0, depositAccount.Credits);
            depositAccount.Transactions[1].Cancel();
        }

        [TestCase(1000, 0.02)]
        [TestCase(500, 0.1)]
        public void AddPercentTest(decimal putCredits, decimal percent)
        {
            Bank bank1 = _centralBank.AddBank("Sperbank");
            Client testClient = bank1.AddClient("TestFN", "TestLN");
            bank1.SetDebitPercent(percent);
            bank1.SetDepositPercents(0.01M, new []{new DepositPercentRange(percent)});
            IBankAccount debitAccount = testClient.CreateBankAccount(BankAccountType.Debit);
            IBankAccount depositAccount = testClient.CreateBankAccount(BankAccountType.Deposit, putCredits);
            debitAccount.PutCredits(putCredits);
            _timeManager.SkipMonth();
            Assert.AreEqual((30 * putCredits * percent / 365) + putCredits, debitAccount.Credits);
            Assert.AreEqual((30 * putCredits * percent / 365) + putCredits, depositAccount.Credits);
        }
        
        [TestCase(100, 0.02)]
        [TestCase(500, 0.1)]
        public void AddCommissionTest(decimal withdrawCredits, decimal commission)
        {
            Bank bank1 = _centralBank.AddBank("Sperbank");
            Client testClient = bank1.AddClient("TestFN", "TestLN");
            bank1.SetCreditCommission(commission);
            bank1.SetDepositPercents(0.01M, new []{new DepositPercentRange(commission)});
            IBankAccount creditAccount = testClient.CreateBankAccount(BankAccountType.Credit);
            creditAccount.WithdrawCredits(withdrawCredits);
            _timeManager.SkipMonth();
            Assert.AreEqual(-withdrawCredits - (30 * commission), creditAccount.Credits);
        }
    }
}