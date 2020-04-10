using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2
{
    public static class Misc
    {
        public static T[][] Arr2dToArrArr<T>(T[,] arr2d)
        {
            T[][] arrArr = new T[arr2d.GetLength(0)][];
            int width = arr2d.GetLength(1);
            for (int h = arrArr.Length - 1; h >= 0; h--)
            {
                arrArr[h] = new T[width];
                for (int w = width - 1; w >= 0; w--)
                {
                    arrArr[h][w] = arr2d[h, w];
                }
            }
            return arrArr;
        }

        public static T[,] ArrArrTo2dArr<T>(T[][] arrArr)
        { 

            int width = arrArr[0].Length;
            T[,] arr2d = new T[arrArr.Length, width];
            
            for (int h = arrArr.Length - 1; h >= 0; h--)
            {
                for (int w = width - 1; w >= 0; w--)
                {
                    arr2d[h,w] = arrArr[h][w];
                }
            }
            return arr2d;
        }
    }
}
