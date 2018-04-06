using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {

	public static bool gameIsPaused = false;
	public static bool playerWon = false;
	public static bool playerLost = false;

	public GameObject inGameMenuUI;

	private Scene currentScene;
	private Text menuTitleText;

	private GameObject resumeButton;
	private GameObject retryButton;
	private GameObject nextStageButton;

	private string pauseText = "Paused";
	private string playerDiedText = "Mission Failed";
	private string playerWonText = "Mission Complete";

	private List<string> sceneNames = new List<string>();
	private int totalScenes = 3;

	void Start () {
		SetSceneAndMenuUI();
		DeactivateMenu();
		DeactivateButtons();

		sceneNames.Add("Main Scene");
		sceneNames.Add("Space Station");
		sceneNames.Add("EnemyBase");
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (gameIsPaused && !playerWon && !playerLost) {
				Resume();
			} else {
				Pause();
			}
		}
		if (Input.GetKeyDown(KeyCode.X)) {
			MissionFailed();
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			MissionComplete();
		}
	}

	void SetSceneAndMenuUI () {
		currentScene = SceneManager.GetActiveScene();
		menuTitleText = GameObject.Find("MenuTitle").GetComponent<Text>();
		resumeButton = GameObject.Find("ResumeButton");
		retryButton = GameObject.Find("RetryButton");
		nextStageButton = GameObject.Find("NextStageButton");
	}

	void DeactivateMenu () {
		inGameMenuUI.SetActive(false);
	}

	void DeactivateButtons () {
		resumeButton.SetActive(false);
		retryButton.SetActive(false);
		nextStageButton.SetActive(false);
	}

	public void Resume () {
		inGameMenuUI.SetActive(false);
		Time.timeScale = 1f;
		gameIsPaused = false;
		Cursor.visible = false;
	}

	void Pause () {
		inGameMenuUI.SetActive(true);
		resumeButton.SetActive(true);
		Time.timeScale = 0f;
		gameIsPaused = true;
		Cursor.visible = true;
		menuTitleText.text = pauseText;
	}

	public void MissionFailed () {
		Pause();
		DeactivateButtons();
		retryButton.SetActive(true);
		menuTitleText.text = playerDiedText;
		playerLost = true;
	}

	public void MissionComplete () {
		Pause();
		DeactivateButtons();
		nextStageButton.SetActive(true);
		menuTitleText.text = playerWonText;
		playerWon = true;
		SavePlayerProgress();
	}

	private void SavePlayerProgress () {
		switch (currentScene.name) {
			case "Main Scene":
				PlayerPrefs.SetInt("cityCleared", 1);
				Debug.Log("updated cityCleared in playerprefs!");
				break;
			case "Space Station":
		        PlayerPrefs.SetInt("spaceStationCleared", 1);
		        Debug.Log("updated spaceStationCleared in playerprefs!");
		        break;
		    case "EnemyBase":
		        PlayerPrefs.SetInt("enemyBaseCleared", 1);
		        Debug.Log("updated enemyBaseCleared in playerprefs!");
		       	break;
		    default:
		    	Debug.Log("Tring to save progress on an invalid stage.");
		    	break;
		}
        PlayerPrefs.Save();
	}

	public void ReloadScene () {
		Time.timeScale = 1f;
		SceneManager.LoadScene(currentScene.name);
	}

	public void GoToNextStage () {
		int nextSceneIndex = sceneNames.IndexOf(currentScene.name) + 1;
		if (nextSceneIndex <= totalScenes) {
			Time.timeScale = 1f;
			string nextSceneName = sceneNames[nextSceneIndex];
			SceneManager.LoadScene(nextSceneName);
		}
	}

	public void LoadMenu () {
		Time.timeScale = 1f;
		SceneManager.LoadScene("Menu");
	}

	public void QuitGame () {
		Application.Quit();
	}
}
