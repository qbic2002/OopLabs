using System;
using System.Collections.Generic;
using System.Linq;

namespace BackupsExtra
{
    internal class Program
    {
        private static List<int> _ints = new ();
        private static void Main()
        {
            _ints.Add(1);
            _ints.Add(3);
            _ints.Add(2);
            Del(_ints);

            // _ints.ForEach(Console.WriteLine);
        }

        private static void Del(List<int> ints)
        {
            ints.Sort((i, i1) =>
            {
                if (i < i1)
                    return -1;
                if (i == i1)
                    return 0;
                return 1;
            });
            ints.RemoveAt(0);
        }
    }
}
