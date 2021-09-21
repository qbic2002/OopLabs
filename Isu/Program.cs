using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Isu.Entities;
using Isu.Services;

namespace Isu
{
    internal class Program
    {
        private static void Main()
        {
            var serv = new Service('M', 3, 4, 99, 2, 3, 4, 30);
            Group gr = serv.AddGroup("M3201");
        }
    }
}