
using System.Xml.Linq;

namespace ModeSetExtractor.Core
{
    static class IOConstants
    {
        public const int kMaxAttemptAccess     = 5000;
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

        static public void writeModeSets(string path, List<ModeSet> modeSets)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                stream.WriteLine(modeSets.Count);
                foreach (ModeSet modeSet in modeSets)
                {
                    stream.WriteLine("\t{0:f2}\t{1:d}", modeSet.Frequency, modeSet.ModeShape.Count);
                    foreach (var item in modeSet.ModeShape)
                        stream.WriteLine("{0}\t{1:g}", item.Key, item.Value);
                }
            }
        }

        static public void writeModel(string path, Model model)
        {
            int kNumDirections = 3;

            // Count number of entities of each type
            int numTotalNodes = 0;
            Dictionary<ElementType, int[]> counterElements = new Dictionary<ElementType, int[]>();
            foreach (string component in model.ComponentNames)
            {
                numTotalNodes += model.ComponentSet.nodeNames[component].Length;
                foreach (ElementType type in model.ElementTypes)
                {
                    if (!counterElements.ContainsKey(type))
                    {
                        counterElements.Add(type, new int[2]);
                        counterElements[type][1] = model.ComponentSet.elementData[type][component].GetLength(1);
                    }
                    counterElements[type][0] += model.ComponentSet.elementData[type][component].GetLength(0);
                }
            }

            // Write data of entities
            using (StreamWriter stream = new StreamWriter(path))
            {
                // Nodes
                stream.WriteLine($"{numTotalNodes}\t{kNumDirections}");
                foreach (string component in model.ComponentNames)
                {
                    Array names = model.ComponentSet.nodeNames[component];
                    Array coordinates = model.ComponentSet.nodeCoordinates[component];
                    int numComponentNodes = names.Length;
                    for (int iNode = 0; iNode != numComponentNodes; ++iNode)
                    {
                        stream.Write($"{component}:{names.GetValue(iNode)}");
                        for (int jDir = 0; jDir != kNumDirections; ++jDir)
                            stream.Write("\t{0:f4}", coordinates.GetValue(iNode, jDir));
                        stream.Write("\n");
                    }
                }

                // Elements
                foreach (ElementType type in model.ElementTypes)
                {
                    int numTotalElements = counterElements[type][0];
                    int numColumns = counterElements[type][1];
                    stream.WriteLine($"{numTotalElements}\t{numColumns}");
                    foreach (string component in model.ComponentNames)
                    {
                        Array elements = model.ComponentSet.elementData[type][component];
                        int numRows = elements.GetLength(0);
                        
                        for (int i = 0; i != numRows; ++i)
                        {
                            for (int j = 0; j != numColumns; ++j)
                            {
                                if (j > 0)
                                    stream.Write("\t");
                                stream.Write($"{component}:{elements.GetValue(i, j)}");
                            }
                            stream.Write("\n");
                        }
                    }
                }

                // Slaves
                int numSlaves = model.Dependencies.slaveNodeNames.Length;
                int numMasters = model.Dependencies.masterNodeNames.GetLength(1);
                stream.WriteLine(numSlaves);
                for (int iSlave = 0; iSlave != numSlaves; ++iSlave)
                {
                    stream.Write(model.Dependencies.slaveNodeNames[iSlave]);
                    for (int jMaster = 0; jMaster != numMasters; ++jMaster)
                    {
                        string master = model.Dependencies.masterNodeNames[iSlave, jMaster];
                        if (master == null)
                            master = "-";
                        stream.Write($"\t{master}");
                    }
                    for (int jDir = 0; jDir != kNumDirections; ++jDir)
                        stream.Write($"\t{model.Dependencies.dirFlags[iSlave, jDir]}");
                    stream.Write("\n");
                }
            }
        }
    }
}
