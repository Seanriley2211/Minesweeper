using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {

    private int centerX;
    private int centerY;

    private void Start() {
        centerX = GameObject.Find("GameMgr").GetComponent<GameMgr>().Cols / 2;
        centerY = GameObject.Find("GameMgr").GetComponent<GameMgr>().Rows / 2;
    }

    private void Update() {
        Vector3.MoveTowards(transform.position, new Vector3(centerX, centerY, transform.position.z), 1);
    }
}
