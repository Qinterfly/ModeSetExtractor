
using LMSTestLabAutomation;
using ModeSetExtractor.Core;

namespace ModesetExtractor.Core
{
    public static class LMSConstants
    {
        public static int kDepthSearch    = 1000;
        public static char kPathDelimiter = '/';
    }

    public class Project
    {
        public Project(in string pathFile)
        {
            try
            {
                mApp = new Application();
                if (mApp.Name == "")
                    mApp.Init("-w DesktopStandard ");
                mApp.OpenProject(pathFile);
            }
            catch (System.Runtime.InteropServices.COMException exc)
            {
                throw exc;
            }

            mPath       = pathFile;
            mDatabase   = mApp.ActiveBook.Database();
            mGeometry   = (IGeometry) mDatabase.GetItem("Geometry");
            mUnitSystem = mApp.UnitSystem;
        }

        public bool isOpened() { return mApp != null; }

        public List<ModeSet> retrieveSelectedModeSets()
        {
            List<ModeSet> modeSets = new List<ModeSet>();
            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\";
            string pathFile = systemPath + IOConstants.kModeSetFileName;
            try
            {
                DataWatch dataWatch = mApp.ActiveBook.FindDataWatch("Navigator_SelectedOIDs");
                IData dataSelected = dataWatch.Data;
                if (dataSelected == null)
                    return modeSets;
                AttributeMap attributeMap = dataSelected.AttributeMap;
                int numSelected = attributeMap.Count;
                string tString;
                for (int iSelected = 0; iSelected != numSelected; ++iSelected)
                {
                    DataWatch blockWatch = mApp.FindDataWatch(attributeMap[iSelected]);
                    // Check if the folder is selected
                    if (blockWatch.Data != null)
                        continue;
                    // Retrieving the folder and section
                    IData dataOID = attributeMap[iSelected].AttributeMap["OID"];
                    string selectedPath = dataOID.AttributeMap["Path"].AttributeMap["PathString"];
                    string[] info = selectedPath.Split('\\');
                    string selectedSectionName = info[0];
                    string selectedFolderName  = info[1];
                    // Search for a modeset
                    string tPath = selectedSectionName + LMSConstants.kPathDelimiter + selectedFolderName;
                    AttributeMap tMap = mDatabase.ElementNames[tPath, LMSConstants.kDepthSearch].KeyNames;
                    tPath += LMSConstants.kPathDelimiter;
                    foreach (string entity in tMap)
                    {
                        tString = tPath + entity;
                        string type = mDatabase.ElementType[tString];
                        // Build up the modeset instance
                        if (type.Equals("ModeSet") || type.Equals("ODSModeSet"))
                        {
                            exportModeSet(type, tString, pathFile);
                            modeSets.Add(new ModeSet(pathFile, selectedFolderName));
                            break;
                        }
                    }
                }
                File.Delete(pathFile);
            }
            catch 
            {

            }
            return modeSets;
        }

        public Model retrieveModel()
        {
            return new Model(mGeometry);
        }

        private void exportModeSet(string type, string pathModeSet, string pathOutputFile)
        {
            try
            {
                IData modeSet = mDatabase.GetItem(pathModeSet);
                AttributeMap map = mApp.CreateAttributeMap();
                map.Add(type, modeSet);
                map.Add("AFMFileName", pathOutputFile);
                string command;
                if (type.Equals("ODSModeSet"))
                    command = "LmsHq::ActiveCompVC::DataExplorerCmd::CExportODSToAFM";
                else
                    command = "LmsHq::ActiveCompVC::DataExplorerCmd::CExportToAFM";
                IData data = mApp.CreateObject(command, map);
                DataWatch port = mApp.ActiveBook.FindDataWatch("Automation\\/DataExplorerCommand");
                port.Data = data;
            }
            catch (Exception exc)
            {
                throw new Exception("Error occured while writing a modeset to a file", exc);
            }
        }

        private string mPath;
        private Application mApp;
        private IDatabase mDatabase;
        private IGeometry mGeometry;
        private IUnitSystem mUnitSystem;
    }
}
