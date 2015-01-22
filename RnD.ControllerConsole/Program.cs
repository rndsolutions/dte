using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RnD.Controller;

namespace RnD.ControllerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server");

            Server.Instance.Start();
            Console.WriteLine("Server is running on: {0}", RnD.Controller.Server.BaseAddress);

            Console.WriteLine("Press Enter to quit.");
            Console.ReadLine();
            RnD.Controller.Server.Instance.Stop();
        }
    }
}
