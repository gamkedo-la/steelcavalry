using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {

	public ParticleSystem psScriptSmoke;
	private ParticleSystem.EmissionModule jetpackSmoke;
	private ParticleSystem.MinMaxCurve emissionWhenFiringJetpackSmoke;

	public ParticleSystem psScriptThrust;
	private ParticleSystem.EmissionModule jetpackThrust;
	private ParticleSystem.MinMaxCurve emissionWhenFiringJetpackThrust;

	// Use this for initialization
	void Start () {
		jetpackSmoke = psScriptSmoke.emission;
		emissionWhenFiringJetpackSmoke = jetpackSmoke.rateOverTime;
		// FIXME: above line not allowed in unity >5.4?
		// we are supposed to get this module from an actual ps instance, something like
		// GetComponentInChildren<ParticleSystem>.EmissionModule??? 

		jetpackSmoke.rateOverTime = 0;

		jetpackThrust = psScriptThrust.emission;
		emissionWhenFiringJetpackThrust = jetpackThrust.rateOverTime; // FIXME
		jetpackThrust.rateOverTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void JetpackToggle(bool on){
		if (on){
			jetpackSmoke.rateOverTime = emissionWhenFiringJetpackSmoke;
			jetpackThrust.rateOverTime = emissionWhenFiringJetpackThrust;
		}
		else if (!on){
			jetpackThrust.rateOverTime = jetpackSmoke.rateOverTime = 0;
		}
	}
}
