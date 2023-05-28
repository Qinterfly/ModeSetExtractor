

using LMSTestLabAutomation;

namespace ModeSetExtractor.Core
{
    using ArrayDictionary = Dictionary<string, Array>;
    using ElementDictionary = Dictionary<ElementType, Dictionary<string, Array>>;
    public enum ElementType { kQuads, kTrias, kLines }

    public class Model
    {
        public Model(IGeometry geometry)
        {
            if (geometry == null)
                return;
            Array X, Y, Z, rotXY, rotXZ, rotYZ;
            Array nodeNamesA, nodeNamesB, nodeNamesC, nodeNamesD;
            Array slaveNodeNames, masterNodeNames1, masterNodeNames2, masterNodeNames3, masterNodeNames4;
            mElementTypes = Enum.GetValues(typeof(ElementType));
            mComponentNames = new List<string>();
            mComponentSet = new ComponentGeometry(mElementTypes);
            mDependencies = new GeometryDependencies();
            Array componentNames = geometry.ComponentNames;
            foreach (string component in componentNames)
            {
                mComponentNames.Add(component);
                Array nodeNames = geometry.ComponentNodeNames[component];
                int numNodes = nodeNames.Length;
                // Component node names
                mComponentSet.nodeNames.Add(component, nodeNames);
                // Nodal positions in the global coordinate system
                geometry.ComponentNodesValues(component, nodeNames, out X, out Y, out Z, out rotXY, out rotXZ, out rotYZ, LocalCoordinates: 0);
                double[,] coordinates = new double[numNodes, 3];
                for (int iNode = 0; iNode != numNodes; ++iNode)
                {
                    coordinates[iNode, 0] = (double)X.GetValue(iNode);
                    coordinates[iNode, 1] = (double)Y.GetValue(iNode);
                    coordinates[iNode, 2] = (double)Z.GetValue(iNode);
                }
                mComponentSet.nodeCoordinates.Add(component, coordinates);
                // Lines 
                geometry.ComponentLines(component, out nodeNamesA, out nodeNamesB);
                int numLines = nodeNamesA.Length;
                string[,] lines = new string[numLines, 2];
                for (int i = 0; i != numLines; ++i)
                {
                    lines[i, 0] = (string)nodeNamesA.GetValue(i);
                    lines[i, 1] = (string)nodeNamesB.GetValue(i);
                }
                mComponentSet.elementData[ElementType.kLines].Add(component, lines);
                // Triangles
                geometry.ComponentTrias(component, out nodeNamesA, out nodeNamesB, out nodeNamesC);
                int numTrias = nodeNamesA.Length;
                string[,] trias = new string[numTrias, 3];
                for (int i = 0; i != numTrias; ++i)
                {
                    trias[i, 0] = (string)nodeNamesA.GetValue(i);
                    trias[i, 1] = (string)nodeNamesB.GetValue(i);
                    trias[i, 2] = (string)nodeNamesC.GetValue(i);
                }
                mComponentSet.elementData[ElementType.kTrias].Add(component, trias);
                // Quads
                geometry.ComponentQuads(component, out nodeNamesA, out nodeNamesB, out nodeNamesC, out nodeNamesD);
                int numQuads = nodeNamesA.Length;
                string[,] quads = new string[numQuads, 4];
                for (int i = 0; i != numQuads; ++i)
                {
                    quads[i, 0] = (string)nodeNamesA.GetValue(i);
                    quads[i, 1] = (string)nodeNamesB.GetValue(i);
                    quads[i, 2] = (string)nodeNamesC.GetValue(i);
                    quads[i, 3] = (string)nodeNamesD.GetValue(i);
                }
                mComponentSet.elementData[ElementType.kQuads].Add(component, quads);
            }
            // Slaves
            geometry.Slaves(out slaveNodeNames, out masterNodeNames1, out masterNodeNames2, out masterNodeNames3, out masterNodeNames4, out X, out Y, out Z);
            int numSlaves = masterNodeNames1.Length;
            mDependencies.slaveNodeNames = new string[numSlaves];
            mDependencies.masterNodeNames = new string[numSlaves, 4];
            mDependencies.dirFlags = new int[numSlaves, 3];
            for (int iSlave = 0; iSlave != numSlaves; ++iSlave)
            {
                // Dependent nodes
                mDependencies.slaveNodeNames[iSlave] = (string)slaveNodeNames.GetValue(iSlave);
                // Leading nodes
                mDependencies.masterNodeNames[iSlave, 0] = (string)masterNodeNames1.GetValue(iSlave);
                mDependencies.masterNodeNames[iSlave, 1] = (string)masterNodeNames2.GetValue(iSlave);
                mDependencies.masterNodeNames[iSlave, 2] = (string)masterNodeNames3.GetValue(iSlave);
                mDependencies.masterNodeNames[iSlave, 3] = (string)masterNodeNames4.GetValue(iSlave);
                // Directional flags
                mDependencies.dirFlags[iSlave, 0] = (int)X.GetValue(iSlave);
                mDependencies.dirFlags[iSlave, 1] = (int)Y.GetValue(iSlave);
                mDependencies.dirFlags[iSlave, 2] = (int)Z.GetValue(iSlave);
            }
        }
        private List<string> mComponentNames;
        private ComponentGeometry mComponentSet;
        private GeometryDependencies mDependencies;
        private Array mElementTypes;
    }

    public class GeometryDependencies
    {
        public string[] slaveNodeNames;
        public string[,] masterNodeNames;
        public int[,] dirFlags;
    }

    public class ComponentGeometry
    {
        public ComponentGeometry(Array elementTypes)
        {
            nodeNames = new ArrayDictionary();
            nodeCoordinates = new ArrayDictionary();
            elementData = new ElementDictionary();
            foreach (ElementType type in elementTypes)
                elementData.Add(type, new Dictionary<string, Array>());
        }
        public ArrayDictionary nodeNames;
        public ArrayDictionary nodeCoordinates;
        public ElementDictionary elementData;
    }
}
