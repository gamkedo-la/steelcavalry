using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeOutText : MonoBehaviour {
	public GameObject button;
	private Text buttonText;
	private Text displayText;
	private string fullText;
	// Use this for initialization
	void Start () {
		buttonText = button.GetComponent<Text> ();
		displayText = GetComponent<Text> ();
		fullText = displayText.text;
		displayText.text = ">_";
		StartCoroutine (TypeLetter ());
	}
	IEnumerator TypeLetter()
	{
		yield return new WaitForSeconds (1.00f);
		displayText.text = ">";

		while (fullText.Length > 0) {
			displayText.text = displayText.text + fullText[0] + "_";
			fullText = fullText.Substring (1, fullText.Length - 1);

			if (fullText.Length == 1 && buttonText.text == "Skip") {
				buttonText.text = "Start";
			}

			if (fullText != "" && fullText[0] == '\n') {
				fullText = fullText[0] + ">" + fullText.Substring(1, fullText.Length-1);
				yield return new WaitForSeconds (0.40f);
			} else {
				yield return new WaitForSeconds (0.05f);
			}

			displayText.text = displayText.text.Substring (0, displayText.text.Length - 1);
		}

		displayText.text += "_";
	}

	// Update is called once per frame
	void Update () {

	}
}
