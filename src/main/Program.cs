
using ModesetExtractor.Core;

namespace ModeSetExtractor
{
    public class App
    {
        public static int Main(string[] args)
        {
            string pathProject = null;
            string outputDir   = null;
            // Parse the arguments to determine the project path and output directory
            int numArgs = args.Length;
            switch (numArgs)
            {
                case 0:
                    Console.WriteLine("The project path is not specified");
                    return 1;
                case 1:
                    pathProject = args[0];
                    outputDir   = Core.IOConstants.kOutputDir;
                    break;
                case 2:
                    pathProject = args[0];
                    outputDir   = args[1];
                    break;
            }
            pathProject = Path.GetFullPath(pathProject);
            // Open a project
            Project project = new Project(pathProject);
            // Retrieve the selected modesets
            List<Core.ModeSet> modeSets = project.retrieveSelectedModeSets();
            // Retrieve the model
            Core.Model model = project.retrieveModel();
            return 0;
        }
    }
}