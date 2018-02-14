using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void StartGame() {
		SceneManager.LoadScene ("Main");
	}

	public void LevelTwo() {
		SceneManager.LoadScene ("Level 2");
	}

	public void Credits() {
		SceneManager.LoadScene ("Credits");
	}

	public void QuitGame() {
		Application.Quit (); 
	}

}
