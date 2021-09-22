using System;
using Isu.Entities;
using Isu.Services;

namespace Isu
{
    internal class Program
    {
        private static void Main()
        {
            var groupValidator = new GroupValidator('M', 3, 4, 30, 2, 3, 4);
            var service = new Service(groupValidator, 30);
        }
    }
}