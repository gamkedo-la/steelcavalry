using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDimmer : MonoBehaviour {
	private Light lightToDim;

	private float minIntensity = 2;
	private float maxIntensity = 8;
	private float intensityStep = 0.05f;
	private bool intensityIsIncreasing = true;
	private int intensityModifier = 1;

	void Start () {
		lightToDim = this.GetComponent<Light>();
		lightToDim.intensity = minIntensity;
	}
	
	void Update () {
		if (!lightToDim) return;
		float currentIntensity = lightToDim.intensity;

		if (intensityIsIncreasing && currentIntensity >= maxIntensity) {
			intensityIsIncreasing = false;
		}
		if (!intensityIsIncreasing && currentIntensity <= minIntensity) {
			intensityIsIncreasing = true;
		}

		intensityModifier = intensityIsIncreasing ? 1 : -1;

		lightToDim.intensity += intensityStep * intensityModifier;
	}
}
