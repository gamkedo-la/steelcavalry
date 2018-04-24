using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {

	public static LocalizationManager instance;

	private Dictionary<string, string> localizedText;
	private bool isReady = false;
	private bool isLoading = false;
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
		isLoading = true;
		string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
		if (File.Exists(filePath) || filePath.Contains("://") || filePath.Contains(":///")) {
			SetSelectedLanguage(fileName);
			StartCoroutine(GetDataAsJson(filePath));
		} else {
			Debug.LogError("Cannot find file '" + filePath + "'!");
		}
	}

	private IEnumerator GetDataAsJson (string filePath) {
		string jsonData;
		if (filePath.Contains("://") || filePath.Contains(":///")) {
	        WWW www = new WWW(filePath);
	        yield return www;
	        jsonData = www.text;
		    SetLocalizedText(jsonData);
	    } else {
	        jsonData = File.ReadAllText(filePath);
		    SetLocalizedText(jsonData);
	    }
	}

	private void SetLocalizedText (string dataAsJson) {
		localizedText = new Dictionary<string, string>();
		LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
		
		for (int i = 0; i < loadedData.items.Length; i++){
			string key = loadedData.items[i].key;
			string value = loadedData.items[i].value;
			localizedText.Add(key, value);
		}

		UpdateLocalizedTextElements();
		isReady = true;
		isLoading = false;
		Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries") ;
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
		if (localizedText == null && !isLoading) {
			string selectedLanguage = PlayerPrefs.GetString("selectedLanguage", "english");
			Debug.Log("selectedLanguage is " + selectedLanguage);
			if (selectedLanguage == "english") {				
				LoadLocalizedText("localizedText_en.json");
			} else {
				LoadLocalizedText("localizedText_es.json");
			}
		}

		string localizedValue = missingTextString;
		if (localizedText != null && localizedText.ContainsKey(key)) {
			localizedValue = localizedText[key];
		}

		return localizedValue;
	}
}
