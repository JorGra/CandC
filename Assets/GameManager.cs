using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int fieldSizeX;
    [SerializeField] int fieldSizeY;
    [SerializeField] int maxGridValue;
    [SerializeField] float cellSize;
    [SerializeField] float runnerAddIntervall = 5;

    [SerializeField] List<Runner> runners = new List<Runner>();

    Pathfinding pathfinding;
    Grid<GridPiece> runnerGrid;
    [SerializeField] GameObject gridObject;
    [SerializeField] GameObject runnerPrefab;

    float currentRunnerAddTime;


    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(fieldSizeX, fieldSizeY, cellSize);
        runnerGrid = new Grid<GridPiece>(fieldSizeX, fieldSizeY, cellSize, Vector3.zero, (Grid<GridPiece> grid, int x, int y) => CreateGridVisualizer(x, y));
    }


    GridPiece CreateGridVisualizer(int x, int y)
    {
        var obj = Instantiate(gridObject, new Vector3(x, 0.01f, y) * cellSize, Quaternion.identity,transform);
        var gridPiece = new GridPiece(x, y, 0, obj.GetComponent<MeshRenderer>());
        return gridPiece;
    }

    public Vector3 GetRandomTargetPos()
    {
        var xRand = Random.Range(0, fieldSizeX);
        var yRand = Random.Range(0, fieldSizeY);
        var target = new Vector3(xRand, 0, yRand);
        //Debug.Log("Target: " + target);

        return target;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var runner in runners)
        {
            runner.UpdateRunner();

            var val = runnerGrid.GetValue(runner.transform.position);
            val.AddValue(1);
            runnerGrid.SetGridObject(runner.transform.position, val);
        }

        if(currentRunnerAddTime < Time.time)
        {
            currentRunnerAddTime = Time.time + runnerAddIntervall;
            AddRunner();
        }
    }

    void AddRunner()
    {
        var runner = Instantiate(runnerPrefab, GetRandomTargetPos(), Quaternion.identity).GetComponent<Runner>();
        runners.Add(runner);
    }
}
