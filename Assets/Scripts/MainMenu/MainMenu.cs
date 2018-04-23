using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MainMenu : MonoBehaviour {
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject levelSelectMenu;
	[SerializeField] GameObject controlsMenu;
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider effectsVolume;
    [SerializeField] Slider musicVolume;
    public AudioSource music;

    private GameObject cityClearedPanel;
    private GameObject spaceStationClearedPanel;
    private GameObject enemyBaseClearedPanel;

    private void Start() {
        music = GetComponent<AudioSource>();
        Debug.Log(GetComponent<AudioSource>());

		if(masterVolume) {
			masterVolume.value = PlayerPrefs.GetFloat("masterVolume", 1f);
		}
		if(effectsVolume) {
			effectsVolume.value = PlayerPrefs.GetFloat("effectsVolume", 1f);
		}
		if(musicVolume) {
			musicVolume.value = PlayerPrefs.GetFloat("musicVolume", 1f);
		}
    }

    public void MissionDetails() {
        StartCoroutine(ChangeLevel("Mission Details"));
    }

    public void StartGame() {
        PlayerPrefs.Save();
        StartCoroutine(ChangeLevel("Main Scene"));
	}

    public void GoToSpaceStation() {
        StartCoroutine(ChangeLevel("Space Station"));
    }

    public void GoToEnemyBase() {
        StartCoroutine(ChangeLevel("EnemyBase"));
    }

	public void Credits() {
		StartCoroutine(ChangeLevel("Credits"));
	}

	public void Menu() {
		SceneManager.LoadScene("Menu");
	}

    IEnumerator ChangeLevel(string sceneName) {
        float fadeTime = GameObject.Find("SceneTransition").GetComponent<SceneFading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneName);
    }

	public void QuitGame() { // no longer used since mainly doing web build
		PlayerPrefs.Save();
        Application.Quit ();
	}

	public void MakeGames() {
		string URL = "https://gamkedo.com";
		#if UNITY_WEBGL
		openWindow(URL);
		#else
		Application.OpenURL(URL);
		#endif
	}

    public void OpenOptions() {
        mainMenu.SetActive(false);
        LocalizationManager.instance.UpdateLocalizedTextElements();
        optionsMenu.SetActive(true);
    }

    public void CloseOptions() {
        mainMenu.SetActive(true);
        LocalizationManager.instance.UpdateLocalizedTextElements();
        optionsMenu.SetActive(false);
    }

	public void OpenControls() {
		mainMenu.SetActive(false);
        LocalizationManager.instance.UpdateLocalizedTextElements();
		controlsMenu.SetActive(true);
	}

	public void CloseControls() {
		mainMenu.SetActive(true);
        LocalizationManager.instance.UpdateLocalizedTextElements();
		controlsMenu.SetActive(false);
	}

    public void ShowLevelSelect() {
        mainMenu.SetActive(false);
        LocalizationManager.instance.UpdateLocalizedTextElements();
        levelSelectMenu.SetActive(true);
        SetClearedPanels();
        DisplayProgress();
    }

    void SetClearedPanels () {        
        if (!cityClearedPanel) {
            cityClearedPanel = GameObject.Find("City Cleared");
        }
        if (!spaceStationClearedPanel) {
            spaceStationClearedPanel = GameObject.Find("Space Station Cleared");
        }
        if (!enemyBaseClearedPanel) {
            enemyBaseClearedPanel = GameObject.Find("Enemy Base Cleared");    
        }
    }

    void DisplayProgress() {
        bool hasClearedCity = PlayerPrefs.GetInt("cityCleared", 0) > 0;
        bool hasClearedSpaceStation = PlayerPrefs.GetInt("spaceStationCleared", 0) > 0;
        bool hasClearedEnemyBase = PlayerPrefs.GetInt("enemyBaseCleared", 0) > 0;

        cityClearedPanel.SetActive(hasClearedCity);
        spaceStationClearedPanel.SetActive(hasClearedSpaceStation);
        enemyBaseClearedPanel.SetActive(hasClearedEnemyBase);
    }

    public void ResetProgress() {
        PlayerPrefs.SetInt("cityCleared", 0);
        PlayerPrefs.SetInt("spaceStationCleared", 0);
        PlayerPrefs.SetInt("enemyBaseCleared", 0);
        PlayerPrefs.Save();
        DisplayProgress();
    }

    public void HideLevelSelect() {
        mainMenu.SetActive(true);
        LocalizationManager.instance.UpdateLocalizedTextElements();
        levelSelectMenu.SetActive(false);
    }

    public void MasterVolumeSlide(float volume) {
        PlayerPrefs.SetFloat("masterVolume", volume);
        music.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1f);
    }

    public void EffectsVolumeSlide(float volume) {
        PlayerPrefs.SetFloat("effectsVolume", volume);
    }

    public void MusicVolumeSlide(float volume) {
        PlayerPrefs.SetFloat("musicVolume", volume);
        music.volume = volume * PlayerPrefs.GetFloat("masterVolume", 1f);
    }

    public void SetLanguage(string fileName) {
        LocalizationManager.instance.LoadLocalizedText(fileName);
    }

	[DllImport("__Internal")]
	private static extern void openWindow(string url);
}
