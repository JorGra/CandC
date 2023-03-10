using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] int fieldSizeX;
    [SerializeField] int fieldSizeY;
    [SerializeField] int maxGridValue;
    [SerializeField] float cellSize;
    [SerializeField] float gridUpdateIntervall;
    [SerializeField] int gridPiecesFilled;
    [SerializeField] float gridPiecesFilledPercentage;

    [Header("Spawn settings")]
    [SerializeField] float runnerAddIntervall = 5;
    [SerializeField] float runnerAddIntervallStart = 5;
    [SerializeField] float runnerAddIntervallMin = 0.5f;
    [SerializeField] float initalIdleTime = 5f;
    [SerializeField] GameObject gridObject;
    [SerializeField] GameObject runnerPrefab;


    [Header("Day-Night Cycle")]
    [SerializeField] Transform lightTransform;
    [SerializeField] Transform lightPositionDay;
    [SerializeField] Transform lightPositionNight;
    [SerializeField] float lightLerpSpeed = 100;
    [SerializeField] float dayFogDensity = 0.01f;
    [SerializeField] float nightFogDensity = 0.05f;
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;
    [SerializeField] Camera mainCamera;
    Transform lightTargetTransform;
    Color targetColor;
    float targetFogDensity;
    private bool night;
    bool lerpLight = false;

    public bool gameOver = false;

    [Header("Game state")]
    [SerializeField] TMP_Text gameOverText;

    [SerializeField] List<Runner> runners = new List<Runner>();

    Pathfinding pathfinding;
    Grid<GridPiece> runnerGrid;

    float currentRunnerAddTime;


    public static GameManager instance;

    float currentGameTime;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpGrid();
        lightTargetTransform = lightPositionDay;
        StartCoroutine(DayNightCycle());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < initalIdleTime)
            return;

        if (currentRunnerAddTime < Time.time && !gameOver)
        {
            currentRunnerAddTime = Time.time + runnerAddIntervall;
            AddRunner();
        }

        if (lerpLight)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lightTargetTransform.position, Vector3.up);
            lightTransform.rotation = Quaternion.RotateTowards(lightTransform.rotation, targetRotation, lightLerpSpeed * Time.deltaTime);

            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetColor, (lightLerpSpeed *0.05f)* Time.deltaTime);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, targetFogDensity, (lightLerpSpeed * 0.05f) * Time.deltaTime);
            mainCamera.backgroundColor = Color.Lerp(RenderSettings.fogColor, targetColor, (lightLerpSpeed * 0.05f) * Time.deltaTime);
        }
        currentGameTime += Time.deltaTime;
        runnerAddIntervall = (1 - Mathf.Exp(currentGameTime * 0.015f)) + runnerAddIntervallStart;
        runnerAddIntervall = Mathf.Clamp(runnerAddIntervall, runnerAddIntervallMin, runnerAddIntervallStart);

    }

    private void FixedUpdate()
    {
        if (gameOver)
            return;
        foreach (var runner in runners)
        {
            runner.UpdateRunner(night);
        }

    }

    private IEnumerator DayNightCycle()
    {
        yield return new WaitForSeconds(60); //60
        StartNight();
        yield return new WaitForSeconds(15);//15
        EndNight();
        yield return new WaitForSeconds(5);//10
        lerpLight = false;
        StartCoroutine(DayNightCycle());
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

    public void StartNight()
    {
        night = true;
        lerpLight = true;
        lightTargetTransform = lightPositionNight;

        targetColor = nightColor;
        targetFogDensity = nightFogDensity;
    }

    public void EndNight()
    {
        night = false;
        targetColor = dayColor;
        lightTargetTransform = lightPositionDay;
        targetFogDensity = dayFogDensity;

    }

    public void GridPieceFilled()
    {
        gridPiecesFilled += 1;
        gridPiecesFilledPercentage = (float)gridPiecesFilled / (fieldSizeX * fieldSizeY);

        if (gridPiecesFilledPercentage >= 1f)
            GameOver();
    }

    public void GridPieceUnfilled()
    {
        gridPiecesFilled -= 1;
        gridPiecesFilledPercentage = (float)gridPiecesFilled / (fieldSizeX * fieldSizeY);
    }

    void GameOver()
    {
        gameOver = true;
        foreach (var runner in runners.ToArray())
        {
            runner.DestroyRunner();
        }

        gameOverText.gameObject.SetActive(true);
        StopAllCoroutines();
    }

    public void RestartGame()
    {
        gameOver = false;
        currentGameTime = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }
        ScoreManager.instance.ResetScore();
        gameOverText.gameObject.SetActive(false);
        SetUpGrid();
        StartCoroutine(DayNightCycle());
    }

    public Grid<GridPiece> GetRunnerGrid() => runnerGrid;
}
