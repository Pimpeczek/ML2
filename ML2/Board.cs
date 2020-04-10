using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2
{
    public class Board
    {
        public static int Size { get; protected set; }
        public static int SquareSize { get; protected set; }
        public int[,] CurState { get; protected set; }
        public bool[,] BlockedPos { get; protected set; }

        public Board(string source, int size, int squareSize)
        {
            CurState = new int[size, size];
            BlockedPos = new bool[size, size];
            char c;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    c = source[i * size + j];
                    CurState[i, j] = c == '.' ? 0 : int.Parse($"{c}");
                    BlockedPos[i, j] = CurState[i, j] != 0;
                }
            }
            Size = size;
            SquareSize = squareSize;
        }

        public string StateToString(int[,] state)
        {
            string str = "";
            for (int j = 0; j < Size; j++)
            {
                str += state[0, j] == 0 ? "  " : $"{state[0, j]}{(BlockedPos[0,j] ? '.' : ' ')}";
                if (j == 2 || j == 5)
                    str += "│ ";
            }
            for (int i = 1; i < Size; i++)
            {
                str += '\n';
                for (int j = 0; j < Size; j++)
                {
                    str += state[i, j] == 0 ? "  " : $"{state[i, j]}{(BlockedPos[i, j] ? '.' : ' ')}";
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
}
