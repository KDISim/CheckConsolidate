using System.Collections.Generic;
using System.Linq;

namespace CheckConsolidate
{
    public class Analyzer
    {
        private readonly IEnumerable<Package> packages;

        public Analyzer(IEnumerable<Package> packages)
        {
            this.packages = packages;
        }

        public bool AllFine => packages.All(o => o.Ok);

        public IEnumerable<string> PackagesNeedingConsolidation => packages.Where(o => !o.Ok).Select(m => m.Name);

        public IEnumerable<string> PackagesAndVersionsNeedingConsolidation
        {
            get
            {
                var consolidates = packages.Where(o => !o.Ok);
                return consolidates.Select(p => p.Name + ": " + string.Join("; ", p.Versions)).ToList();
            }
        }

        public int Count => packages.Count(o => !o.Ok);
    }
}