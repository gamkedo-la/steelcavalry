using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
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
        SceneManager.LoadScene ("Main");
	}

	public void LevelTwo() {
		SceneManager.LoadScene ("Level 2");
	}

	public void Credits() {
		SceneManager.LoadScene ("Credits");
	}

	public void CreditsTwo() {
		SceneManager.LoadScene ("Credits 2");
	}

	public void CreditsThree() {
		SceneManager.LoadScene ("Credits 3");
	}

	public void Menu() {
		SceneManager.LoadScene ("Menu");
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

    public void MasterVolumeSlide(float volume) {
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void EffectsVolumeSlide(float volume) {
        PlayerPrefs.SetFloat("effectsVolume", volume);
    }

    public void MusicVolumeSlide(float volume) {
        PlayerPrefs.SetFloat("musicVolume", volume);
        music.volume = volume;
    }
}
