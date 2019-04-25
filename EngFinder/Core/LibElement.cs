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
                IList<ElementFilter> vList = new List<ElementFilter>();
                BuiltInCategory vBuiltInCategory = (BuiltInCategory)vCategoryId.IntegerValue;
                ParameterValueProvider vPovider  = new ParameterValueProvider(valRevitParameter.ElementId);
                string vRulestring = valValue;
                FilterStringRuleEvaluator vEvaluator  = new FilterStringEquals();
                FilteredElementCollector vCollector = new FilteredElementCollector(_Doc);
                vCollector.OfCategory(vBuiltInCategory);
                FilterRule vRuleRulestring = new FilterStringRule( vPovider, vEvaluator, vRulestring, false);
                ElementParameterFilter vFilterRulestring = new ElementParameterFilter(vRuleRulestring);
                LibNumeric insLibNumeric = new LibNumeric();
                if (insLibNumeric.IsDouble(valValue))
                {
                    Double vNum = 0;
                    Double.TryParse(valValue, out vNum);
                    FilterNumericGreater vEvaluatorNumeri = new FilterNumericGreater();
                    double vRuleDoubleVal = vNum;
                    const double eps = 10E-06;
                    var vFilterDoubleRule = new FilterDoubleRule(vPovider, vEvaluatorNumeri, vRuleDoubleVal, eps);
                    var vElementParameterFilterDoubleRule = new ElementParameterFilter(vFilterDoubleRule);
                    vList.Add(vElementParameterFilterDoubleRule);
                }

                if (insLibNumeric.IsInt(valValue))
                {
                    int vNum = 0;
                    int.TryParse(valValue, out vNum);
                    FilterNumericEquals vEvaluatorNumeri = new FilterNumericEquals();
                    
                    int vRuleIntVal = vNum;
                    var vFilterIntRule = new FilterIntegerRule(vPovider, vEvaluatorNumeri, vRuleIntVal);
                    var vElementParameterFilterIntRule = new ElementParameterFilter(vFilterIntRule);
                    vList.Add(vElementParameterFilterIntRule);
                }
                vList.Add(vFilterRulestring);
                LogicalOrFilter vLogicalOrFilter = new LogicalOrFilter(vList);
                vCollector.WherePasses(vLogicalOrFilter);
                IList<Element> vElements = vCollector.ToElements();
                vResult= vElements.Concat(vResult).ToList();
            }
            return vResult;
        }


    }
}
