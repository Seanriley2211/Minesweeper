using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState {
    PREGAME,
    WAIT,
    NEWGAME,
    INPLAY,
    WON,
    LOSS,
}

public class GameMgr : MonoBehaviour {

    [Header("Game Settings")]
    [SerializeField] private GameObject tile;
    [SerializeField] private int cols;
    [SerializeField] private int rows;
    [SerializeField] private int mines;
    [SerializeField] public Sprite[] sprites;
    private int mineValue = -1;

    [Header("Tracking")]
    [SerializeField] private GameState currState;
    [SerializeField] private int minesRemaining;
    [SerializeField] private float time;
    [SerializeField] private bool cheat;

    [Header("UI Elements")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject uiTimer;
    [SerializeField] private GameObject uiMines;
    [SerializeField] private Text timerText;
    [SerializeField] private Text minesText;
    [SerializeField] private GameObject restart;

    [Header("Pregame")]
    [SerializeField] string[] settingsStrings = new string[] { "Easy", "Medium", "Hard", "Custom", "Start" };
    [SerializeField] private GameObject pregamePanel;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject descriptionButton;
    [SerializeField] private GameObject customButtom;

    public List<GameObject> board = new List<GameObject>();


    public bool Cheat {
        get { return cheat; }
        set { cheat = value; }
    }
    public int Cols {
        get { return cols; }
        set { cols = value; }
    }
    public int Rows {
        get { return rows; }
        set { rows = value; }
    }
    public int Mines {
        get { return mines; }
        set { mines = value; }
    }
    public int MinesRemaining {
        get { return minesRemaining; }
        set { this.minesRemaining = value; }
    }
    public GameState CurrState {
        get { return currState; }
    }

    private void Start() {
        ChangeState(GameState.PREGAME);
    }

    private void Update() {

        switch (currState) {

            //SELECT DIFFICULTY
            case GameState.PREGAME:
                for (int i = 0; i < settingsStrings.Length; i++) {
                    GameObject temp = Instantiate(settingsButton, pregamePanel.transform);
                    temp.GetComponent<Button>().ButtonID = i;
                    temp.transform.GetChild(0).GetComponent<Text>().text = settingsStrings[i];
                }

                ChangeState(GameState.WAIT);
                break;

            //WAITS FOR USER TO SELECT START GAME
            case GameState.WAIT:

                break;

            //INITIALIZE BOARD, START A GAME
            case GameState.NEWGAME:
                GenerateBoard();
                CenterCameraOverBoard();
                PlantTheMines();
                SetCellValues();
                uiMines.SetActive(true);
                uiTimer.SetActive(true);
                ChangeState(GameState.INPLAY);
                break;

            //GAME IN PROGRESS
            case GameState.INPLAY:
                time += Time.deltaTime;
                uiTimer.GetComponentInChildren<Text>().text = ((int)time).ToString();
                uiMines.GetComponentInChildren<Text>().text = MinesRemaining.ToString();

                //Handle double click
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1)
                    || Input.GetMouseButton(1) && Input.GetMouseButton(0)) {

                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                    Cell cell = hitInfo.transform.gameObject.GetComponent<Cell>();

                    if (hit && cell.Uncovered) {

                        // Uncover adjacent cells
                        List<GameObject> tempList = CountFlags(cell);
                        if ((8 - tempList.Count) == cell.MineValue) {

                            for (int i = 0; i < tempList.Count; i++) {
                                tempList[i].GetComponent<Cell>().CheckMineValue();
                            }
                        }
                    }


                //Handle Left Mouse Clicks
                } else if (Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1)) {

                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

                    if (hit)
                        hitInfo.transform.gameObject.GetComponent<Cell>().CheckMineValue();

                    CheckIfDone();

                //Handle Right Mouse Clicks
                } else if (Input.GetMouseButtonUp(1) && !Input.GetMouseButton(0)) {

                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

                    if (hit) {
                        Cell cell = hitInfo.transform.gameObject.GetComponent<Cell>();

                        //Am i placing or removing a flag?
                        if (!cell.Uncovered && !cell.Flagged) {
                            cell.PlantFlag();
                        } else if (!cell.Uncovered && cell.Flagged) {
                            cell.UnplantFlag();
                        }

                        //Update UI
                        uiMines.GetComponentInChildren<Text>().text = MinesRemaining.ToString();
                    }
                    CheckIfDone();
                }
                
                break;
            
            //Uncovers the entire board when user wins
            case GameState.WON:

                UncoverAll();
                break;

            //Uncovers the entire board when the user loses
            case GameState.LOSS:
                
                UncoverAll();
                break;

            default:
                break;

        }
    }



    //GENERATE GAMEBOARD, SET CELL IDS
    private void GenerateBoard() {

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                Vector3 spawnPoint = new Vector3(transform.position.x + j, transform.position.y - i, transform.position.z);
                GameObject temp = Instantiate(tile, spawnPoint, Quaternion.identity, this.gameObject.transform);
                temp.GetComponent<Cell>().Id = i * cols + j;
                board.Add(temp);
            }
        }
    }

    //Centers the camera over the board
    private void CenterCameraOverBoard() {
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        SpriteRenderer thisRenderer = transform.GetComponent<SpriteRenderer>();
        GameObject centerMark = gameObject.transform.GetChild(0).gameObject;

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        bounds.Encapsulate(thisRenderer.bounds);
        collider.center = bounds.center - transform.position;
        collider.size = bounds.size;

        Transform[] allKids = gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform kid in allKids) {
            SpriteRenderer kidRenderer = kid.GetComponent<SpriteRenderer>();
            if (kidRenderer != null) {
                bounds.Encapsulate(kidRenderer.bounds);
            }

            collider.center = bounds.center - transform.position;
            collider.size = bounds.size;
        }

        centerMark.transform.position = collider.center;
        Camera.main.transform.position = new Vector3(centerMark.transform.position.x, centerMark.transform.position.y, -1f);
    }

    //Determines which cells will contain a mine
    private void PlantTheMines() {

        for (int i = 0; i < mines; i++) {
            int randNum = Random.Range(0, rows * cols);
            board[randNum].GetComponent<Cell>().SetMineValue(mineValue);
            minesRemaining++;

            //DEBUG LINE TO COLORIZE THE MINES
            if (cheat) {
                board[randNum].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            }
        }
    }

    //Once all the mines are placed, 
    //this sets the values of all remaining cells
    private void SetCellValues() {
        int rowCount = 1;

        for (int i = 0; i < board.Count; i++) {
            
            Cell currCell = board[i].GetComponent<Cell>();
            if (i == cols * rowCount) {
                rowCount++;
            }
            
            //INCREMENT MINE VALUE FOR EACH CELL THAT ISN'T A MINE
            if (board[i].GetComponent<Cell>().MineValue != mineValue) {

                //NORTH
                if (i >= cols) {
                    
                    if (i != (cols * rowCount - 1) && board[i - cols + 1].GetComponent<Cell>().MineValue == mineValue) {
                        currCell.SetMineValue(currCell.MineValue + 1);
                    }
                    
                    if (board[i - cols].GetComponent<Cell>().MineValue == mineValue) {
                        currCell.SetMineValue(currCell.MineValue + 1);
                    }
                    
                    if (i % cols != 0 && board[i - cols - 1].GetComponent<Cell>().MineValue == mineValue) {
                        currCell.SetMineValue(currCell.MineValue + 1);
                    }
                }

                //SOUTH
                if (i < board.Count - cols) {
                    
                    if (i % cols != 0 && board[i + cols - 1].GetComponent<Cell>().MineValue == mineValue) {
                        currCell.SetMineValue(currCell.MineValue + 1);
                    }
                    
                    if (board[i + cols].GetComponent<Cell>().MineValue == mineValue) {
                        currCell.SetMineValue(currCell.MineValue + 1);
                    }
                    
                    if (i != (cols * rowCount - 1) && board[i + cols + 1].GetComponent<Cell>().MineValue == mineValue) {
                        currCell.SetMineValue(currCell.MineValue + 1);
                    }
                }

                //WEST
                if (i % cols != 0 && board[i - 1].GetComponent<Cell>().MineValue == mineValue) {
                    currCell.SetMineValue(currCell.MineValue + 1);
                }

                //EAST
                if (i != (cols * rowCount - 1) && board[i+1].GetComponent<Cell>().MineValue == mineValue) {
                    currCell.SetMineValue(currCell.MineValue + 1);
                }
            }
        }
    }

    //CHECK SURROUNDING CELLS
    public void CheckForZero() {
        int rowCount = 1;

        for (int i = 0; i < board.Count; i++) {
            if (i == cols * rowCount) {
                rowCount++;
            }

            //ONLY CHECKS FOR ZERO'S AROUND ACTIVATED CELLS
            if (board[i].GetComponent<Cell>().Activation == true) {

                //NORTH
                if (i >= cols) {

                    if (i != (cols * rowCount - 1) && !board[i - cols + 1].GetComponent<Cell>().Activation) {
                        board[i - cols + 1].GetComponent<Cell>().CheckMineValue();
                    }

                    if (!board[i - cols].GetComponent<Cell>().Activation) {
                        board[i - cols].GetComponent<Cell>().CheckMineValue();
                    }

                    if (i % cols != 0 && !board[i - cols - 1].GetComponent<Cell>().Activation) {
                        board[i - cols - 1].GetComponent<Cell>().CheckMineValue();
                    }
                }

                //SOUTH
                if (i < board.Count - cols) {
                    
                    if (i % cols != 0 && !board[i + cols - 1].GetComponent<Cell>().Activation) {
                        board[i + cols - 1].GetComponent<Cell>().CheckMineValue();
                    }
                    
                    if (!board[i + cols].GetComponent<Cell>().Activation) {
                        board[i + cols].GetComponent<Cell>().CheckMineValue();
                    }
                    
                    if (i != (cols * rowCount - 1) && !board[i + cols + 1].GetComponent<Cell>().Activation) {
                        board[i + cols + 1].GetComponent<Cell>().CheckMineValue();
                    }
                }

                //WEST
                if (i % cols != 0 && !board[i - 1].GetComponent<Cell>().Activation) {
                    board[i - 1].GetComponent<Cell>().CheckMineValue();
                }
                
                //EAST
                if (i != (cols * rowCount - 1) && !board[i + 1].GetComponent<Cell>().Activation) {
                    board[i + 1].GetComponent<Cell>().CheckMineValue();
                }
            }
        }
    }

    //COUNT FLAGS TOUCHING A SPECIFIC CELL
    public List<GameObject> CountFlags(Cell cell) {
        int rowCount = cell.Id+1;
        List<GameObject> cells = new List<GameObject>();

        //NORTH
        if (cell.Id >= cols) {

            if (cell.Id != (cols * rowCount - 1) && !board[cell.Id - cols + 1].GetComponent<Cell>().Flagged) {
                cells.Add(board[cell.Id - cols + 1]);
            }

            if (!board[cell.Id - cols].GetComponent<Cell>().Flagged) {
                cells.Add(board[cell.Id - cols]);
            }

            if (cell.Id % cols != 0 && !board[cell.Id - cols - 1].GetComponent<Cell>().Flagged) {
                cells.Add(board[cell.Id - cols - 1]);
            }
        }

        //SOUTH
        if (cell.Id < board.Count - cols) {

            if (cell.Id % cols != 0 && !board[cell.Id + cols - 1].GetComponent<Cell>().Flagged) {
                cells.Add(board[cell.Id + cols - 1]);
            }

            if (!board[cell.Id + cols].GetComponent<Cell>().Flagged) {
                cells.Add(board[cell.Id + cols]);
            }

            if (cell.Id != (cols * rowCount - 1) && !board[cell.Id + cols + 1].GetComponent<Cell>().Flagged) {
                cells.Add(board[cell.Id + cols + 1]);
            }
        }

        //WEST
        if (cell.Id % cols != 0 && !board[cell.Id - 1].GetComponent<Cell>().Flagged) {
            cells.Add(board[cell.Id - 1]);
        }
        //EAST
        if (cell.Id != (cols * rowCount - 1) && !board[cell.Id + 1].GetComponent<Cell>().Flagged) {
            cells.Add(board[cell.Id + 1]);
        }

        return cells;
    }

    //CHECK IF WE'RE FINISHED
    public void CheckIfDone() {
        int correctMine = 0;
        int incorrectMine = 0;
        int remainingCells = rows * cols;

        foreach (GameObject cell in board) {
            Cell thisCell = cell.GetComponent<Cell>();

            if (thisCell.Uncovered) { remainingCells--; }
            
            //if cell is a bomb and is flagged
            if (thisCell.MineValue == mineValue && thisCell.Flagged) {
                correctMine++;
                remainingCells--;

            } else if (thisCell.MineValue != mineValue && thisCell.Flagged) {

                incorrectMine++;
            }
        }

        //if player has found all mines and has no incorrectly flagged cells
        if (correctMine == mines && incorrectMine == 0) {
            ChangeState(GameState.WON);
        }

        //if player has eliminated all cells that aren't mines
        if (remainingCells == MinesRemaining && incorrectMine == 0) {
            ChangeState(GameState.WON);
        }
    }

    //UNCOVER EVERY CELL REGARDLESS OF CONDITION
    public void UncoverAll() {

        foreach (GameObject go in board) {
            Cell cell = go.GetComponent<Cell>();

            if (!cell.Uncovered) {
                cell.CheckMineValue();
            }
        }
        restart.SetActive(true);
    }

    public void ChangeState(GameState newState) {
        if (currState != newState) {
            currState = newState;
        }
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void RestartGame() {
        print("restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
    }
}
