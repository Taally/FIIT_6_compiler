using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{
    static class ThreeAddressCodeTmp
    {
        private static int tmpInd = 1, tmpLabelInd = 1;
        public static string GenTmpName() => "#t" + tmpInd++;
        public static string GenTmpLabel() => "L" + tmpLabelInd++;
    }
}
