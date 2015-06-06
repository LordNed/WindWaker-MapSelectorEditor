using System;

namespace CLIReadout
{
    class Program
    {
        static void Main(string[] args)
        {
            string menuFile = "C:\\Users\\Matt\\Downloads\\Menu1.dat";

            DebugMenuModifier reader = new DebugMenuModifier();
            reader.Load(menuFile);


            reader.Save("C:\\Users\\Matt\\Downloads\\Menu1_Out.dat");
            
            Console.WriteLine("Finished");
        }
    }
}
