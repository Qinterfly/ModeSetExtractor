
namespace ModeSetExtractor.Core
{
    static class IOConstants
    {
        public const int kMaxAttemptAccess     = 5000;
        public static string kModeSetFileName  = "modeSet.afm";
        public static string kModelFileName    = "model.txt";
        public static string kModeSetsFileName = "modesets.txt";
        public static string kOutputDir        = "output";
    }

    static public class IO
    {
        // Open a file for reading
        static public StreamReader openFileForReading(string path, int attemptAccess)
        {
            StreamReader stream = null;
            while (--attemptAccess > 0)
            {
                try
                {
                    stream = new StreamReader(path);
                    break;
                }
                catch
                {

                }
            }
            return stream;
        }
    }
}
