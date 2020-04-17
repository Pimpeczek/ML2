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
        public int SizeR { get; protected set; }
        public int SizeC { get; protected set; }
        public int SquareSizeC { get; protected set; }
        public int SquareSizeR { get; protected set; }
        public int InitialEmptyCells { get; protected set; }
        public Sudoku(string source)
        {
            string[] split = source.Split(';');
            SizeR = int.Parse(split[2]);
            SizeC = int.Parse(split[3]);
            SquareSizeR = int.Parse(split[4]);
            SquareSizeC = int.Parse(split[5]);
            InitialEmptyCells = 0;
            for (int i = split[6].Length - 1; i >= 0; i--)
            {
                if (split[6][i] == '.')
                    InitialEmptyCells++;
            }
            board = new Board(split[6], SizeR, SizeC, SquareSizeR, SquareSizeC);
        }

        public bool CheckRow(int[][] state, int row)
        {
            bool[] checkedNumbers = new bool[SizeR];
            for (int c = SizeR - 1; c >= 0; c--)
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
            bool[] checkedNumbers = new bool[SizeC];
            for (int r = SizeC - 1; r >= 0; r--)
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
            bool[] checkedNumbers = new bool[SquareSizeC * SquareSizeR];
            int cb = (c / SquareSizeC) * SquareSizeC;
            int rb = (r / SquareSizeR) * SquareSizeR;
            int cc = cb + SquareSizeC;
            int rr = rb + SquareSizeR;
            for (int i = rr - 1; i >= rb; i--)
            {
                for (int j = cc - 1; j >= cb; j--)
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
            for (int r = 0; r < SizeR; r++)
            {
                if (!CheckRow(state, r))
                    return false;
            }

            for (int c = 0; c < SizeC; c++)
            {
                if (!CheckCol(state, c))
                    return false;
            }

            for (int r = SizeR / SquareSizeR-1; r >= 0; r--)
            {
                for (int c = SizeC / SquareSizeC - 1; c >= 0; c--)
                {
                    if (!CheckSquare(state, SquareSizeR * r, SquareSizeC * c))
                        return false;
                }
            }
            return true;
        }

        public bool IsFilled(int[][] state)
        {
            for (int r = 0; r < SizeR; r++)
            {
                for (int c = 0; c < SizeC; c++)
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
            for (int i = 0; i < SizeR; i++)
            {
                for (int j = 0; j < SizeC; j++)
                {
                    if (board.CurState[i, j] == 0)
                    {
                        domain = new List<int>();
                        for (int n = 1; n <= SquareSizeC * SquareSizeR; n++)
                        {
                            domain.Add(n);
                        }
                        
                        for (int n = 0; n < SizeC; n++)
                        {
                            if (domain.Contains(board.CurState[i, n]))
                                domain.Remove(board.CurState[i, n]);

                        }
                        for (int n = 0; n < SizeR; n++)
                        {
                            if (domain.Contains(board.CurState[n, j]))
                                domain.Remove(board.CurState[n, j]);
                        }
                        for (int r = 0; r < SquareSizeR; r++)
                        {
                            for (int c = 0; c < SquareSizeC; c++)
                            {
                                if (domain.Contains(board.CurState[(i / SquareSizeR) * SquareSizeR + r, (j / SquareSizeC) * SquareSizeC + c ]))
                                    domain.Remove(board.CurState[(i / SquareSizeR) * SquareSizeR + r, (j / SquareSizeC) * SquareSizeC + c ]);
                            }
                        }
                        
                        vars[counter] = new CSP.Variable(i, j, new Domain(domain));
                        
                        counter++;
                    }
                }
            }
            return vars;
        }

        public CSP.Constraint[] PrepareConstraints()
        {
            CSP.Constraint[] con = new CSP.Constraint[1];
            con[0] = new Constraint(CheckAll);
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
            if (!CheckSquare(state, (R / 3) * 3, (C / 3) * 3))
                return false;
            return true;
        }
    }
}
