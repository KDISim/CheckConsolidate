using System.Collections.Generic;
using System.Linq;
using CheckConsolidate;
using NSubstitute;
using NUnit.Framework;
using WrapThat.SystemIO;

namespace CheckConsolidateTests
{
    public class AnalyzerTests
    {
        private static readonly IEnumerable<string> NoExclusions = new List<string>();
        [Test]
        public void ThatItFindsAllNotOk()
        {
            var packages = new[]
            {
                "NUnit3TestAdapter.3.8.1-debug03-dbg", "NUnit3TestAdapter.3.8.1","NUnit3TestAdapter.3.8.1", "NUnit3TestAdapter.3.8.0",
                "System.Reflection.TypeExtensions.4.3.0","MyTool.Is.Superb"
            };
                
            var dir = Substitute.For<IDirectory>();
            
            dir.GetDirectories(Arg.Any<string>()).Returns(packages);
            var scanner = new Scanner("whatever", dir);
            dir.Exists(Arg.Any<string>()).Returns(true);
            var res = scanner.FindPackages().ToList();
            
            var analyzer = new Analyzer(res, NoExclusions);

            Assert.That(analyzer.AllFine,Is.False);

            var consolidates = analyzer.PackagesNeedingConsolidation;
            Assert.That(consolidates.Count(),Is.EqualTo(1));
            var consWithVersions = analyzer.PackagesAndVersionsNeedingConsolidation.ToList();
            Assert.That(consWithVersions.Count, Is.EqualTo(1));
            var cons = consWithVersions.First();
            Assert.Multiple(() =>
            {
                Assert.That(cons.Contains("3.8.1"));
                Assert.That(cons.Contains("3.8.0"));
                Assert.That(cons.Contains("debug03"));
                Assert.That(cons.Contains("NUnit3TestAdapter"));
                Assert.That(analyzer.Count, Is.EqualTo(1));
            });

        }

       
        [Test]
        public void ThatItFindsAllOk()
        {
            var packages = new[]
            {
                "NUnit3TestAdapter.3.8.1-debug03-dbg",
                "System.Reflection.TypeExtensions.4.3.0",
                "MyTool.Is.Superb"
            };

            var dir = Substitute.For<IDirectory>();
            
            dir.Exists(Arg.Any<string>()).Returns(true);
            dir.GetDirectories(Arg.Any<string>()).Returns(packages);
            var scanner = new Scanner("whatever", dir);
            
            var res = scanner.FindPackages().ToList();

            var analyzer = new Analyzer(res, NoExclusions);

            Assert.That(analyzer.AllFine, Is.True);
        }
        
        [Test]
        public void ThatItExcludes()
        {
            var packages = new[]
            {
                "NUnit3TestAdapter.3.8.1-debug03-dbg", "NUnit3TestAdapter.3.8.1","NUnit3TestAdapter.3.8.1", "NUnit3TestAdapter.3.8.0",
                "System.Reflection.TypeExtensions.4.3.0","MyTool.Is.Superb"
            };
                
            var dir = Substitute.For<IDirectory>();
            var exclusions = new List<string>();
            
            exclusions.Add("NUnit3TestAdapter");
            
            dir.GetDirectories(Arg.Any<string>()).Returns(packages);
            var scanner = new Scanner("whatever", dir);
            dir.Exists(Arg.Any<string>()).Returns(true);
            var res = scanner.FindPackages().ToList();

            var analyzer = new Analyzer(res, exclusions);
           
            Assert.That(analyzer.AllFine, Is.True);
        }

    }
}
