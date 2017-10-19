using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FST_WindowsService;

namespace FSTServiceConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            FSTService s = new FSTService();
            s.Initialize();
            s.StartProcess();
        }
    }
}
