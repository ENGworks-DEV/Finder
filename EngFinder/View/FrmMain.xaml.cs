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

        private ExternalCommandData p_commanddata;
        
        Document _Doc;
        List<RevitParameter> _RevitParameter;
        public ObservableCollection<RevitParameter> RevitParameters { get; set; }
        public ObservableCollection<Category> CategoryList { get; set; }
        List<Category> _Category { get; set; }
        public ObservableCollection<CheckedListItem> CategoryObjects { get; set; }
        public ObservableCollection<Element> ElementList { get; set; }

      
        public FrmMain(Document IntDocument, ExternalCommandData cmddata_p)
        {

            p_commanddata = cmddata_p;

            InitializeComponent();


            UIApplication uiApp = cmddata_p.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            
            _Doc = IntDocument;
            CategoryInitValue();
            this.Topmost = true;

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
            _RevitParameter = _RevitParameter.GroupBy(p => p.Name).Select(x => x.First()).ToList();
            if (valFromWindowLoaded)
            {
                RevitParameters = new ObservableCollection<RevitParameter>(_RevitParameter.OrderBy(x => x.Name));
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
            List<BuiltInCategory> list = new List<BuiltInCategory>();
            List<BuiltInCategory> vBuiltInCats = list;

            vBuiltInCats.Add(BuiltInCategory.OST_ConduitRun);
            vBuiltInCats.Add(BuiltInCategory.OST_Conduit);
            vBuiltInCats.Add(BuiltInCategory.OST_DataDevices);
            vBuiltInCats.Add(BuiltInCategory.OST_ConduitFitting);
            vBuiltInCats.Add(BuiltInCategory.OST_CableTray);
            vBuiltInCats.Add(BuiltInCategory.OST_CableTrayFitting);
            vBuiltInCats.Add(BuiltInCategory.OST_DuctAccessory);
            vBuiltInCats.Add(BuiltInCategory.OST_DuctFitting);
            vBuiltInCats.Add(BuiltInCategory.OST_DuctCurves);
            vBuiltInCats.Add(BuiltInCategory.OST_PipeCurves);
            vBuiltInCats.Add(BuiltInCategory.OST_PipeFitting);
            vBuiltInCats.Add(BuiltInCategory.OST_PipeAccessory);
            vBuiltInCats.Add(BuiltInCategory.OST_FabricationPipework);
            vBuiltInCats.Add(BuiltInCategory.OST_FabricationContainment);
            vBuiltInCats.Add(BuiltInCategory.OST_FabricationDuctworkInsulation);
            vBuiltInCats.Add(BuiltInCategory.OST_FabricationDuctworkRise);
            vBuiltInCats.Add(BuiltInCategory.OST_FabricationDuctwork);


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
                ObservableCollection<RevitParameter> vRevitParameters = new ObservableCollection<RevitParameter>(_RevitParameter
                    .Where(p => p.Name.ToLower()
                    .Contains( TxtSearch.Text.ToLower())));

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
            foreach (var vData in valRevitParameter.OrderBy(x => x.Name))
            {
                RevitParameters.Add(vData);
            }
            
            ListParameter.Items.Refresh();
        }

        public void CategoryCreateCheckBoxList()
        {
            CategoryObjects = new ObservableCollection<CheckedListItem>();
            foreach (var vData in _Category.OrderBy(x=>x.Name))
            {
                CheckedListItem vRecord = new CheckedListItem
                {
                    Id = vData.Id.ToString(),
                    Name = vData.Name,
                    IsChecked = true
                };
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
            if (ListParameter.SelectedItem != null)
                try
                {
                    FilterParameter(txtFilter.Text);
                }
                catch (Exception vEx)
                {
                    TextBlockError.Text = vEx.Message;
                }
            else
                TaskDialog.Show("Alert", "You have not selected any parameter");
        }

        private void ElementListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                foreach (Element vData in ElementListView.SelectedItems)
                {
                    List<ElementId> list = new List<ElementId>();
                    ICollection<ElementId> ids = list;
                    ids.Add(vData.Id);
                    UIDocument uiDoc = new UIDocument(_Doc);
                    uiDoc.Selection.SetElementIds(ids);
                    uiDoc.ShowElements(ids);
                    break;
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
            LibParameters inSLibParameters = new LibParameters(_Doc);
            List<RevitParameter> vParamsList = inSLibParameters.GetFilterByCategory(CategoriesList());
            RevitParameter vPickedElement = (RevitParameter)ListParameter.SelectedItem;
            List<RevitParameter> vCommonParameters = vParamsList.FindAll(p => p.Name.Equals(vPickedElement.Name));
            IList<Element> vResult = new List<Element>();

            foreach (RevitParameter vParam in vCommonParameters) {
                IList<Element> vList = vLibElement.GetBy(vParam, CategoriesList(), valFilter);
                if (null != vList) {
                    vResult = vResult.Concat(vList).ToList();
                }
            }

            ElementList = new ObservableCollection<Element>(vResult);
            ElementListView.ItemsSource = ElementList;
        }

        private void IsolateElements_click(object sender, RoutedEventArgs e)
        {
            UIApplication uiApp = p_commanddata.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;

            if (ElementListView.Items.Count > 0)
                {
                    try
                    {

                        List<ElementId> vIds = new List<ElementId>();
                        foreach (Element vData in ElementListView.Items)
                        {
                            vIds.Add(vData.Id);
                        }

                        _Doc.ActiveView.IsolateElementsTemporary(vIds);
                         uiDoc.RefreshActiveView();
                        //this.Hide();
                        //this.Show();
                        
                    }
                    catch (Exception vEx)
                    {
                        TextBlockError.Text = vEx.Message;
                    } 
                }

        }

        private void HighlightElements_click(object sender, RoutedEventArgs e)
        {
            UIApplication uiApp = p_commanddata.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;



            if (ElementListView.Items.Count > 0)
            {
                    try
                    {
                        OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                        Autodesk.Revit.DB.Color red = new Autodesk.Revit.DB.Color(255, 0, 0);
                        Element solidFill = new FilteredElementCollector(_Doc).OfClass(typeof(FillPatternElement)).Where(q => q.Name.Contains("Solid")).First();

                        ogs.SetProjectionLineColor(red);
                        ogs.SetProjectionLineWeight(8);
                    
                    try
                    {

                        List<ElementId> vIds = new List<ElementId>();
                        foreach (Element vData in ElementListView.Items)
                        {
                            vIds.Add(vData.Id);
                            using (Transaction t = new Transaction(_Doc, "Highlight element"))
                            {
                                try
                                {

                                    _Doc.ActiveView.SetElementOverrides(vData.Id, ogs);

                                }
                                catch (Exception ex)
                                {
                                    TaskDialog.Show("Exception", ex.ToString());
                                }


                                t.Commit();

                            }

                        }
                        _Doc.ActiveView.IsolateElementsTemporary(vIds);
                        _Doc.ActiveView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Exception", ex.ToString());
                    }

                }
                    catch (Exception vEx)
                    {
                        TextBlockError.Text = vEx.Message;
                    }
                    uiDoc.RefreshActiveView();

            }

            }


        private void LbActors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListParameter_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AllIndustry_Click(object sender, RoutedEventArgs e) {
            CategoryObjects.Clear();
            foreach (var vData in _Category.OrderBy(x => x.Name)) {
                CheckedListItem vRecord = new CheckedListItem {
                    Id = vData.Id.ToString(),
                    Name = vData.Name,
                    IsChecked = allIndustry.IsChecked ?? false
                };
                LoadInfo(false);
                CategoryObjects.Add(vRecord);
            }
        }
    }

    public class CheckedListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

}
