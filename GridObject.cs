using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Falling_sand;

public class GridObject
{
    public int[,] grid;

    public int saferead(int x, int y)
    {
        int r = 1;

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        if (x >= 0 && y >= 0 && x < width && y < height) r = grid[x, y];

        return r;
    }
    public void safewrite(int x, int y, int value)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        if (x >= 0 && y >= 0 && x < width && y < height) grid[x, y] = value;
    }


    static Random rand = new Random();
    public static GridObject Initialize(int x, int y)
    {
        GridObject Grid = new GridObject();
        Grid.grid = new int[x, y];

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                Grid.grid[j, i] = 0;
            }
        }
        return Grid;
    }

    public void gravitycalc()
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Loop in reverse order (from last row to first, last column to first)
        for (int i = cols - 2; i >= 0; i--)
        {
            for (int j = rows - 1; j >= 0; j--)
            {
                //check if currently particle
                if (saferead(j, i) == 0 && saferead(j, i) != 64000) continue;

                //check downward movement, we know current is filled
                if (saferead(j, i + 1) == 0)
                {
                    safewrite(j, i + 1, saferead(j, i));
                    safewrite(j, i, 0);

                    continue;
                }

                bool checkedright = false;
            
                if (rand.NextDouble() > 0.5f) goto left;


            right:
                if (saferead(j + 1, i) == 0 && saferead(j + 1, i + 1) == 0)
                {
                    safewrite(j + 1, i + 1, saferead(j, i));
                    safewrite(j, i, 0);
                    continue;
                }
                checkedright = true;
                
            left:
                if (saferead(j - 1, i) == 0 && saferead(j - 1, i + 1) == 0)
                {
                    safewrite(j - 1, i + 1, saferead(j, i));
                    safewrite(j, i, 0);
                    continue;
                }
                else if (!checkedright) goto right;
            }
        }
    }

}
