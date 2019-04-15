using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using EngFinder.Model;
 

namespace EngFinder.Core
{
    public class LibParameters
    {
        Document _Doc;
        public LibParameters(Document IntDocument) {
            _Doc = IntDocument;
        }

        public List<RevitParameter> GetFilterByCategory(List<ElementId> valValue)
        {
            List<RevitParameter> vResult = new List<RevitParameter>();
            foreach (var vData in valValue.AsParallel())
            {
                List<ElementId> vFilter = new List<ElementId>();
                vFilter.Add(vData);
                var vList = ParameterFilterUtilities.GetFilterableParametersInCommon(_Doc, vFilter);
                //Category vCategory = _Doc.Settings.Categories.get_Item((BuiltInCategory)vData.IntegerValue);
                foreach (ElementId vElementId in vList.AsParallel())
                {
                    RevitParameter vRecord = new RevitParameter();
                    string vName = string.Empty;
                    if (vResult.Where(p => p.Id == vElementId.IntegerValue).Count() == 0)
                    {

                        if (vElementId.IntegerValue < 0)
                        {
                            vName = LabelUtils.GetLabelFor((BuiltInParameter)vElementId.IntegerValue) + " - " + vElementId.ToString();
                        }
                        else
                        {
                            vName = _Doc.GetElement(vElementId).Name + " - " + vElementId.ToString();
                        }

                        vRecord.Id = vElementId.IntegerValue;
                        vRecord.ElementId = vElementId;
                        vRecord.Name = vName;
                        vResult.Add(vRecord);
                    }


                   
                }
            }
            return vResult;
        }

        public  List<RevitParameterInfocs> GetParametersBy(Element valElement)
        {
            List<RevitParameterInfocs> vResult =
                (from Parameter p in valElement.Parameters
                 select new RevitParameterInfocs
                 {
                     Name = p.Definition.Name,
                     Value = p.AsValueString(),
                     Group = p.Definition.ParameterGroup,
                     Type = p.Definition.ParameterType,
                     Storage = p.StorageType,
                     Shared = p.IsShared,
                     ReadOnly = p.IsReadOnly,
                     ID = p.IsShared ? p.GUID.ToString() :
                     (p.Definition as InternalDefinition).Id.ToString()
                 }).ToList();
            return vResult;
        }

        public bool Contains(string valValueToString, RevitParameter valRevitParameter, Element valElement)
        {
            bool vResult = false;
            List<RevitParameterInfocs> vList = GetParametersBy(valElement);
            if (vList != null)
            {
                if (string.IsNullOrEmpty(valValueToString))
                {
                    vResult = (vList.Where(p => p.ID == valRevitParameter.Id.ToString()).Count() > 0);
                }
                else
                {
                    vResult = (vList.Where(p => p.ID == valRevitParameter.Id.ToString() && p.Value == valValueToString).Count() > 0);

                }
            }
            return vResult;
        }
    }
}
