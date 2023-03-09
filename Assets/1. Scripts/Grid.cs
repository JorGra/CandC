using System;
using UnityEngine;

public class Grid<T>
{
    private int height;
    private int width;
    private float cellSize;
    private Vector3 originPosition;
    private T[,] gridArray;


    public event EventHandler<OnGridObjectChangedEventArgs> OnGridValueChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<T>, int, int, T> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new T[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
    }

    public Vector3 GetWordPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });

    }

    public void SetGridObject(Vector3 worldPosition, T value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);   
    }


    public T GetGridObject(int x, int y)
    {
        if (x < 0 || x >= width)
            x = Mathf.Clamp(x, 0, width-1);
        if (y < 0 || y >= height)
            y = Mathf.Clamp(y, 0, height - 1);

        return gridArray[x, y];
        //if (x >= 0 && y >= 0 && x < width && y < height)
        //else
        //    return default(T);
    }

    public T GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public int GetHeight() => height;
    public int GetWidth() => width;
    public float GetCellSize() => cellSize;
}
