using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngFinder.Core
{
    public class LibNumeric
    {

        public bool IsDouble(string valText)
        {
            Double vNum = 0;
            bool vResult = false;
            if (string.IsNullOrEmpty(valText))
            {
                return false;
            }
            vResult = Double.TryParse(valText, out vNum);
            return vResult;
        }

        public bool IsInt(string valText)
        {
            int vNum = 0;
            bool vResult = false;
            if (string.IsNullOrEmpty(valText))
            {
                return false;
            }
            vResult = int.TryParse(valText, out vNum);
            return vResult;
        }
    }
}
