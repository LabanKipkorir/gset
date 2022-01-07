using System;
using System.IO;
using System.Reflection;

namespace CSETWebCore.DatabaseManager.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            if(args == null || args.Length < 1)
            {
                const string msg = "ERROR: Database connection parameter not defined";
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(msg);
                Console.Out.WriteLine($"Error: {msg}");
                return -1;
            }
            try
            {
                DbManager dbManager = new DbManager(typeof(CSETWebCore.Api.Interfaces.ILogger).Assembly.GetName().Version, args[1]);
                dbManager.SetupDb();
            } catch (Exception e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.Message);
                Console.Out.WriteLine($"ERROR: {e.Message}");
                return -1;
            }


            Console.Out.WriteLine("Database setup complete");

            return 0;
        }
    }
}
