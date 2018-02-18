using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitterContainer : MonoBehaviour {
	public static Transform instanceTransform;

	void Start () {
		instanceTransform = transform;
	}
}
