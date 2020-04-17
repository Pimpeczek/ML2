using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2
{
    public class Board
    {
        public static int SizeR { get; protected set; }
        public static int SizeC { get; protected set; }
        public static int SquareSizeR { get; protected set; }
        public static int SquareSizeC { get; protected set; }
        public int[,] CurState { get; protected set; }
        public bool[,] BlockedPos { get; protected set; }

        public Board(string source, int sizeR, int sizeC, int squareSizeR, int squareSizeC)
        {
            CurState = new int[sizeR, sizeC];
            BlockedPos = new bool[sizeR, sizeC];
            string val;
            string[] split = source.TrimEnd("\n\r".ToCharArray()).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sizeR; i++)
            {
                for (int j = 0; j < sizeC; j++)
                {
                    val = split[i * sizeC + j];
                    CurState[i, j] = val == "." ? 0 : int.Parse($"{val}");
                    BlockedPos[i, j] = CurState[i, j] != 0;
                }
            }
            SizeR = sizeR;
            SizeC = sizeC;
            SquareSizeR = squareSizeR;
            SquareSizeC = squareSizeC;
        }

        public string StateToString(int[][] state)
        {
            return StateToString(Misc.ArrArrTo2dArr(state));
        }

        string IntToStr(int i)
        {
            if (i <= 9)
                return $"{i}";
            return $"{(char)(i + 55)}";
        }

        public string StateToString(int[,] state)
        {
            string str = "";
            for (int j = 0; j < SizeC; j++)
            {
                str += state[0, j] == 0 ? "  " : $"{IntToStr(state[0, j])}{(BlockedPos[0,j] ? '╴' : ' ')}";
                if ((j + 1) % SquareSizeC == 0 && j < SizeC - 1)
                    str += "│ ";
            }
            for (int i = 1; i < SizeR; i++)
            {
                str += '\n';
                for (int j = 0; j < SizeC; j++)
                {
                    str += state[i, j] == 0 ? "  " : $"{IntToStr(state[i, j])}{(BlockedPos[i, j] ? '╴' : ' ')}";
                    if ((j + 1) % SquareSizeC == 0 && j < SizeC - 1)
                        str += "│ ";
                }
                if ((i + 1) % SquareSizeR == 0 && i < SizeR - 1)
                {
                    str += '\n';
                    for (int j = 0; j < SizeC; j++)
                    {
                        if ((j + 1) % SquareSizeC == 0 && j < SizeC - 1)
                            str += "──┼─";
                        else
                            str += "──";
                    }
                }
            }
            return str;
        }

        public string ScrambleState(int seed, int iterations)
        {
            Random rng = new Random(seed);

            int[,] newState = (int[,])CurState.Clone();

            for (int i = 0; i < iterations; i++)
            {
                switch(rng.Next(5))
                {
                    case 0:
                        SwapBigColumn(newState, rng.Next(SizeC / SquareSizeC), rng.Next(SizeC / SquareSizeC));
                        break;
                    case 1:
                        SwapBigRow(newState, rng.Next(SizeR / SquareSizeR), rng.Next(SizeR / SquareSizeR));
                        break;
                    case 2:
                        SwapValues(newState, rng.Next(SquareSizeR * SquareSizeC) + 1, rng.Next(SquareSizeR * SquareSizeC) + 1);
                        break;
                    case 3:
                        MirrorBigColumn(newState, rng.Next(SizeC / SquareSizeC));
                        break;
                    case 4:
                        MirrorBigRow(newState, rng.Next(SizeR / SquareSizeR));
                        break;
                }
                
            }
            string res = $"0;0.0;{SizeR};{SizeC};{SquareSizeR};{SquareSizeC};";
            for (int i = 0; i < SizeR; i++)
            {
                for (int j = 0; j < SizeC; j++)
                {
                    res += (newState[i, j] == 0 ? "." : $"{newState[i, j]}") + (j >= SizeC - 1 && i >= SizeR - 1 ? "" : " ");
                }
            }
            return res;

        }

        void SwapBigColumn(int[,] state, int c1, int c2)
        {
            if (c1 == c2)
                return;
            c1 *= SquareSizeC;
            c2 *= SquareSizeC;
            for (int c = 0; c < SquareSizeC; c++)
            {
                for (int r = 0; r < SizeR; r++)
                {
                    Misc.Swap(ref state[r, c1 + c], ref state[r, c2 + c]);
                }
            }
        }
        void MirrorBigColumn(int[,] state, int c1)
        {
            c1 *= SquareSizeC;
            int swapColumns = SquareSizeC / 2;
            for (int r = 0; r < SizeR; r++)
            {
                for (int c = 0; c < swapColumns; c++)
                {
                    Misc.Swap(ref state[r, c1 + c], ref state[r, c1 + SquareSizeC - 1 - c]);
                }
            }
        }

        void SwapBigRow(int[,] state, int r1, int r2)
        {
            if (r1 == r2)
                return;
            r1 *= SquareSizeR;
            r2 *= SquareSizeR;
            for (int r = 0; r < SquareSizeR; r++)
            {
                for (int c = 0; c < SizeC; c++)
                {
                    Misc.Swap(ref state[r1 + r, c], ref state[r2 + r, c]);
                }
            }
        }
        void MirrorBigRow(int[,] state, int r1)
        {
            r1 *= SquareSizeR;
            int swapRows = SquareSizeR / 2;
            for (int c = 0; c < SizeC; c++)
            {
                for (int r = 0; r < swapRows; r++)
                {
                    Misc.Swap(ref state[r1 + r, c], ref state[r1 + SquareSizeR - 1 - r, c]);
                }
            }
        }

        void SwapValues(int[,] state, int v1, int v2)
        {
            if (v1 == v2)
                return;
            for (int r = 0; r < SizeR; r++)
            {
                for (int c = 0; c < SizeC; c++)
                {
                    if (state[r, c] == v1)
                        state[r, c] = v2;
                    else if (state[r, c] == v2)
                        state[r, c] = v1;
                }
            }
        }

        public override string ToString()
        {
            return StateToString(CurState);
        }
    }
}
