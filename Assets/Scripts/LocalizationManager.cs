using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {

	public static LocalizationManager instance;

	private Dictionary<string, string> localizedText;
	private bool isReady = false;

	// Use this for initialization
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
			string dataAsJson = File.ReadAllText(filePath);
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

			for (int i = 0; i < loadedData.items.Length; i++){
				string key = loadedData.items[i].key;
				string value = loadedData.items[i].value;
				localizedText.Add(key, value);
			}

			Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries") ;
		} else {
			Debug.LogError("Cannot find file '" + filePath + "'!");
		}

		isReady = true;
	}

	public bool getIsReady() {
		return isReady;
	}
}
