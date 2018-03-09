using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeOutText : MonoBehaviour {
	public GameObject button;
	private Text buttonText;
	private Text displayText;
	private string fullText;

	public AudioClip click;
	public AudioClip consoleOn;
	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		buttonText = button.GetComponent<Text>();
		displayText = GetComponent<Text>();
		fullText = displayText.text;
		displayText.text = ">_";
		displayText.canvasRenderer.SetAlpha(0.0f);
		audioSource = GetComponent<AudioSource>();
		StartCoroutine (TypeLetter());
	}
	IEnumerator TypeLetter()
	{
		yield return new WaitForSeconds (1.00f);

		displayText.text = ">_";
		displayText.CrossFadeAlpha(1.0f, 0.75f, false);
		audioSource.PlayOneShot(consoleOn, 3.00f);

		yield return new WaitForSeconds (1.00f);

		displayText.text = ">";

		while (fullText.Length > 0) {
			displayText.text = displayText.text + fullText[0] + "_";
			fullText = fullText.Substring(1, fullText.Length - 1);

			if (fullText == "") {
				yield return new WaitForSeconds(1.0f);
				buttonText.text = "Start";
			} else if (fullText[0] == '\n') {
				fullText = fullText[0] + ">" + fullText.Substring(1, fullText.Length - 1);
				yield return new WaitForSeconds (0.40f);
			} else if (fullText[0] == '.') {
				yield return new WaitForSeconds(0.15f);
				audioSource.PlayOneShot(click, 0.05f);
			} else {
				audioSource.PlayOneShot(click, 0.05f);
				yield return new WaitForSeconds(0.05f);
			}
			displayText.text = displayText.text.Substring(0, displayText.text.Length - 1);
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
