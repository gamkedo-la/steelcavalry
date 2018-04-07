using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This is a singleton class component.
public class UIResourceManager : MonoBehaviour {
    public MouseCursor mouseCursor;

    public static MouseCursor MouseCursor;

    private InGameMenu inGameMenu;
    private Text enemyCountText;
    private Text droneCountText;

    private static UIResourceManager _instance;

    public static UIResourceManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<UIResourceManager>();
                // DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake() {
        if (_instance == null) {
            _instance = this;
            // DontDestroyOnLoad(this);
        } else if (this != _instance) {
            Destroy(gameObject);            
        }

        UIResourceManager.MouseCursor = mouseCursor;
        inGameMenu = GameObject.Find("In Game Menu UI").GetComponent<InGameMenu>();

        GetEnemyCountTexts();
        SetEnemyCountTexts();
    }

    void Update() {
        if (!inGameMenu.playerWon && !inGameMenu.playerLost) {            
            SetEnemyCountTexts();
        }
    }

    void GetEnemyCountTexts() {
        GameObject enemyCountTextObject = GameObject.Find("Enemy count text");
        enemyCountText = enemyCountTextObject.GetComponent<Text>();

        GameObject droneCountTextObject = GameObject.Find("Drone count text");
        droneCountText = droneCountTextObject.GetComponent<Text>();
    }

    public void SetEnemyCountTexts() {
        int totalAiPilots = FindObjectsOfType<AI>().Length;
        int totalDrones = FindObjectsOfType<Enemy>().Length;

        enemyCountText.text = "x" + totalAiPilots;
        droneCountText.text = "x" + totalDrones;
                
        int totalEnemies = totalAiPilots + totalDrones;
        if (totalEnemies < 1) {
            inGameMenu.MissionComplete();
        }
    }
}
