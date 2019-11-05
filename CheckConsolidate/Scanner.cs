using System;
using System.Collections.Generic;
using System.Linq;
using WrapThat.SystemIO;

namespace CheckConsolidate
{
    public class Scanner
    {

        private readonly IDirectory dir;
        private readonly string directoryPath;

        public int Status { get; private set; }

        public Scanner(string directoryPath, IDirectory pdir = null)
        {
            this.directoryPath = directoryPath;

            dir = pdir ?? new Directory();
            Status = 0;
        }

        public IEnumerable<Package> FindPackages()
        {
            if (!dir.Exists(directoryPath))
            {
                Status = -1;
                return null;
            }

            var packages = dir.GetDirectories(directoryPath);
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

            Status = 1;
            return list;
        }


        public (string Name, string Version) Parse(string packageName)
        {
            if (packageName.Contains("\\"))
            {
                packageName = packageName.Substring(packageName.LastIndexOf('\\')+1);
            }
            
            var split = packageName.Split('.').ToList();
            int m = split.Count;
            int n = split.FindIndex(IsDigitsOnly);
            if (n == -1) // not found
                return (packageName, "");
            int vFields = m - n;
            if (vFields > 4)
            {
                n += (vFields - 4);
            }
          
            string name = string.Join(".", split.Take(n));
            string version = string.Join(".", split.Skip(n));
            return (name, version);
        }

        private static bool IsDigitsOnly(string str) => str.All(ch => ch >= '0' && ch <= '9');
    }
}