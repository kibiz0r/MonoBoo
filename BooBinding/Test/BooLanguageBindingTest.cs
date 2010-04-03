using System;
using NUnit.Framework;
using MonoDevelop.Projects;
using Moq;
using MonoDevelop.Core;

namespace BooBinding.Test
{
    [TestFixture()]
    public class BooLanguageBindingTest
    {
        private BooLanguageBinding booLanguageBinding;

        [SetUp]
        public void SetUp()
        {
            booLanguageBinding = new BooLanguageBinding();
        }

        [Test()]
        public void CompileCanceled()
        {
            var items = new ProjectItemCollection();
            var configuration = new Mock<DotNetProjectConfiguration>(MockBehavior.Strict);
            var configSelector = new Mock<ConfigurationSelector>(MockBehavior.Strict);
            var monitor = new Mock<IProgressMonitor>(MockBehavior.Strict);
            monitor.SetupGet(m => m.IsCancelRequested).Returns(true);
            monitor.Setup(m => m.ReportError("Build canceled", null));
            var buildResult = booLanguageBinding.Compile(items, configuration.Object, configSelector.Object, monitor.Object);
            monitor.VerifyAll();
        }
    }
}
