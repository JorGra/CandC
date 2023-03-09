using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] int fieldSizeX;
    [SerializeField] int fieldSizeY;
    [SerializeField] int maxGridValue;
    [SerializeField] float cellSize;
    [SerializeField] float gridUpdateIntervall;
    [SerializeField] float runnerAddIntervall = 5;

    public bool gameOver = false;

    [SerializeField] TMP_Text gameOverText;

    [SerializeField] List<Runner> runners = new List<Runner>();

    Pathfinding pathfinding;
    Grid<GridPiece> runnerGrid;
    [SerializeField] GameObject gridObject;
    [SerializeField] GameObject runnerPrefab;

    float currentRunnerAddTime;

    [SerializeField] int gridPiecesFilled;
    [SerializeField] float gridPiecesFilledPercentage;

    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpGrid();
    }

    void SetUpGrid()
    {
        pathfinding = new Pathfinding(fieldSizeX +1, fieldSizeY+1, cellSize);
        runnerGrid = new Grid<GridPiece>(fieldSizeX, fieldSizeY, cellSize, Vector3.zero, (Grid<GridPiece> grid, int x, int y) => CreateGridVisualizer(x, y));
        StartCoroutine(UpdateGrid(gridUpdateIntervall));

    }

    GridPiece CreateGridVisualizer(int x, int y)
    {
        var obj = Instantiate(gridObject, new Vector3(x, 0.01f, y) * cellSize, Quaternion.identity,transform);
        var gridPiece = new GridPiece(x, y, 0, obj.GetComponentInChildren<MeshRenderer>(), maxGridValue);
        return gridPiece;
    }

    public Vector3 GetRandomTargetPos()
    {
        var xRand = Random.Range(0, fieldSizeX +2);
        var yRand = Random.Range(0, fieldSizeY +2);
        var target = new Vector3(xRand, 0, yRand);
        //Debug.Log("Target: " + target);

        return target;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentRunnerAddTime < Time.time && !gameOver)
        {
            currentRunnerAddTime = Time.time + runnerAddIntervall;
            AddRunner();
        }
    }

    private void FixedUpdate()
    {
        if (gameOver)
            return;
        foreach (var runner in runners)
        {
            runner.UpdateRunner();
        }

    }
    IEnumerator UpdateGrid(float gridUpdateIntervall)
    {
        if (gameOver)
            yield return null;
        foreach (var runner in runners.ToArray())
        {
            if (runner.isGrabbed || !runner.isGrounded)
                continue;

            var gridPiece = runnerGrid.GetValue(runner.transform.position);
            if(gridPiece != null)
            {
                gridPiece.AddValue(1);
                runnerGrid.SetGridObject(runner.transform.position, gridPiece);
            }
        }
        yield return new WaitForSeconds(gridUpdateIntervall);
        StartCoroutine(UpdateGrid(gridUpdateIntervall));
    }

    void AddRunner()
    {
        var runner = Instantiate(runnerPrefab, GetRandomTargetPos() + Vector3.up * 50f, Quaternion.identity).GetComponent<Runner>();
        runners.Add(runner);
    }
    public void RemoveRunner(Runner runner)
    {
        runners.Remove(runner);
    }

    public void GridPieceFilled()
    {
        gridPiecesFilled += 1;
        gridPiecesFilledPercentage = (float)gridPiecesFilled / (fieldSizeX * fieldSizeY);

        if (gridPiecesFilledPercentage >= 0.99)
            GameOver();
    }

    void GameOver()
    {
        gameOver = true;
        foreach (var runner in runners.ToArray())
        {
            runner.DestroyRunner();
        }

        gameOverText.gameObject.SetActive(true);
    
    }

    public void RestartGame()
    {
        gameOver = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }
        ScoreManager.instance.ResetScore();
        gameOverText.gameObject.SetActive(false);
        SetUpGrid();
    }

}
