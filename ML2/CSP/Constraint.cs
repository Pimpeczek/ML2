using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2.CSP
{
    public class Constraint
    {
        protected Func<int[][], int, int, bool> check;

        public Constraint(Func<int[][], int, int, bool> check)
        {
            this.check = check;
        }
        public bool Check(int[][] state, int x, int y)
        {
            return check(state, x, y);
        }
    }
}
