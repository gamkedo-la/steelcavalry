using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pulse : MonoBehaviour {
	public Text buttonText;
	private bool pulseActive;

	// Use this for initialization
	void Start () {
		buttonText.canvasRenderer.SetAlpha(0.0f);
		pulseActive = false;
	}

	// Update is called once per frame
	void Update () {
		if (!pulseActive && buttonText.text == "Start")
		{
			pulseActive = true;
			StartCoroutine(PulseText());
		}
	}

	public void ChangeToStartButton()
	{
		buttonText.text = "Start";
	}

	IEnumerator PulseText()
	{
		while (true)
		{
			buttonText.CrossFadeAlpha(0.0f, 1.0f, false);
			yield return new WaitForSeconds(1.0f);
			buttonText.CrossFadeAlpha(0.0f, 1.0f, false);
			yield return new WaitForSeconds(1.0f);
		}
	}
}
