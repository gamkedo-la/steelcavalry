using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public static bool gameIsPaused = false;
	public GameObject inGameMenuUI;
	private Scene currentScene;
	private string pauseText = "Paused";
	private string playerDiedText = "Mission Failed";
	private string playerWonText = "Mission Complete";

	void Start () {
		currentScene = SceneManager.GetActiveScene();
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
	}

	public void Resume () {
		inGameMenuUI.SetActive(false);
		Time.timeScale = 1f;
		gameIsPaused = false;
		Cursor.visible = false;
	}

	void Pause () {
		inGameMenuUI.SetActive(true);
		Time.timeScale = 0f;
		gameIsPaused = true;
		Cursor.visible = true;
	}

	public void ReloadScene () {
		Time.timeScale = 1f;
		SceneManager.LoadScene(currentScene.name);
	}

	public void GoToNextStage () {
		// Handle going to the next stage
	}

	public void LoadMenu () {
		Time.timeScale = 1f;
		SceneManager.LoadScene("Menu");
	}

	public void QuitGame () {
		Application.Quit();
	}
}
