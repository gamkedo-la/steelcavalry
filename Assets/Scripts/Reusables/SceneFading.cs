using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFading : MonoBehaviour {

	public Texture2D fadeOutTexture;
	public float fadeSpeed = 0.8f;

	private int drawDepth = -1000;
	private float alpha = 1.0f;
	private int fadeDirection = -1;

	void OnGUI () {
		alpha += fadeDirection * fadeSpeed * Time.deltaTime;
		// keep it between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		Rect rect = new Rect(0, 0, Screen.width, Screen.height);
		GUI.DrawTexture(rect, fadeOutTexture);
	}

	public float BeginFade (int direction) {
		fadeDirection = direction;
		return (fadeSpeed);
	}

	void OnEnable() {
		BeginFade(-1);
	}
}
