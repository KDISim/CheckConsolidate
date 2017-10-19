using System;
using System.Collections.Generic;
using System.Linq;
using WrapThat.SystemIO;

namespace CheckConsolidate
{
    public class Scanner
    {

        private IDirectory dir;
        private string directoryPath;

        public Scanner(string directoryPath, IDirectory pdir = null)
        {
            this.directoryPath = directoryPath;

            dir = pdir ?? new Directory();
        }

        public IEnumerable<Package> FindPackages()
        {
            var packagePath = System.IO.Path.Combine(directoryPath, "packages");
            var packages = dir.GetDirectories(packagePath);
            var list = new List<Package>();
            foreach (var package in packages)
            {
                var p = new Package(Parse(package).ToTuple());
                var e = list.FirstOrDefault(m => m.Name == p.Name);
                if (e != null)
                {
                    var pv = p.Versions.First();
                    var v = e.Versions.FirstOrDefault(o => o == pv);
                    if (v == null)
                        e.AddVersion(pv);

                }
                else
                {
                    list.Add(p);
                }



            }
            return list;
        }


        public (string Name, string Version) Parse(string packagename)
        {
            var split = packagename.Split('.').ToList();
            int n = split.FindIndex(IsDigitsOnly);
            if (n == -1) // not found
                return (packagename, "");
            string name = string.Join(".", split.Take(n));
            string version = string.Join(".", split.Skip(n));
            return (name, version);
        }
        static bool IsDigitsOnly(string str) => str.All(ch => ch >= '0' && ch <= '9');
    }
}