using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject levelSelectMenu;
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider effectsVolume;
    [SerializeField] Slider musicVolume;
    public AudioSource music;

    private void Start() {
        music = GetComponent<AudioSource>();
        Debug.Log(GetComponent<AudioSource>());

        masterVolume.value = PlayerPrefs.GetFloat("masterVolume", 1f);
        effectsVolume.value = PlayerPrefs.GetFloat("effectsVolume", 1f);
        musicVolume.value = PlayerPrefs.GetFloat("musicVolume", 1f);
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

	public void CreditsTwo() {
		StartCoroutine(ChangeLevel("Credits 2"));
	}

	public void CreditsThree() {
		StartCoroutine(ChangeLevel("Credits 3"));
	}

	public void Menu() {
		SceneManager.LoadScene("Menu");
	}

    IEnumerator ChangeLevel(string sceneName) {
        float fadeTime = GameObject.Find("SceneTransition").GetComponent<SceneFading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneName);
    }

	public void QuitGame() {
        PlayerPrefs.Save();
        Application.Quit (); 
	}

    public void OpenOptions() {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptions() {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void ShowLevelSelect() {
        mainMenu.SetActive(false);
        levelSelectMenu.SetActive(true);
    }

    public void HideLevelSelect() {
        mainMenu.SetActive(true);
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
}
