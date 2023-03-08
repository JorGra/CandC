using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPiece
{
    int x, y;
    int val;
    MeshRenderer tile;

    const int MAX_VAL = 15;

    public GridPiece(int x, int y, int val, MeshRenderer tile)
    {
        this.x = x;
        this.y = y;
        this.val = val;
        this.tile = tile;
    }

    public int GetValue() => val;

    public void SetValue(int newVal)
    {
        val = Mathf.Clamp(newVal, 0, MAX_VAL);
        tile.material.SetColor("_BaseColor", Color.Lerp(Color.white, Color.black, val / MAX_VAL));
    }

    public void AddValue(int valToAdd)
    {
        val = Mathf.Clamp(val + valToAdd, 0, MAX_VAL);
        tile.material.SetColor("_BaseColor", Color.Lerp(Color.white, Color.black, (float)val / MAX_VAL));
    }
}
