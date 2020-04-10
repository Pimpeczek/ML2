using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML2.CSP;

namespace ML2
{
    public class Sudoku : CSP.ICSPProblem
    {
        public Board board;
        public static readonly int Size = 9;
        public static readonly int SquareSize = 3;
        public int InitialEmptyCells { get; protected set; }
        public Sudoku(string source)
        {
            InitialEmptyCells = 0;
            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (source[i] == '.')
                    InitialEmptyCells++;
            }
            board = new Board(source, Size, SquareSize);
        }

        public bool CheckRow(int[][] state, int row)
        {
            bool[] checkedNumbers = new bool[Size];
            for (int c = Size - 1; c >= 0; c--)
            {
                if (state[row][c] > 0)
                {
                    if (checkedNumbers[state[row][c] - 1])
                    {
                        return false;
                    }
                    else
                    {
                        checkedNumbers[state[row][c] - 1] = true;
                    }
                }
            }
            return true;
        }

        public bool CheckCol(int[][] state, int col)
        {
            bool[] checkedNumbers = new bool[Size];
            for (int r = Size - 1; r >= 0; r--)
            {
                if (state[r][col] > 0)
                {
                    if (checkedNumbers[state[r][col] - 1])
                    {
                        return false;
                    }
                    else
                    {
                        checkedNumbers[state[r][col] - 1] = true;
                    }
                }
            }
            return true;
        }

        public bool CheckSquare(int[][] state, int c, int r)
        {
            bool[] checkedNumbers = new bool[9];
            c = (c / SquareSize) * SquareSize;
            r = (r / SquareSize) * SquareSize;
            int cc = c + SquareSize;
            int rr = r + SquareSize;
            for (int i = rr - 1; i >= r; i--)
            {
                for (int j = cc - 1; j >= c; j--)
                {
                    if (state[i][j] > 0)
                    {
                        if (checkedNumbers[state[i][j] - 1])
                        {
                            return false;
                        }
                        else
                        {
                            checkedNumbers[state[i][j] - 1] = true;
                        }
                    }
                }
            }
            return true;
        }

        public bool CheckPosFull(int[][] state, int x, int y)
        {
            if (!CheckCol(state, x))
                return false;
            if (!CheckRow(state, y))
                return false;
            if (!CheckSquare(state, x, y))
                return false;
            return true;
        }

        public bool CheckAll(int[][] state)
        {
            for (int r = 0; r < Size; r++)
            {
                if (!CheckRow(state, r))
                    return false;
                if (!CheckCol(state, r))
                    return false;
                if (!CheckSquare(state, (r / 3) * 3, (r % 3) * 3))
                    return false;
            }
            return true;
        }

        public bool IsFilled(int[][] state)
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (state[r][c] == 0)
                        return false;
                }
            }
            return true;
        }

        public CSP.Variable[] PrepareVariables()
        {

            CSP.Variable[] vars = new CSP.Variable[InitialEmptyCells];
            CSP.Domain dom;
            List<int> domain;
            int counter = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (board.CurState[i, j] == 0)
                    {
                        domain = new List<int>();
                        for (int n = 1; n <= Size; n++)
                        {
                            domain.Add(n);
                        }
                        for (int n = 0; n < Size; n++)
                        {
                            if (domain.Contains(board.CurState[i, n]))
                                domain.Remove(board.CurState[i, n]);

                            if (domain.Contains(board.CurState[n, j]))
                                domain.Remove(board.CurState[n, j]);

                            if (domain.Contains(board.CurState[(n / SquareSize), n % SquareSize]))
                                domain.Remove(board.CurState[(i / SquareSize) * SquareSize + (n / SquareSize), (j / SquareSize) * SquareSize + n % SquareSize]);
                        }
                        vars[counter] = new CSP.Variable(i, j, new Domain(domain));
                        //Console.WriteLine(vars[counter]);
                        counter++;
                    }
                }
            }
            Array.Sort(vars, new CSP.Comparers.VariableCompare());
            //Array.Reverse(vars);
            return vars;
        }

        public CSP.Constraint[] PrepareConstraints()
        {
            CSP.Constraint[] con = new CSP.Constraint[3];
            con[0] = new CSP.Constraint((s, x, y) => CheckRow(s, y));
            con[1] = new CSP.Constraint((s, x, y) => CheckCol(s, x));
            con[2] = new CSP.Constraint(CheckSquare);
            return con;
        }

        public CSP.State PrepareState()
        {
            return new CSP.State(board.CurState);
        }

        public bool ShrinkDomains(int[][] state, Variable[] variables)
        {
            int mem;
            Variable var;
            bool flag;
            for(int v = 0; v < variables.Length; v++)
            {
                if (!variables[v].Set)
                    if (!ShrinkDomain(state, variables[v]))
                        return false;
            }
            return true;
        }
        public bool ShrinkDomain(int[][] state, Variable variable)
        {
            int mem;
            for (int d = 0; d < variable.DomainSize; d++)
            {
                if (variable.Domain.Mask[d])
                {
                    mem = state[variable.R][variable.C];
                    state[variable.R][variable.C] = variable.Domain.Values[d];
                    if (!CheckRow(state, variable.R) || !CheckRow(state, variable.R) || !CheckRow(state, variable.R))
                    {
                        variable.Domain.HideValueByPosition(d);
                        if (variable.Domain.MaskedSize <= 0)
                        {
                            //Console.WriteLine("DUPA");
                            //Console.WriteLine(board.StateToString(Misc.ArrArrTo2dArr(state)));
                            //Console.WriteLine();
                            //Console.ReadKey(true);
                            state[variable.R][variable.C] = mem;
                            return false;
                        }
                    }
                    state[variable.R][variable.C] = mem;
                }
            }

            return true;
        }

        public bool CheckPoint(int[][] state, int R, int C)
        {
            if (!CheckRow(state, R))
                return false;
            if (!CheckCol(state, C))
                return false;
            if (!CheckSquare(state, (R / 3) * 3, (C % 3) * 3))
                return false;
            return true;
        }
    }
}
