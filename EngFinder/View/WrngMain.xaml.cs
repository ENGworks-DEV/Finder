using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using EngFinder.Core;
using EngFinder.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EngFinder.View
{
   
    /// <summary>
    /// Interaction logic for FrmMain.xaml
    /// </summary>
    public partial class WrngMain : Window
    {

        public string warningString;
        public string WarningString
        {
            get { return warningString; }
            set { warningString = value; }
        }


        public WrngMain(string input)
        {
            warningString = input;
            this.DataContext = this;
            InitializeComponent();
            this.Topmost = true;
           

        }

       

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


    }

   

}
