using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Values {
    FLAG = 9,
    COVER = 10,
    MAYBE = 11,
    EXPLODED = 12,
    DISARMED = 13,
    WRONG = 14,
}

public class Cell : MonoBehaviour {

    private Values values;
    private GameMgr gameMgr;
    private Cell cell;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private int id;
    [SerializeField] private int mineValue = -1;
    [SerializeField] private bool activation;       //THIS IS ONLY FOR CHECKING CASCADING UNCOVERINGS!!!
    [SerializeField] private bool uncovered;
    [SerializeField] private bool flagged;

    public void SetMineValue(int mineVal) {
        this.mineValue = mineVal;
    }
    public bool Flagged {
        get { return flagged; }
        set { this.flagged = value; }
    }
    public bool Activation {
        get { return activation; }
        set { this.activation = value; }
    }
    public bool Uncovered {
        get { return uncovered; }
        set { this.uncovered = value; }
    }
    public int MineValue {
        get { return mineValue; }
        set { this.mineValue = value; }
    }
    public int Id {
        get { return id; }
        set { this.id = value; }
    }

    private void Start() {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        cell = this.GetComponent<Cell>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }


    //Checks the value of an individual cell.
    public void CheckMineValue() {

        //If the cell contains a mine, 
        //the sprite will be a "disarmed" sprite in a successful game 
        //or it will be an "exploded" sprite in an unsuccessful game
        if (MineValue == -1) {

            if (gameMgr.CurrState == GameState.WON) {
                spriteRenderer.sprite = gameMgr.sprites[(int)Values.DISARMED];
                GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);

            } else {

                spriteRenderer.sprite = gameMgr.sprites[(int)Values.EXPLODED];
                gameMgr.ChangeState(GameState.LOSS);
            }
        }

        //If the cell contains no mine nearby and was not flagged, 
        //the gameMgr script will check the value of the surrounding cells
        if (MineValue == 0 && !Flagged) {

            if (gameMgr.CurrState == GameState.INPLAY) {
                Activation = true;
                Uncovered = true;
                gameMgr.CheckForZero();
                spriteRenderer.sprite = gameMgr.sprites[0];
            }

        } else if (MineValue == 0 && Flagged) {

            if (gameMgr.CurrState == GameState.LOSS) {
                spriteRenderer.sprite = gameMgr.sprites[(int)Values.WRONG];
            }
        }

        //If there is a bomb nearby but not marked with a flag
        if (MineValue > 0 && !Flagged && !Activation) {

            if (gameMgr.CurrState == GameState.INPLAY && !Uncovered) {
                Uncovered = true;
                spriteRenderer.sprite = gameMgr.sprites[cell.MineValue];
            }
        }
    }

    public void PlantFlag() {
        Flagged = true;
        gameMgr.MinesRemaining--;

        //update icon
        spriteRenderer.sprite = gameMgr.sprites[9];
    }

    //UNPLANT THE FLAG
    public void UnplantFlag() {
        Flagged = false;
        gameMgr.MinesRemaining++;

        //update icon
        spriteRenderer.sprite = gameMgr.sprites[(int)Values.COVER];
    }
}
    
