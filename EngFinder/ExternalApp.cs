using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.IO;

namespace ExternalApp
{
    class CsAddpanel : Autodesk.Revit.UI.IExternalApplication
    {
        // Generate an Guid for the App
        static AddInId m_appId = new AddInId(new Guid(
        "3c13b613-6ec9-4c78-9ff3-cecfb9e52f1b"));

        // Get the absolute path of this assembly.
        static string ExecutingAssemblyPath = System.Reflection.Assembly
          .GetExecutingAssembly().Location;


        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream,
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            // add new ribbon panel 
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Value Finder");

            // Create a push button in the ribbon panel "NewRibbonPanel". 
            // the add-in application "HelloWorld" will be triggered when button is pushed. 
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("Value Finder",
                "Value Finder", ExecutingAssemblyPath, "EngFinder.Command")) as PushButton;

            pushButton.ToolTip = "Value Finder";

            pushButton.LongDescription =
             "Search in your model elements with a specific parameter and value. You can filter any category " +
             "that you want, then any parameter and finally select the value that you are looking for.";

            // Set the large image shown on button.
            pushButton.LargeImage = PngImageSource("EngFinder.View.FinderLogo.png");


            // Context (F1) Help - new in 2013 
            //string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // %AppData% 

            string path;
            path = System.IO.Path.GetDirectoryName(
               System.Reflection.Assembly.GetExecutingAssembly().Location);

            string newpath = Path.GetFullPath(Path.Combine(path, @"..\"));

            ContextualHelp contextHelp = new ContextualHelp(
                ContextualHelpType.Url,
                "https://engworks.com/values-finder/");

            pushButton.SetContextualHelp(contextHelp);


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

}