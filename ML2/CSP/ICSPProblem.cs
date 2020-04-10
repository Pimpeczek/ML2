using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2.CSP
{
    public interface ICSPProblem
    {
        bool CheckRow(int[][] state, int row);

        bool CheckCol(int[][] state, int col);

        bool CheckSquare(int[][] state, int c, int r);

        bool CheckPosFull(int[][] state, int x, int y);

        bool CheckPoint(int[][] state, int r, int c);

        bool CheckAll(int[][] state);

        bool IsFilled(int[][] state);

        Variable[] PrepareVariables();

        bool ShrinkDomains(int[][] state, Variable[] variables);

        bool ShrinkDomain(int[][] state, Variable variable);

        Constraint[] PrepareConstraints();

        State PrepareState();
    }
}
