using System;
using NUnit.Framework;
using MonoDevelop.Projects;
using Moq;
using MonoDevelop.Core;
using System.IO;
using MonoDevelop.Core.Execution;
using NUnit.Framework.SyntaxHelpers;

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
            monitor.Setup(m => m.EndTask());
            // TODO: Do we need this?
            monitor.SetupGet(m => m.IsCancelRequested).Returns(true);
            monitor.Setup(m => m.ReportError(buildCanceled, null));
            processStarter.Setup(ps => ps.StartProcess(booCommand, "", workingDirectory, It.IsAny<TextWriter>(), monitorLog.Object)).Returns(new ProcessWrapper());
            
            var buildResult = booLanguageBinding.Compile(items, configuration.Object, configSelector.Object, monitor.Object);
            Assert.That(buildResult, Is.Not.Null);
        }
    }
}
