using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Tools
{
    public static class DataMapTools
    {
        public static int[,] MergeArrayWith(this int[,] array1, int[,] array2)
        {
            int rows = array1.GetLength(0);
            int cols = array1.GetLength(1);

            if (rows != array2.GetLength(0) || cols != array2.GetLength(1))
            {
                throw new ArgumentException("Both arrays must have the same dimensions.");
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (array2[i, j] != 0)
                    {
                        array1[i, j] = array2[i, j];
                    }
                }
            }

            return array1;
        }
    }
}
