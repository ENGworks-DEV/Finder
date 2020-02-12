#region Namespaces
using System;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Selection;
using EngFinder.View;
#endregion


namespace EngFinder
{
    class App : IExternalApplication
    {

        static Type myType = typeof(App);
        static string nameSpaceNm = myType.Namespace;

        public static string NameSpaceNm

        {

            get { return nameSpaceNm; }

            set { nameSpaceNm = value; }

        }

        public Result OnStartup(UIControlledApplication application)
        {


            // Get the absolut path of this assembly
            string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly(
               ).Location;

            // Create a ribbon panel
            RibbonPanel m_projectPanel = application.CreateRibbonPanel(
                "Value Finder");

            //Execute File location
            string fileLctn = NameSpaceNm + ".MainCommand";

            //Button
            PushButton pushButton = m_projectPanel.AddItem(new PushButtonData(
                    "Value Finder", "Value Finder", ExecutingAssemblyPath,
                    fileLctn)) as PushButton;

            //Add Help ToolTip 
            pushButton.ToolTip = NameSpaceNm;

            //Add long description 
            pushButton.LongDescription =
             "Search in your model elements with a specific parameter and value. You can filter any category " +
                 "that you want, then any parameter and finally select the value that you are looking for.";

            //Icon file location
            string iconFlLctn = NameSpaceNm + ".Resources.Icon.png";

            // Set the large image shown on button.
            pushButton.LargeImage = PngImageSource(
                iconFlLctn);

            // Get the location of the solution DLL
            string path = System.IO.Path.GetDirectoryName(
               System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Combine path with \
            string newpath = Path.GetFullPath(Path.Combine(path, @"..\"));


            ContextualHelp contextHelp = new ContextualHelp(
                ContextualHelpType.Url,
                "https://engworks.com/values-finder/");

            // Assign contextual help to pushbutton
            pushButton.SetContextualHelp(contextHelp);

            return Result.Succeeded;

        }

        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            // Get Bitmap from Resources folder
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream,
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class MainCommand : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Selection sel = uidoc.Selection;

            // Retrieve elements from database

            FrmMain vInsFrmMain = new FrmMain(doc, commandData);

            vInsFrmMain.Show();
            return Result.Succeeded;
        }
    }
}




