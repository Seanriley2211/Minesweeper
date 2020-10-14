using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Konami))]
public class CheatMode : MonoBehaviour {
    private Konami code;
    public Text successText;
    [SerializeField] private GameMgr gameMgr;

    private void Awake() {
        code = GetComponent<Konami>();
    }


    private void Update() {
        if (code.success) {
            successText.gameObject.SetActive(true);
            gameMgr.Cheat = true;
            code.gameObject.SetActive(false);
        }
    }
}
