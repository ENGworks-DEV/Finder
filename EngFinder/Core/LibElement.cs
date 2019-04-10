using Autodesk.Revit.DB;
using EngFinder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngFinder.Core
{
    public class LibElement
    {
        Document _Doc;
        public LibElement(Document IntDocument)
        {
            _Doc = IntDocument;
        }
       public IList<Element> GetBy(RevitParameter valRevitParameter, List<ElementId> valCategoryElementId, string valValue)
        {
            IList<Element> vResult = new List<Element>();
            ElementMulticategoryFilter vFilter = new ElementMulticategoryFilter(valCategoryElementId);
            FilteredElementCollector vFilteredElementCollector = new FilteredElementCollector(_Doc);
            vFilteredElementCollector.WherePasses(vFilter);
            var vElements = vFilteredElementCollector.ToElements();
            LibParameters vLibParameters = new LibParameters(_Doc);
            foreach(var vData  in vElements.AsParallel())
            {
                
                if (vLibParameters.Contains(valValue, valRevitParameter, vData))
                {
                    vResult.Add(vData);
                }
            }
            return vResult;
        }
    }
}
