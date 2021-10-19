using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IsuExtra;
using IsuExtra.Entities;
using IsuExtra.Services;

namespace IsuExtra
{
    internal class Program
    {
        private static void Main()
        {
            List<int> list1 = new List<int>()
            {
                55,
                1,
                2,
            };
            List<int> list2 = new List<int>()
            {
                55,
                2,
                1,
            };
            Console.WriteLine(list1.SequenceEqual(list2));
        }
    }
}
