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
            // JoinTrainingGroupManager jM = new JoinTrainingGroupManager();
            // JoinTrainingGroup jG = jM.AddJTG(Faculty.TIT);
            var list = new List<int>
            {
                1,
                2,
                4,
            };
            ReadOnlyCollection<int> rolist = new ReadOnlyCollection<int>(list);
            rolist.ToList().Add(7);
            list.ForEach(Console.WriteLine);
        }
    }
}
