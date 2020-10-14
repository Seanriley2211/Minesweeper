using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameDefaults : ScriptableObject {

    [SerializeField] private int easyCols = 9;
    [SerializeField] private int easyRows = 9;
    [SerializeField] private int easyMines = 10;
    [SerializeField] private int easyCamera = 8;
    [SerializeField] private int mediumCols = 16;
    [SerializeField] private int mediumRows = 16;
    [SerializeField] private int mediumMines = 40;
    [SerializeField] private int mediumCamera = 14;
    [SerializeField] private int hardCols = 30;
    [SerializeField] private int hardRows = 16;
    [SerializeField] private int hardMines = 99;
    [SerializeField] private int hardCamera = 18;
    [SerializeField] private int customCols = 0;
    [SerializeField] private int customRows = 0;
    [SerializeField] private int customMines = 0;
    [SerializeField] private int customCamera = 0;


    public int EasyCols {
        get { return easyCols; }
    }
    public int EasyRows {
        get { return easyRows; }
    }
    public int EasyMines {
        get { return easyMines; }
    }
    public int EasyCamera {
        get { return easyCamera; }
    }
    public int MediumCols {
        get { return mediumCols; }
    }
    public int MediumRows {
        get { return mediumRows; }
    }
    public int MediumMines {
        get { return mediumMines; }
    }
    public int MediumCamera {
        get { return mediumCamera; }
    }
    public int HardCols {
        get { return hardCols; }
    }
    public int HardRows {
        get { return hardRows; }
    }
    public int HardMines {
        get { return hardMines; }
    }
    public int HardCamera {
        get { return hardCamera; }
    }
    public int CustomCols {
        get { return customCols; }
        set { customCols = value; }
    }
    public int CustomRows {
        get { return customRows; }
        set { customRows = value; }
    }
    public int CustomMines {
        get { return customMines; }
        set { customMines = value; }
    }
    public int CustomCamera {
        get { return customCamera; }
        set { customCamera = value; }
    }
}
