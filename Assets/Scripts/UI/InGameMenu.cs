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

	private GameObject audioManager;

	private GameObject resumeButton;
	private GameObject retryButton;
	private GameObject nextStageButton;
	private GameObject stageClearedPanel;

	[SerializeField] private GameEventAudioEvent stageClearedAudio = null;

	private string pauseText;
	private string playerDiedText;
	private string playerWonText;
	private string cityClearedText;
	private string spaceStationClearedText;
	private string enemyBaseClearedText;

	private List<string> sceneNames = new List<string>();
	private int totalScenes = 3;

	void Start () {
		pauseText = LocalizationManager.instance.GetLocalizedValue("game_paused");
		playerDiedText = LocalizationManager.instance.GetLocalizedValue("player_died");
		playerWonText = LocalizationManager.instance.GetLocalizedValue("player_won");

		cityClearedText = LocalizationManager.instance.GetLocalizedValue("city_cleared");
		spaceStationClearedText = LocalizationManager.instance.GetLocalizedValue("space_station_cleared");
		enemyBaseClearedText = LocalizationManager.instance.GetLocalizedValue("enemy_base_cleared");

		audioManager = GameObject.Find("Audio Manager");
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
		if (currentScene.name != "EnemyBase") {			
			nextStageButton.SetActive(true);
		}
		menuTitleText.text = playerWonText;
		stageClearedPanel.SetActive(true);
		stageClearedText.text = GetStageClearedText();
		playerWon = true;
		SavePlayerProgress();
		ReduceMusicVolume();
		PlayStageClearedVO();
	}

	void ReduceMusicVolume () {
		AudioSource music = audioManager.gameObject.GetComponent<AudioSource>();
		music.volume *= 0.25f;
	}

	void PlayStageClearedVO () {
		switch (currentScene.name) {
			case "Main Scene":
				stageClearedAudio.Raise(AudioEvents.CityClearedVO, transform.position);
				break;
			case "Space Station":
				stageClearedAudio.Raise(AudioEvents.SpaceStationClearedVO, transform.position);
		        break;
		    case "EnemyBase":
		    	stageClearedAudio.Raise(AudioEvents.EnemyBaseClearedVO, transform.position);
		        break;
		    default:
		    	break;
		}
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
