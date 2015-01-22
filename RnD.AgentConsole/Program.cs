using System;

namespace RnD.AgentConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Agent.Shell agent = new Agent.Shell();

            agent.Start();
            Console.WriteLine("Press Enter to quit.");
            Console.ReadLine();
            //Console.ReadLine();
            //agent.DownloadMaterials();
            //Console.ReadLine();
            //  agent.Register();
        }
    }
}
