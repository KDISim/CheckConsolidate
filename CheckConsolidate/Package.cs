using System;
using System.Collections.Generic;

namespace CheckConsolidate
{
    public class Package
    {
        public string Name { get; set; }
        private List<string> versions = new List<string>();

        public IEnumerable<string> Versions => versions;
        public Package(string name, string version)
        {
            Name = name;
            versions.Add(version);
        }

        public Package(Tuple<string, string> p) : this(p.Item1, p.Item2)
        {

        }

        public void AddVersion(string pv)
        {
            versions.Add(pv);
        }

        public bool Ok => versions.Count == 1;
    }
}