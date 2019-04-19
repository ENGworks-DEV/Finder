using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngFinder.Model
{
    public class RevitParameter
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ElementId ElementId { get; set; }
        public Definition Definition { get; set; }

    }
}
