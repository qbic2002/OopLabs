﻿using System;
using Banks.Entities;
using Banks.Services;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            CentralBank centralBank = new CentralBank();
            Bank bank = centralBank.AddBank("Sber");
            bank.AddClient("TestFN", "TestLN");
        }
    }
}
