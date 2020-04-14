using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2.CSP
{
    public class Constraint
    {
        protected Func<int[][], bool> check;

        public Constraint(Func<int[][], bool> check)
        {
            this.check = check;
        }
        public bool Check(int[][] state)
        {
            return check(state);
        }
    }
}
