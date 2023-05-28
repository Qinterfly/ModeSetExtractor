

using System.Globalization;

namespace ModeSetExtractor.Core
{
    public class ModeSet
    {
        // Construct a modeset from a file
        public ModeSet(string path, string name)
        {
            const char delimiter = '"';
            Name = name;
            ModeShape = new Dictionary<string, double>();
            StreamReader stream = IO.openFileForReading(path, IOConstants.kMaxAttemptAccess);
            string tString;
            string[] lineData;
            // Retrieve the frequency
            while (!stream.EndOfStream)
            {
                tString = stream.ReadLine();
                if (tString.Contains("FREQUENCY"))
                {
                    lineData = tString.Split(delimiter);
                    Frequency = double.Parse(lineData[1], CultureInfo.InvariantCulture);
                    break;
                }
            }
            // Determine where the modeshape starts
            while (!stream.EndOfStream)
            {
                tString = stream.ReadLine();
                if (tString.Contains("BEGIN") && tString.Contains("VECTOR"))
                    break;
            }
            // Read the modeshape
            while (!stream.EndOfStream)
            {
                tString = stream.ReadLine();
                if (tString.Contains("END") && tString.Contains("VECTOR"))
                {
                    break;
                }
                lineData = tString.Split(delimiter);
                double imagPart = double.Parse(lineData[2], CultureInfo.InvariantCulture);
                ModeShape.Add(lineData[0], imagPart);
            }
            stream.Close();
        }

        public readonly string Name;
        public readonly double Frequency;
        public readonly Dictionary<string, double> ModeShape;
    }
}
