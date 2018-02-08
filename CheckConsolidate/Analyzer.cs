using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckConsolidate
{
    public class Analyzer
    {
        private readonly IEnumerable<Package> allPackages;
        private readonly IEnumerable<string> exclusions;

        public Analyzer(IEnumerable<Package> allPackages, IEnumerable<string> exclusions)
        {
            this.allPackages = allPackages;
            this.exclusions = exclusions;
        }

        private IEnumerable<Package> PackagesToAnalyze => allPackages.Where(p => !p.Ok).Where(p => !Exclude(p));

        public bool AllFine => PackagesToAnalyze.All(o => o.Ok);
        

        public IEnumerable<string> PackagesNeedingConsolidation => PackagesToAnalyze.Select(m => m.Name);

        public IEnumerable<string> PackagesAndVersionsNeedingConsolidation => PackagesToAnalyze.Select(ToConsolidateString);

        private bool Exclude(Package package)
        {
            return exclusions.Any(e => e.Equals(package.Name, StringComparison.OrdinalIgnoreCase));
        }

        private string ToConsolidateString(Package package)
        {
            return package.Name + ": " + string.Join("; ", package.Versions);
        }

        public int Count => allPackages.Count(o => !o.Ok);
    }
}