
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TestsCore
    {
        [TestMethod]
        public void runMain()
        {
            string[] args = { mProjectPath, mOutputDir };
            ModeSetExtractor.App.Main(args);
        }

        private string mProjectPath = "../../../../examples/W70.lms";
        private string mOutputDir   = "output";
    }
}