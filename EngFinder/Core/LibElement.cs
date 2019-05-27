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
            var ee = (BuiltInParameter)valRevitParameter.ElementId.IntegerValue;
            var param = _Doc.GetElement(ee.ToString());
            var it = _Doc.ParameterBindings.ForwardIterator();
            List<string> names = new List<string>();
            while (it.MoveNext())
            {
                names.Add(it.Key.Name);
            }
            var parameter = _Doc.GetElement(valRevitParameter.ElementId);

            LibNumeric insLibNumeric = new LibNumeric();
            if (insLibNumeric.IsDouble(valValue))
            {
                vResult = GetElementValueDouble(valRevitParameter, valCategoryElementId, valValue);
            }
            else
            {
                vResult = GetElementValueIntOrstring(valRevitParameter, valCategoryElementId, valValue);
            }

                
            return vResult;
        }

        private IList<Element> GetElementValueIntOrstring(RevitParameter valRevitParameter, List<ElementId> valCategoryElementId, string valValue)
        {
            IList<Element> vResult = new List<Element>();
            foreach (var vCategoryId in valCategoryElementId)
            {
                IList<ElementFilter> vList = new List<ElementFilter>();
                BuiltInCategory vBuiltInCategory = (BuiltInCategory)vCategoryId.IntegerValue;
                ParameterValueProvider vPovider = new ParameterValueProvider(valRevitParameter.ElementId);
                string vRulestring = valValue;
                FilterStringRuleEvaluator vEvaluator = new FilterStringEquals();
                FilteredElementCollector vCollector = new FilteredElementCollector(_Doc);
                vCollector.OfCategory(vBuiltInCategory);
                FilterRule vRuleRulestring = new FilterStringRule(vPovider, vEvaluator, vRulestring, false);
                ElementParameterFilter vFilterRulestring = new ElementParameterFilter(vRuleRulestring);
                LibNumeric insLibNumeric = new LibNumeric();
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
                if (vElements != null)
                {
                    if (vElements.Count > 0)
                    {
                        vResult = vElements.Concat(vElements).ToList();

                    }

                }
                
            }
            return vResult;
        }

        public IList<Element> GetElementValueDouble(RevitParameter valRevitParameter, List<ElementId> valCategoryElementId, string valValue)
        {
            IList<Element> vResult = new List<Element>();
            foreach (var vCategoryId in valCategoryElementId)
            {
                BuiltInCategory vBuiltInCategory = (BuiltInCategory)vCategoryId.IntegerValue;
                ParameterValueProvider vPovider = new ParameterValueProvider(valRevitParameter.ElementId);
                string vRulestring = valValue;
                FilteredElementCollector vCollector = new FilteredElementCollector(_Doc);
                vCollector.OfCategory(vBuiltInCategory);
                LibNumeric insLibNumeric = new LibNumeric();
                double ruleValDb = 0.0;
                if (insLibNumeric.IsDouble(valValue))
                {
                    Double vNum = 0;
                    Double.TryParse(valValue, out vNum);
                    ruleValDb = vNum;
                }
                ParameterValueProvider pvp = new ParameterValueProvider(valRevitParameter.ElementId);
                FilterNumericEquals fnrv = new FilterNumericEquals();
                var vFilterDoubleRule = new FilterDoubleRule(pvp, fnrv, ruleValDb, 10e-10);
                var epf = new ElementParameterFilter(vFilterDoubleRule);
                vCollector.WherePasses(epf);
                IList<Element> vElements = vCollector.ToElements();
                if (vElements != null)
                {
                    if (vElements.Count > 0)
                    {
                        vResult = vElements.Concat(vElements).ToList();

                    }

                }

            }
            return vResult;
        }


    }
}
