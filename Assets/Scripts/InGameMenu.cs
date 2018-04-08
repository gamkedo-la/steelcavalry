using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {

	public static bool gameIsPaused = false;
	public bool playerWon = false;
	public bool playerLost = false;

	public GameObject inGameMenuUI;

	private Scene currentScene;
	private Text menuTitleText;
	private Text stageClearedText;

	private GameObject resumeButton;
	private GameObject retryButton;
	private GameObject nextStageButton;
	private GameObject stageClearedPanel;

	private string pauseText = "Paused";
	private string playerDiedText = "Mission Failed";
	private string playerWonText = "Mission Complete";

	private string cityClearedText = 
		"Well done, private! It looks like more of those space pirates are " + 
		"attacking a transport ship. I just sent you their coordinates. Get over there ASAP!";

	private string spaceStationClearedText = 
		"Good work! While you kept the ship safe, we traced outgoing " +
		"transmissions back to the pirate base. Coordinates sent. Let's go round 'em up!";

	private string enemyBaseClearedText = 
		"It looks like you got 'em all! A team is inbound to secure the area " + 
		"and confiscate their equipment. Come on back to HQ for debriefing. Excellent work!";

	private List<string> sceneNames = new List<string>();
	private int totalScenes = 3;

	void Start () {
		SetSceneAndMenuUI();
		DeactivateMenu();
		DeactivateButtonsAndPanels();

		sceneNames.Add("Main Scene");
		sceneNames.Add("Space Station");
		sceneNames.Add("EnemyBase");
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) && !playerWon && !playerLost) {
			if (gameIsPaused) {
				Resume();
			} else {
				Pause();
			}
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			MissionComplete();
		}
	}

	void SetSceneAndMenuUI () {
		currentScene = SceneManager.GetActiveScene();
		menuTitleText = GameObject.Find("MenuTitle").GetComponent<Text>();
		stageClearedPanel = GameObject.Find("StageClearedPanel");
		stageClearedText = GameObject.Find("StageCleared").GetComponent<Text>();
		resumeButton = GameObject.Find("ResumeButton");
		retryButton = GameObject.Find("RetryButton");
		nextStageButton = GameObject.Find("NextStageButton");
	}

	void DeactivateMenu () {
		inGameMenuUI.SetActive(false);
	}

	void DeactivateButtonsAndPanels () {
		resumeButton.SetActive(false);
		retryButton.SetActive(false);
		nextStageButton.SetActive(false);
		stageClearedPanel.SetActive(false);
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
		DeactivateButtonsAndPanels();
		retryButton.SetActive(true);
		menuTitleText.text = playerDiedText;
		playerLost = true;
	}

	public void MissionComplete () {
		Pause();
		DeactivateButtonsAndPanels();
		nextStageButton.SetActive(true);
		menuTitleText.text = playerWonText;
		stageClearedPanel.SetActive(true);
		stageClearedText.text = GetStageClearedText();
		playerWon = true;
		SavePlayerProgress();
	}

	public string GetStageClearedText() {
		switch (currentScene.name) {
			case "Main Scene":
				return cityClearedText;
			case "Space Station":
		        return spaceStationClearedText;
		    case "EnemyBase":
		        return enemyBaseClearedText;
		    default:
		    	return "";
		}
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
