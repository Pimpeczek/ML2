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
            StreamReader sr = new StreamReader("data\\puzzles.txt");
            Stopwatch sw = new Stopwatch();
            string[] lines;
            lines = sr.ReadToEnd().Split('\n');
            CSP.Solver solver;
            for (int i = 0; i < 40; i++)
            {
                
                s = new Sudoku(lines[i].Split(';')[2]);
                Console.WriteLine(s.board);
                solver = new CSP.Solver(s);
                sw.Restart();
                solver.Calculate();
                Console.WriteLine(i + "\t" + sw.ElapsedMilliseconds);
                Console.WriteLine(solver.FinalState);
                Console.ReadKey(true);
            }

            Console.ReadKey(true);
        }
    }
}
