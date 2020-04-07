using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ML2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Sudoku s;
            Solver csp;
            StreamReader sr = new StreamReader("puzzles.txt");
            Stopwatch sw = new Stopwatch();
            Variable[] variables;
            string[] lines = sr.ReadToEnd().Split('\n');
            s = new Sudoku(lines[0].Split(';')[2]);
            Console.WriteLine(s.board.ToString());

            
            Console.ReadKey(true);
        }
    }

    public class Solver
    {
        protected Variable[] variables;
        protected Constraint[] constraints;
        protected Stack<int[,]> stateStack;
        public int[,] FinalState;
        protected int curVar;
        int counter = 0;
        public double PossibleCombinations { get; protected set; }
        public Solver(Variable[] variables, Constraint[] constraints, int[,] state)
        {
            this.variables = variables;
            PossibleCombinations = variables[0].DomainSize;
            for (int i = 1; i < variables.Length; i++)
            {
                PossibleCombinations *= variables[i].DomainSize;
            }
            this.constraints = constraints;
            stateStack = new Stack<int[,]>();
            stateStack.Push(state);
            curVar = -1;
            
        }

        public void Calculate()
        {
            Try();
            stateStack.Clear();

        }

        protected bool Try()
        {
            curVar++;
            if (curVar >= variables.Length)
                return false;
            int[,] newState = (int[,])stateStack.Peek().Clone();
            stateStack.Push(newState);
            for (int d = variables[curVar].DomainSize-1; d >=0; d--)
            {
                newState[variables[curVar].R, variables[curVar].C] = variables[curVar].Domain.Values[d];

                counter++;

                if (Sudoku.CheckAll(newState))
                {
                    if (Sudoku.IsFilled(newState))
                    {
                        FinalState = stateStack.Pop();
                        stateStack.Clear();
                        return true;
                    }
                    if(Try())
                    {
                        return true;
                    }
                    else
                    {
                        stateStack.Pop();
                    }
                    curVar--;
                }
                
            }

            return false;
        }

        public void WriteState()
        {
            Console.WriteLine($"{Board.StateToString(stateStack.Peek())}\n");
        }

    }

    public class VariableCompare : IComparer<Variable>
    {
        public int Compare(Variable x, Variable y)
        {
            return x.DomainSize.CompareTo(y.DomainSize);
        }
    }

    public class Sudoku
    {
        public Board board;
        public static readonly int Size = 9;
        public static readonly int SquareSize = 3;
        public int InitialEmptyCells { get; protected set; }
        public Sudoku(string source)
        {
            InitialEmptyCells = 0;
            for (int i = source.Length-1; i >=0; i--)
            {
                if (source[i] == '.')
                    InitialEmptyCells++;
            }
            board = new Board(source, Size, SquareSize);
        }

        public static bool CheckRow(int[,] state, int row)
        {
            bool[] checkedNumbers = new bool[Size];
            for (int c = Size-1; c >= 0; c--)
            {
                if (state[row, c] > 0)
                {
                    if (checkedNumbers[state[row, c]-1])
                    {
                        return false;
                    }
                    else
                    {
                        checkedNumbers[state[row, c]-1] = true;
                    }
                }
            }
            return true;
        }

        public static bool CheckCol(int[,] state, int col)
        {
            bool[] checkedNumbers = new bool[Size];
            for (int r = Size-1; r >= 0; r--)
            {
                if (state[r, col] > 0)
                {
                    if (checkedNumbers[state[r, col]-1])
                    {
                        return false;
                    }
                    else
                    {
                        checkedNumbers[state[r, col]-1] = true;
                    }
                }
            }
            return true;
        }

        public static bool CheckSquare(int[,] state, int c, int r)
        {
            bool[] checkedNumbers = new bool[9];
            c = (c / SquareSize) * SquareSize;
            r = (r / SquareSize) * SquareSize;
            int cc = c + SquareSize;
            int rr = r + SquareSize;
            for (int i = rr -1; i >= r; i--)
            {
                for (int j = cc-1; j >= c; j--)
                {
                    if (state[i, j] > 0)
                    {
                        if (checkedNumbers[state[i, j]-1])
                        {
                            return false;
                        }
                        else
                        {
                            checkedNumbers[state[i, j]-1] = true;
                        }
                    }
                }
            }
            return true;
        }

        public static bool CheckPosFull(int[,] state, int x, int y)
        {
            if (!CheckCol(state, x))
                return false;
            if (!CheckRow(state, y))
                return false;
            if (!CheckSquare(state, x, y))
                return false;
            return true;
        }

        public static bool CheckAll(int[,] state)
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

        public static bool IsFilled(int[,] state)
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (state[r, c] == 0)
                        return false;
                }
            }
            return true;
        }

        public Variable[] PrepareVariables()
        {

            Variable[] vars = new Variable[InitialEmptyCells];
            Domain dom;
            int counter = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (board.CurState[i, j] == 0)
                    {
                        dom = new Domain(new List<int>());
                        for (int n = 1; n <= Size; n++)
                        {
                            dom.Values.Add(n);
                        }
                        for (int n = 0; n < Size; n++)
                        {
                            if (dom.Values.Contains(board.CurState[i, n]))
                                dom.Values.Remove(board.CurState[i, n]);

                            if (dom.Values.Contains(board.CurState[n, j]))
                                dom.Values.Remove(board.CurState[n, j]);

                            if (dom.Values.Contains(board.CurState[(n / SquareSize), n % SquareSize]))
                                dom.Values.Remove(board.CurState[ (i / SquareSize) * SquareSize + (n / SquareSize), (j / SquareSize) * SquareSize + n % SquareSize]);
                        }
                        vars[counter] = new Variable(i, j, dom);
                        //Console.WriteLine(vars[counter]);
                        counter++;
                    }
                }
            }

            return vars;
        }

        public Constraint[] PrepareConstraints()
        {
            Constraint[] con = new Constraint[3];
            con[0] = new Constraint((s, x, y) => CheckRow(s, y));
            con[1] = new Constraint((s, x, y) => CheckCol(s, x));
            con[2] = new Constraint(CheckSquare);
            return con;
        }

    }

    public class Board
    {
        public static int Size { get; protected set; }
        public static int SquareSize { get; protected set; }
        public int[,] CurState { get; protected set; }

        protected int[][,] states;
        public Board(string source, int size, int squareSize)
        {
            CurState = new int[size, size];
            char c;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    c = source[i * size + j];
                    CurState[i,j] = c == '.' ? 0 : int.Parse($"{c}");
                }
            }
            Size = size;
            SquareSize = squareSize;
        }

        public static string StateToString(int[,]state)
        {
            string str = "";
            for (int j = 0; j < Size; j++)
            {
                str += state[0, j] == 0 ? "  " : $"{state[0, j]} ";
                if (j == 2 || j == 5)
                    str += "│ ";
            }
            for (int i = 1; i < Size; i++)
            {
                str += '\n';
                for (int j = 0; j < Size; j++)
                {
                    str += state[i, j] == 0 ? "  " : $"{state[i, j]} ";
                    if (j == 2 || j == 5)
                        str += "│ ";
                }
                if (i == 2 || i == 5)
                {
                    str += '\n';
                    for (int j = 0; j < Size; j++)
                    {
                        if (j == 2 || j == 5)
                            str += "──┼─";
                        else
                            str += "──";
                    }
                }
            }
            return str;
        }

        public override string ToString()
        {
            return StateToString(CurState);
        }
    }

    public class Domain
    {
        public List<int> Values { get; protected set; }
        public Domain(int lowerBoud, int upperBoud)
        {
            Values = new List<int>();
            for (int i = lowerBoud; i <= upperBoud; i++)
                Values.Add(i);
        }

        public Domain(List<int> values)
        {
            Values = values;
        }

        public override string ToString()
        {
            
            if(Values.Count > 0)
            {
                string str = $"[{Values[0]}";
                for(int i = 1; i < Values.Count; i++)
                {
                    str += $", {Values[i]}";
                }
                return str + ']';
            }
            return "[]";
        }
    }

    public class Variable
    {
        public int R { get; protected set; }
        public int C { get; protected set; }
        
        public int DomainSize
        {
            get
            {
                return Domain.Values.Count;
            }
        }
        public Domain Domain { get; protected set; }

        public Variable(int r, int c, Domain domain)
        {
            R = r;
            C = c;
            Domain = domain;
        }

        public override string ToString()
        {
            return $"RC=[{R},{C}]|D={Domain}";
        }
    }

    public class Constraint
    {
        protected Func<int[,], int, int, bool> check;

        public Constraint(Func<int[,], int, int, bool> check)
        {
            this.check = check;
        }
        public bool Check(int[,] state, int x, int y)
        {
            return check(state, x, y);
        }
    }
}
