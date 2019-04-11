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
    public partial class FrmMain : Window
    {
       
        Document _Doc;
        List<RevitParameter> _RevitParameter;
        public ObservableCollection<RevitParameter> RevitParameters { get; set; }
        public ObservableCollection<Category> CategoryList { get; set; }
        List<Category> _Category { get; set; }
        public ObservableCollection<CheckedListItem> CategoryObjects { get; set; }
        public ObservableCollection<Element> ElementList { get; set; }

      
        public FrmMain(Document IntDocument)
        {
            InitializeComponent();
            _Doc = IntDocument;
            CategoryInitValue();
           
        }
        private void ListParameter_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CategoryCreateCheckBoxList();
            LoadInfo(true);
            this.DataContext = this;
            
        }
        void LoadInfo(bool valFromWindowLoaded)
        {
            LibParameters inSLibParameters = new LibParameters(_Doc);
            List<ElementId> vCatList = CategoriesList();
            _RevitParameter = inSLibParameters.GetFilterByCategory(vCatList);
            if (valFromWindowLoaded)
            {
                RevitParameters = new ObservableCollection<RevitParameter>(_RevitParameter);
            }
            else
            {
                ListParameterRefresh(new ObservableCollection<RevitParameter>(_RevitParameter));
            }
        }

        List<ElementId> CategoriesList()
        {
            List<ElementId> vResult = new List<ElementId>();
            foreach (var vData in _Category)
            {
               if( CategoryObjects.Where(p=>p.Id == vData.Id.ToString() && p.IsChecked ==true).Count() > 0)
                {
                    vResult.Add(vData.Id);
                }
            }
            return vResult;
        }

        void CategoryInitValue()
        {
            List<ElementId> vResult = new List<ElementId>();
            List<BuiltInCategory> vBuiltInCats = new List<BuiltInCategory>();
            vBuiltInCats.Add(BuiltInCategory.OST_Doors);
            vBuiltInCats.Add(BuiltInCategory.OST_Rooms);
            vBuiltInCats.Add(BuiltInCategory.OST_Windows);
            vBuiltInCats.Add(BuiltInCategory.OST_DataDevices);
            vBuiltInCats.Add(BuiltInCategory.OST_ConduitFitting);
            _Category = new List<Category>();
            foreach (var vData in vBuiltInCats)
            {
                Category vRecord = _Doc.Settings.Categories.get_Item(vData);
            

                if (vRecord != null)
                {
                    _Category.Add(vRecord);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                ObservableCollection<RevitParameter> vRevitParameters = new ObservableCollection<RevitParameter>(_RevitParameter.Where(p => p.Name.Contains(TxtSearch.Text)));
                ListParameterRefresh(vRevitParameters);
            }
            catch (Exception vEx)
            {
                TextBlockError.Text = vEx.Message;
            }

        }

        private void ListParameterRefresh(ObservableCollection<RevitParameter> valRevitParameter)
        {
            RevitParameters.Clear();
            foreach (var vData in valRevitParameter)
            {
                RevitParameters.Add(vData);
            }

            ListParameter.Items.Refresh();
        }

        public void CategoryCreateCheckBoxList()
        {
            CategoryObjects = new ObservableCollection<CheckedListItem>();
            foreach (var vData in _Category)
            {
                CheckedListItem vRecord = new CheckedListItem();
                vRecord.Id = vData.Id.ToString();
                vRecord.Name = vData.Name;
                vRecord.IsChecked = true;
                CategoryObjects.Add(vRecord);
            }

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var vData in CategoryObjects)
                {
                    LoadInfo(false);
                }

            }
            catch (Exception vEx)
            {
                TextBlockError.Text = vEx.Message;
            }
        }




        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FilterParameter(txtFilter.Text);
            }
            catch (Exception vEx)
            {
                TextBlockError.Text = vEx.Message;
            }

          
        }

        private void ElementListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                foreach (Element vData in ElementListView.SelectedItems)
                {
                    ICollection<ElementId> ids = new List<ElementId>();
                    ids.Add(vData.Id);
                    UIDocument uiDoc = new UIDocument(_Doc);
                    uiDoc.Selection.SetElementIds(ids);
                    uiDoc.ShowElements(ids);
                }
            }
            catch (Exception vEx)
            {
                TextBlockError.Text = vEx.Message;
            }
        }

        private void ListParameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                FilterParameter("");

            }
            catch (Exception vEx)
            {
                TextBlockError.Text = vEx.Message;
            }

        }

        void FilterParameter(string valFilter)
        {
            LibElement vLibElement = new LibElement(_Doc);
            IList<Element> vList = vLibElement.GetBy((RevitParameter)ListParameter.SelectedItem, CategoriesList(), valFilter);
            ElementList = new ObservableCollection<Element>(vList);
            ElementListView.ItemsSource = ElementList;
        }
    }

    public class CheckedListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

}
