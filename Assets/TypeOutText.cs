using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeOutText : MonoBehaviour {
	private Text displayText;
	private string fullText;
	// Use this for initialization
	void Start () {
		displayText = GetComponent<Text> ();
		fullText = displayText.text;
		displayText.text = "";
		StartCoroutine (TypeLetter ());
	}
	IEnumerator TypeLetter()
	{
		while (fullText.Length > 0) {
			// TODO: cut one letter off the start of full text and stick that letter on the end of dislpayText.text
			displayText.text = displayText.text + fullText[0];
			fullText = fullText.Substring (1, fullText.Length - 1);
			yield return new WaitForSeconds (0.1f);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
