using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private Color[][] grid_array;
    

    public Grid(int w, int h)
    {
        width = w;
        height = h;
        grid_array = MatrixCreate(w, h);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid_array[i][j] = new Color(0.1f, 0.1f, 0.1f);
            }
        }
    }

    public void ChangeColor(int x, int y , Color color)
    {
        grid_array[x][y] = color;
    }

    public static Color[][] MatrixCreate(int rows, int cols)
    {
        Color[][] result = new Color[rows][];
        for (int i = 0; i < rows; ++i)
            result[i] = new Color[cols];
        return result;
    }
    public int Width()
    {
        return width;
    }
    public int Height()
    {
        return height;
    }
    public Color Color(int w, int h)
    {
        return grid_array[w][h];
    }

    public void Clear()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid_array[i][j] = new Color(0.1f, 0.1f, 0.1f);
            }
        }
    }

    public float Value(int x, int y)
    {      
        float result = grid_array[x][y].r;
        result += grid_array[x][y].g;
        result += grid_array[x][y].b;

        result /= 3f;

        return result;
    }
}