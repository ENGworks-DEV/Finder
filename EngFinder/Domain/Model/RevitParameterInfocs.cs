using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngFinder.Model {
    public class RevitParameterInfocs {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Value { get; set; }
        public BuiltInParameterGroup Group { get; set; }
        public ParameterType Type { get; set; }
        public StorageType Storage { get; set; }
        public string Unit { get; set; }
        public bool Shared { get; set; }
        public bool ReadOnly { get; set; }
    }
}
