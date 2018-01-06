using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {
	public float afterSec;

	// Use this for initialization
	void Start () {
		Destroy(gameObject, afterSec);
	}
}
