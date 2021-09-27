using System;
using System.Collections.Generic;
using Isu.Entities;
using Isu.Services;

namespace Isu
{
    internal class Program
    {
        private static GroupValidator _groupValidator = new GroupValidator('M', 3, 4, 30, 2, 3, 4);
        private static Service _isuService = new Service(_groupValidator, 30);
        private static void Main()
        {
        }
    }
}