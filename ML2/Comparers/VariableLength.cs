using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2.Comparers
{
    public class VariableCompare : IComparer<Variable>
    {
        public int Compare(Variable x, Variable y)
        {
            return x.DomainSize.CompareTo(y.DomainSize);
        }
    }
}
