using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPiece
{
    int x, y;
    int val;
    MeshRenderer tile;

    int maxVal = 5;

    public GridPiece(int x, int y, int val, MeshRenderer tile, int maxVal)
    {
        this.x = x;
        this.y = y;
        this.val = val;
        this.tile = tile;
        this.maxVal = maxVal;
    }

    public int GetValue() => val;

    public void SetValue(int newVal)
    {
        val = Mathf.Clamp(newVal, 0, maxVal);
        tile.material.SetColor("_BaseColor", Color.Lerp(Color.white, Color.black, val / maxVal));
    }

    public void AddValue(int valToAdd)
    {
        val = Mathf.Clamp(val + valToAdd, 0, maxVal);
        tile.material.SetColor("_BaseColor", Color.Lerp(Color.white, Color.black, (float)val / maxVal));
    }
}
