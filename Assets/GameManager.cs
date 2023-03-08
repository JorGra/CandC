using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int fieldSizeX;
    [SerializeField] int fieldSizeY;
    [SerializeField] float cellSize;

    Pathfinding pathfinding;
    Grid<int> testGrid;
    [SerializeField] GameObject gridObject;

    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(fieldSizeX, fieldSizeY);
        testGrid = new Grid<int>(fieldSizeX, fieldSizeY, cellSize, Vector3.zero, (Grid<int> grid, int x, int y) => CreateGridVisualizer(x, y));
    }


    int CreateGridVisualizer(int x, int y)
    {
        Instantiate(gridObject, new Vector3(x, 0.01f, y) * cellSize, Quaternion.identity,transform);
        return 0;
    }

    public Vector3 GetRandomTargetPos()
    {
        var xRand = Random.Range(0, fieldSizeX);
        var yRand = Random.Range(0, fieldSizeY);
        var target = new Vector3(xRand, 0, yRand);

        return target;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
