using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {

	public static bool gameIsPaused = false;
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
			if (gameIsPaused) {
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
	}

	public void MissionComplete () {
		Pause();
		DeactivateButtons();
		nextStageButton.SetActive(true);
		menuTitleText.text = playerWonText;
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
