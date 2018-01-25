using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckConsolidate;
using NSubstitute;
using NUnit.Framework;
using WrapThat.SystemIO;

namespace CheckConsolidateTests
{
    public class ScannerTests
    {
        [TestCase("Microsoft.VisualStudio.TextTemplating.Interfaces.11.0.11.0.50727", "Microsoft.VisualStudio.TextTemplating.Interfaces.11","0.11.0.50727")]
        [TestCase("MyTool.Sys", "MyTool.Sys", "")]
        [TestCase("NUnit3TestAdapter.3.8.1-debug03-dbg", "NUnit3TestAdapter", "3.8.1-debug03-dbg")]
        [TestCase("System.Reflection.TypeExtensions.4.3.0", "System.Reflection.TypeExtensions", "4.3.0")]
        public void ThatParsingWorks(string fullname, string expectedname, string expectedversion)
        {

            var scanner = new Scanner("whatever");

            var res = scanner.Parse(fullname);

            Assert.Multiple(() =>
            {
                Assert.That(res.Name, Is.EqualTo(expectedname));
                Assert.That(res.Version, Is.EqualTo(expectedversion));
            });


        }

        [Test]
        public void ThatPackagingWorks()
        {
            var packages = new[]
            {
                "NUnit3TestAdapter.3.8.1-debug03-dbg", "NUnit3TestAdapter.3.8.1","NUnit3TestAdapter.3.8.1", "NUnit3TestAdapter.3.8.0",
                "System.Reflection.TypeExtensions.4.3.0","MyTool.Is.Superb"
            };

            var dir = Substitute.For<IDirectory>();
            dir.Exists(Arg.Any<string>()).Returns(true);
            dir.GetDirectories(Arg.Any<string>()).Returns(packages);
            var scanner = new Scanner("whatever", dir);

            var res = scanner.FindPackages().ToList();
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Count, Is.EqualTo(3));
            var nunit = res.FirstOrDefault(o => o.Name.Contains("NUnit"));
            Assert.That(nunit.Versions.Count(),Is.EqualTo(3));
            var types = res.FirstOrDefault(o => o.Name.Contains("Type"));
            Assert.That(types.Versions.Count(), Is.EqualTo(1));
            var mytool = res.FirstOrDefault(o => o.Name.Contains("MyTool"));
            Assert.That(mytool,Is.Not.Null);
            Assert.That(mytool.Versions.FirstOrDefault(), Is.Not.Null);
            Assert.That(mytool.Versions.Count(), Is.EqualTo(1));
            var nunitversion = nunit.Versions.First();
            Assert.That(nunit.Name,Does.Not.EndWith("."));
            Assert.That(nunitversion,Does.Not.StartsWith("."));
            Assert.That(scanner.Status, Is.EqualTo(1));
        }

        [Test]
        public void ThatItDontCrashWhenNoPackagesDirectory()
        {
            var dir = Substitute.For<IDirectory>();
            dir.Exists(Arg.Any<string>()).Returns(false);
            var scanner = new Scanner("whatever", dir);

            var res = scanner.FindPackages();
            Assert.That(res,Is.Null);
            Assert.That(scanner.Status,Is.EqualTo(-1));
        }



    }
}
