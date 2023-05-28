
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
                    Console.WriteLine("Path to a project is not specified");
                    Console.ReadLine();
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
            outputDir   = Path.GetFullPath(outputDir);

            // Open a project
            Console.WriteLine("Opening a project...");
            Project project = new Project(pathProject);

            // Retrieve the selected modesets
            Console.WriteLine("Retrieving selected modesets...");
            List<Core.ModeSet> modeSets = project.retrieveSelectedModeSets();

            // Retrieve the model
            Console.WriteLine("Obtaining geometry...");
            Core.Model model = project.retrieveModel();

            // Writing the model and modesets
            Console.WriteLine("Writing geometry and modesets...");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            Core.IO.writeModeSets(Path.Combine(outputDir, Core.IOConstants.kModeSetsFileName), modeSets);
            Core.IO.writeModel(Path.Combine(outputDir, Core.IOConstants.kModelFileName), model);

            return 0;
        }
    }
}