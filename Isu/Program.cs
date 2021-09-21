using Isu.Entities;
using Isu.Services;

namespace Isu
{
    internal class Program
    {
        private static void Main()
        {
            GroupValidator groupValidator = new GroupValidator('M', 3, 4, 30, 2, 3, 4, 30);
            Service serv = new Service(groupValidator);
            serv.AddGroup("M3201");
            serv.AddGroup("M3201");
        }
    }
}