using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace CheckConsolidate
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "CheckConsolidate"
            };

            app.HelpOption("-?|-h|--help");

            var directoryOption = app.Option("-d|--directory",
                "Path to the packages folder to scan. Default value is <working directory>\\packages",
                CommandOptionType.SingleValue);
            var strictOption = app.Option("-s|--strict", "Enable strict mode.", CommandOptionType.NoValue);
            var reportOption = app.Option("-r|--reportOnly",
                "Only report consolidation issues but don't fail (return exit code 0).", CommandOptionType.NoValue);
            var excludeOption = app.Option("-e|--exclude", "Exclude a package id from the consolidation checks (can be specified multiple times).",
                CommandOptionType.MultipleValue);
            var verboseOption = app.Option("-v|--verbose", "enable verbose logging", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                if (verboseOption.HasValue())
                {
                    Console.WriteLine("Dumping option values:");
                    foreach (var option in app.Options)
                    {
                        switch (option.OptionType)
                        {
                            case CommandOptionType.NoValue:
                                Console.WriteLine($"{option.LongName}={option.HasValue()}");
                                break;
                            case CommandOptionType.SingleValue:
                                Console.WriteLine($"{option.LongName}={option.Value()}");
                                break;
                            case CommandOptionType.MultipleValue:
                                Console.WriteLine($"{option.LongName}={string.Join(", ", option.Values)}");
                                break;
                        }

                    }
                }
                var defaultDirPath = Path.Combine(Directory.GetCurrentDirectory(), "packages");

                string dirpath;
                if (directoryOption.HasValue())
                {
                    dirpath = directoryOption.Value();
                }
                else
                {
                    dirpath = defaultDirPath;
                }
                
                var scanner = new Scanner(dirpath);
                var res = scanner.FindPackages();
                if (res == null)
                {
                    if (scanner.Status == -1)
                    {
                        Console.WriteLine($"No packages folder found. All projects may be using PackageReferences.");
                        return 0;
                    }

                    Console.Error.WriteLine($"No packages returned. Failure in detection");
                    return -1;
                }

                var analyzer = new Analyzer(res, excludeOption.Values);

                if (analyzer.AllFine)
                {
                    Console.WriteLine("Solution has no nuget package consolidation issues");
                    return 0;
                }

                (reportOption.HasValue() ? Console.Out
                                         : Console.Error).WriteLine("The following packages needs consolidation:");

                if (strictOption.HasValue())
                {
                    foreach (var issue in analyzer.PackagesNeedingConsolidation)
                    {
                        (reportOption.HasValue() ? Console.Out
                                                 : Console.Error).WriteLine(issue);
                    }
                }
                else
                {
                    foreach (var issue in analyzer.PackagesAndVersionsNeedingConsolidation)
                    {
                        (reportOption.HasValue() ? Console.Out
                                                 : Console.Error).WriteLine(issue);
                    }
                }

                return reportOption.HasValue() ? 0 : analyzer.Count;
            });

            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
