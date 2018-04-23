using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {

	public static LocalizationManager instance;

	private Dictionary<string, string> localizedText;
	private bool isReady = false;
	private string missingTextString = "Localized text not found";

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}
	
	public void LoadLocalizedText (string fileName) {
		localizedText = new Dictionary<string, string>();
		string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
		if (File.Exists(filePath)) {
			
			SetSelectedLanguage(fileName);

			string selectedLanguage = PlayerPrefs.GetString("selectedLanguage", "spanish");
			string dataAsJson = File.ReadAllText(filePath);
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

			for (int i = 0; i < loadedData.items.Length; i++){
				string key = loadedData.items[i].key;
				string value = loadedData.items[i].value;
				localizedText.Add(key, value);
			}

			UpdateLocalizedTextElements();

			isReady = true;

			Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries") ;
		} else {
			Debug.LogError("Cannot find file '" + filePath + "'!");
		}
	}

	public bool GetIsReady () {
		return isReady;
	}

	public void SetSelectedLanguage (string fileName) {
		string selectedLanguage = "english";
		if (fileName != "localizedText_en.json") {
			selectedLanguage = "spanish";
		}
		PlayerPrefs.SetString("selectedLanguage", selectedLanguage);
	}

	public void UpdateLocalizedTextElements() {
		LocalizedText[] textObjects = FindObjectsOfType<LocalizedText>();
		foreach(LocalizedText textObject in textObjects) {
			textObject.SetLocalizedText();
		}
	}

	public string GetLocalizedValue (string key) {
		if (localizedText == null) {
			string selectedLanguage = PlayerPrefs.GetString("selectedLanguage", "english");
			Debug.Log("selectedLanguage is " + selectedLanguage);
			if (selectedLanguage == "english") {				
				LoadLocalizedText("localizedText_en.json");
			} else {
				LoadLocalizedText("localizedText_es.json");
			}
		}

		string localizedValue = missingTextString;
		
		if (localizedText.ContainsKey(key)) {
			localizedValue = localizedText[key];
		}

		return localizedValue;
	}
}
