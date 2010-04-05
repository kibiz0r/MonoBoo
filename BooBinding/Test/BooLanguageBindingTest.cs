using System;
using NUnit.Framework;
using MonoDevelop.Projects;
using Moq;
using MonoDevelop.Core;
using System.IO;
using MonoDevelop.Core.Execution;
using NUnit.Framework.Constraints;
using System.Reflection;
using Boo.Lang.CodeDom;

namespace BooBinding.Test
{
    [TestFixture]
    public class BooLanguageBindingTest
    {
        private BooLanguageBinding booLanguageBinding;
        private Mock<IProcessStarter> processStarter;
        private MockFactory mockFactory;
        private const string booCommand = "booc";
        private const string buildCanceled = "Build canceled";
        private const string workingDirectory = "WorkingDirectory";
        private ProjectItemCollection items = new ProjectItemCollection();
        private Mock<DotNetProjectConfiguration> configuration;
        private Mock<ConfigurationSelector> configSelector;
        private Mock<IProgressMonitor> monitor;
        private Mock<TextWriter> monitorLog;

        [SetUp]
        public void SetUp()
        {
            mockFactory = new MockFactory(MockBehavior.Strict);
            processStarter = mockFactory.Create<IProcessStarter>();
            configuration = mockFactory.Create<DotNetProjectConfiguration>();
            configSelector = mockFactory.Create<ConfigurationSelector>();
            monitor = mockFactory.Create<IProgressMonitor>();
            monitorLog = mockFactory.Create<TextWriter>();
            booLanguageBinding = new BooLanguageBinding(processStarter.Object);
        }

        [TearDown]
        public void TearDown()
        {
            mockFactory.VerifyAll();
        }

        [Test]
        public void CompileCanceled()
        {
            configuration.SetupGet(c => c.OutputDirectory).Returns(workingDirectory);
            monitorLog.Setup(ml => ml.WriteLine(buildCanceled));
            monitor.SetupGet(m => m.Log).Returns(monitorLog.Object);
            monitor.Setup(m => m.EndTask()); // TODO: Do we need this?
            monitor.SetupGet(m => m.IsCancelRequested).Returns(true);
            monitor.Setup(m => m.ReportError(buildCanceled, null));
            processStarter.Setup(ps => ps.StartProcess(booCommand, "", workingDirectory, It.IsAny<TextWriter>(), monitorLog.Object)).Returns(new ProcessWrapper());
            
            var buildResult = booLanguageBinding.Compile(items, configuration.Object, configSelector.Object, monitor.Object);
            Assert.That(buildResult, Is.Not.Null);
        }

        [TestCase(@"foo.boo(1,21): BCE0043: Unexpected token: 2.", 21, "BCE0043", "Unexpected token: 2.", "foo.boo", false, 1)]
        [TestCase(@"foo.boo(1,1): BCW0003: WARNING: Unused local variable 'x'.", 1, "BCW0003", "Unused local variable 'x'.", "foo.boo", true, 1)]
        [TestCase(@"Name with spaces.boo(1,6): BCE0018: The name 'foo' does not denote a valid type ('not found').", 6, "BCE0018", "The name 'foo' does not denote a valid type ('not found').", "Name with spaces.boo", false, 1)]
        [TestCase(@"paren(something)(1,1): BCE0005: Unknown identifier: 'blah'.", 1, "BCE0005", "Unknown identifier: 'blah'.", "paren(something)", false, 1)]
        [TestCase(@"paren(0,0)(1,1): BCE0005: Unknown identifier: 'blah'.", 1, "BCE0005", "Unknown identifier: 'blah'.", "paren(0,0)", false, 1)]
        public void ParseOutputLine(string output, int column, string errorNumber, string errorText, string fileName, bool isWarning, int line)
        {
            var error = booLanguageBinding.ParseOutputLine(output);
            Assert.That(error.Column, Is.EqualTo(column));
            Assert.That(error.ErrorNumber, Is.EqualTo(errorNumber));
            Assert.That(error.ErrorText, Is.EqualTo(errorText));
            Assert.That(error.FileName, Is.EqualTo(fileName));
            Assert.That(error.IsWarning, Is.EqualTo(isWarning));
            Assert.That(error.Line, Is.EqualTo(line));
        }

        [Test]
        public void ParseOutputLineBadLine()
        {
            var error = booLanguageBinding.ParseOutputLine("");
            Assert.That(error, Is.Null);
        }

        [Test]
        public void GetSupportedClrVersions()
        {
            var versions = booLanguageBinding.GetSupportedClrVersions();
            Assert.That(versions, Is.EqualTo(new ClrVersion[] { ClrVersion.Net_2_0 }));
        }

        [Test]
        public void GetCodeDomProvider()
        {
            Assert.That(booLanguageBinding.GetCodeDomProvider(), Is.InstanceOf<BooCodeProvider>());
        }

        [Test]
        public void ProjectStockIcon()
        {
            Assert.That(booLanguageBinding.ProjectStockIcon, Is.EqualTo("md-project"));
        }

        [TestCase("foo.boo", Result = true)]
        [TestCase("FOO.BOO", Result = true)]
        [TestCase("foo", Result = false)]
        [TestCase("foo.zip", Result = false)]
        [TestCase("foo.boo.zip", Result = false)]
        public bool IsSourceCodeFile(string fileName)
        {
            return booLanguageBinding.IsSourceCodeFile(fileName);
        }

        [TestCase("foo", Result = "foo.boo")]
        public string GetFileName(string fileNameWithoutExtension)
        {
            return booLanguageBinding.GetFileName(fileNameWithoutExtension);
        }

        [Test]
        public void Language()
        {
            Assert.That(booLanguageBinding.Language, Is.EqualTo("Boo"));
        }
    }
}
