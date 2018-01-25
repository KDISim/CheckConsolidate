using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace CheckConsolidate
{
    public class Program
    {
        public static int Main(string[] args)
        {
            bool concise = false;
            bool reportOnly = false;
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg == "-s")
                        concise = true;
                    if (arg == "-r")
                        reportOnly = true;
                }
            }
            var dirpath = System.IO.Directory.GetCurrentDirectory();
            var scanner = new Scanner(dirpath);
            var res = scanner.FindPackages();
            if (res == null)
            {
                if (scanner.Status == -1)
                {
                    Console.WriteLine($"No packages folder found. All projects may be using PackageReferences.");
                    return 0;
                }
                Console.WriteLine($"No packages returned. Failure in detection");
                return -1;

            }
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
            return reportOnly ? 0 : analyzer.Count;
        }
    }
}
