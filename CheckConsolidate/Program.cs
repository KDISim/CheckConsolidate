using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace CheckConsolidate
{
    public class Program
    {
        public static int Main(string[] args)
        {
            bool concise=!(args.Length == 1 && args[0] == "-s");

            var dirpath = System.IO.Directory.GetCurrentDirectory();
            var scanner = new Scanner(dirpath);
            var res = scanner.FindPackages();
            var analyzer = new Analyzer(res);

            if (analyzer.AllFine)
            {
                Console.WriteLine("Solution has no nuget package consolidation issues");
                return 0;
            }
            Console.WriteLine("The following packages needs consolidation:");
            if (concise)
            {
                foreach (var issue in analyzer.PackagesNeedingConsolidation)
                {
                    Console.WriteLine(issue);
                }
            }
            else
            {
                foreach (var issue in analyzer.PackagesAndVersionsNeedingConsolidation)
                {
                    Console.WriteLine(issue);
                }
            }
            return analyzer.Count ;
        }
    }
}
