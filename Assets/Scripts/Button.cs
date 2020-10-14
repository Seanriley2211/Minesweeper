using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    [SerializeField] private int buttonID;
    [SerializeField] private GameDefaults gameDefaults;
    [SerializeField] private int custCols;
    [SerializeField] private int custRows;
    [SerializeField] private int custMines;

    private GameMgr gameMgr;
    private GameObject description;

    private void Start() {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();

        
    }

    public int ButtonID {
        get { return buttonID; }
        set { this.buttonID = value; }
    }

    //Sets the size of the game board, 
    //amount of mines the game will contain, 
    //and the position of the camera over the game board
    public void OnClick() {
        switch (ButtonID) {
            case 0:
                gameMgr.Cols = gameDefaults.EasyCols;
                gameMgr.Rows = gameDefaults.EasyRows;
                gameMgr.Mines = gameDefaults.EasyMines;
                Camera.main.orthographicSize = gameDefaults.EasyCamera;
                break;

            case 1:
                gameMgr.Cols = gameDefaults.MediumCols;
                gameMgr.Rows = gameDefaults.MediumRows;
                gameMgr.Mines = gameDefaults.MediumMines;
                Camera.main.orthographicSize = gameDefaults.MediumCamera;
                break;

            case 2:
                gameMgr.Cols = gameDefaults.HardCols;
                gameMgr.Rows = gameDefaults.HardRows;
                gameMgr.Mines = gameDefaults.HardMines;
                Camera.main.orthographicSize = gameDefaults.HardCamera;
                break;

            case 3:
                gameMgr.Cols = gameDefaults.CustomCols;
                gameMgr.Rows = gameDefaults.CustomRows;
                gameMgr.Mines = gameDefaults.CustomMines;
                Camera.main.orthographicSize = gameDefaults.HardCamera;
                break;

            case 4:
                gameMgr.ChangeState(GameState.NEWGAME);
                this.transform.parent.gameObject.SetActive(false);
                break;

            default:
                break;

        }
    }

}
