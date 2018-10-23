using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Ven_BowlCMD
{
    internal class Input
    {
        public TextReader ConIn = Console.In;

        internal bool ConfirmYes()
        {
            string input = ConIn.ReadLine();
            return input.ToLower().StartsWith('y');
        }

        internal List<int> GetNumberList()
        {
            string input = ConIn.ReadLine();
            return input.Split(',').Select(int.Parse).ToList();
        }


    }
}
