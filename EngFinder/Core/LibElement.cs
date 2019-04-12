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

            foreach( var vCategoryId in valCategoryElementId)
            {
                BuiltInCategory vBuiltInCategory = (BuiltInCategory)vCategoryId.IntegerValue;
                ParameterValueProvider vPovider  = new ParameterValueProvider(valRevitParameter.ElementId);
                string vRulestring = valValue;
                FilterStringRuleEvaluator vEvaluator  = new FilterStringEquals();
                FilteredElementCollector vCollector = new FilteredElementCollector(_Doc);
                vCollector.OfCategory(vBuiltInCategory);
                FilterRule vRule = new FilterStringRule( vPovider, vEvaluator, vRulestring, false);
                ElementParameterFilter vFilter   = new ElementParameterFilter(vRule);
                vCollector.WherePasses(vFilter);
                IList<Element> vElements = vCollector.ToElements();
                vResult= vElements.Concat(vResult).ToList();
            }
            return vResult;
        }
    }
}
