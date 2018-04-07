using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This is a singleton class component.
public class UIResourceManager : MonoBehaviour {
    public MouseCursor mouseCursor;

    public static MouseCursor MouseCursor;

    private InGameMenu inGameMenu;

    GameObject enemyThreats;
    GameObject droneThreats;

    private Text enemyCountText;
    private Text droneCountText;

    private int totalAiPilots;
    private int totalDrones;

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
        enemyThreats = GameObject.Find("Enemy threats");
        droneThreats = GameObject.Find("Drone threats");

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

        totalAiPilots = FindObjectsOfType<AI>().Length;
        totalDrones = FindObjectsOfType<Enemy>().Length;

        if (totalDrones < 1) {
            hideDroneCount();
        }

        if (totalAiPilots < 1) {
            hideEnemyCount();
        }
    }

    void hideEnemyCount() {
        enemyThreats.SetActive(false);
        Vector3 droneThreatsPosition = droneThreats.transform.localPosition;
        droneThreatsPosition.y = 0f;
        droneThreats.transform.localPosition = droneThreatsPosition;
    }

    void hideDroneCount() {
        droneThreats.SetActive(false);
    }

    public void SetEnemyCountTexts() {
        totalAiPilots = FindObjectsOfType<AI>().Length;
        totalDrones = FindObjectsOfType<Enemy>().Length;

        enemyCountText.text = "x" + totalAiPilots;
        droneCountText.text = "x" + totalDrones;
                
        int totalEnemies = totalAiPilots + totalDrones;
        if (totalEnemies < 1) {
            inGameMenu.MissionComplete();
        }
    }
}
