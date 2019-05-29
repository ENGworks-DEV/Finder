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
            bool vIsParse = false;
            double vData = 0;
            string vValueToString = valValue;
            if (IsParsed(valValue, out vData))
            {
                valValue = vData.ToString();
                vIsParse = true;
            }
            LibNumeric insLibNumeric = new LibNumeric();
            if (insLibNumeric.IsDouble(valValue))
            {
                vResult = GetElementValueDouble(valRevitParameter, valCategoryElementId, valValue);
                if (vResult.Count == 0)
                {
                    if (vIsParse)
                    {
                        vResult = GetElementValueDoubleLessOrEqual(valRevitParameter, valCategoryElementId, valValue, vValueToString);

                        if (vResult.Count == 0)
                        {
                            vResult = GetElementValueDoubleGreaterOrEqual(valRevitParameter, valCategoryElementId, valValue, vValueToString);
                        }
                    }
                }
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


        public IList<Element> GetElementValueDoubleLessOrEqual(RevitParameter valRevitParameter, List<ElementId> valCategoryElementId, string valValue, string valValueToString)
        {
            IList<Element> vResult = new List<Element>();
            IList<Element> vResultTemp = new List<Element>();
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
                    ruleValDb = vNum + 0.001;
                }
                ParameterValueProvider pvp = new ParameterValueProvider(valRevitParameter.ElementId);
                FilterNumericLessOrEqual fnrv = new FilterNumericLessOrEqual();
                var vFilterDoubleRule = new FilterDoubleRule(pvp, fnrv, ruleValDb, 10e-10);
                var epf = new ElementParameterFilter(vFilterDoubleRule);
                vCollector.WherePasses(epf);
                IList<Element> vElements = vCollector.ToElements();
                if (vElements != null)
                {
                    if (vElements.Count > 0)
                    {
                        vResultTemp = vElements.Concat(vElements).ToList();

                    }
                }
            }
            vResult = GetElementValueToString(valRevitParameter, vResultTemp, valValueToString, valValue);
            return vResult;
        }


        public IList<Element> GetElementValueDoubleGreaterOrEqual(RevitParameter valRevitParameter, List<ElementId> valCategoryElementId, string valValue, string valValueToString)
        {
            IList<Element> vResult = new List<Element>();
            IList<Element> vResultTemp = new List<Element>();
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
                    ruleValDb = vNum + 0.001;
                }
                ParameterValueProvider pvp = new ParameterValueProvider(valRevitParameter.ElementId);
                FilterNumericGreaterOrEqual fnrv = new FilterNumericGreaterOrEqual();
                var vFilterDoubleRule = new FilterDoubleRule(pvp, fnrv, ruleValDb, 10e-10);
                var epf = new ElementParameterFilter(vFilterDoubleRule);
                vCollector.WherePasses(epf);
                IList<Element> vElements = vCollector.ToElements();
                if (vElements != null)
                {
                    if (vElements.Count > 0)
                    {
                        vResultTemp = vElements.Concat(vElements).ToList();
                    }
                }
            }
            vResult = GetElementValueToString(valRevitParameter, vResultTemp, valValueToString, valValue);
            return vResult;
        }

        public IList<Element> GetElementValueToString(RevitParameter valRevitParameter, IList<Element> valElements, string valValueToString, string valValue)
        {
            IList<Element> vResult = new List<Element>();
            foreach (var vElement in valElements)
            {
                LibParameters insLibParameters = new LibParameters(_Doc);
                var vParameter = vElement.get_Parameter((BuiltInParameter)valRevitParameter.ElementId.IntegerValue);
                var demo = vParameter.AsValueString();
                var vDiferen = Math.Round(vParameter.AsDouble(), 2) - Math.Round(Convert.ToDouble(valValue), 2);
                if (vDiferen < 0)
                {
                    vDiferen = vDiferen * -1;

                }
                if (vDiferen < 0.01)
                {
                    vResult.Add(vElement);
                }
            }
            return vResult;
        }

        bool IsParsed(string valParse, out double OutParse)
        {
            Units vUnits = _Doc.GetUnits();
            bool vResult = UnitFormatUtils.TryParse(vUnits, UnitType.UT_Length, valParse, out OutParse);
            return vResult;
        }


    }
}
