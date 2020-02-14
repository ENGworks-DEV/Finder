using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngFinder.Utils
{
    public static class StringClasses
    {
        public static List<string> lstNames()
        {
            List<string> result = new List<string>();

            result.Add("Comments");
            result.Add("Mark");
            result.Add("Family Name");
            result.Add("Free Size");
            result.Add("Insulation Type");
            result.Add("Lining Type");
            result.Add("System Abbreviation");
            result.Add("System Classification");
            result.Add("System Classification");
            result.Add("System Name");
            result.Add("Type Name");

            return result;
        }
    }
}
