using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2.CSP
{
    public class State
    {
        public int[][] Values { get; protected set; }

        public State(int[,] board)
        {
            Values = Misc.Arr2dToArrArr(board);
        }

        public State(int[][] board)
        {
            Values = board;
        }

        public State MakeClone()
        {
            int[][] newVals = new int[Values.Length][];
            for (int r = Values.Length - 1; r >= 0; r--)
            {
                newVals[r] = new int[Values[r].Length];
                for (int c = newVals[r].Length - 1; c >= 0; c--)
                {
                    newVals[r][c] = Values[r][c];
                }
            }
            return new State(newVals);
        }

        public override string ToString()
        {
            string str = $"{Values[0][0]}";
            for (int c = 1; c < Values[0].Length; c++)
            {
                str += $" {Values[0][c]}";
            }
            for (int r = 1; r < Values.Length; r++)
            {
                str += $"\n{Values[r][0]}";
                for (int c = 1; c < Values[r].Length; c++)
                {
                    str += $" {Values[r][c]}";
                }
            }
            return str;
        }
    }
}
