using FNBCoreETL.Framework;
using FNBCoreETL.Logging;
using FNBTellerETL.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLConsole
{
    class Program
    {
        private static void WriteToConsole(ETLAppLogEventArgs e)
        {
            Console.WriteLine($"{e.Severity} {e.ShortMsg} : {e.LongMsg}");
        }

        static void Main(string[] args)
        {
            FNBTellerETL.Bootstrap.Go(Assembly.GetExecutingAssembly().GetName().Name,
                System.Environment.MachineName,
                FileConfiguration.Environment.Value,
                Guid.NewGuid(),
                FileConfiguration.Database.ConnStrFNBCustom.Value,
                VerbosityEnum.All);

            EventManager.SubscribeToAppLog(VerbosityEnum.All, WriteToConsole);

            ETLFramework entryPoint = new FNBTellerETL.EntryPoint();

            //add in old / custom chain???

            //var arguments = new string[1];
            //arguments[0] = "normalChain";

            
            entryPoint.Go(args);
                       

            if (FileConfiguration.Environment.Value == "LCL")
            {
                Console.WriteLine("Press Any Key ...");
                Console.ReadKey();
            }
        }
    }
}
