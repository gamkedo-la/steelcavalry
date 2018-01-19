using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {
	private float minAngle = -10;
	private float maxAngle = 10;
	public Mech mech;

	private Vector3 originalAngles;

	// Use this for initialization
	void Start () {
		originalAngles = transform.rotation.eulerAngles;
	}

	// Update is called once per frame
	void Update () {
		if (!mech.inUse) return;
		Vector3 mousePosition = Utilities.GetMouseWorldPosition(Input.mousePosition);
		bool mouseIsBehindMech = (
			!(mousePosition.x < transform.position.x && !mech.isFacingRight) &&
			!(mousePosition.x > transform.position.x && mech.isFacingRight)
		);
		if (mouseIsBehindMech) return;

		Vector3 positionDifference = mousePosition - transform.position;
		positionDifference.Normalize();

		float zCorrection = 0;
		float yAngle = 90f;
		float xAngle = Mathf.Atan2(positionDifference.y, positionDifference.x) * Mathf.Rad2Deg;

		if (mech.isFacingRight) {
			xAngle = Mathf.Clamp(xAngle, minAngle, maxAngle);
		} else {
			if (xAngle > 0) {
				xAngle = Mathf.Clamp(xAngle, 180f + minAngle, 180f);
			} else {
				xAngle = Mathf.Clamp(xAngle, -180f, -180f + maxAngle);
			}

			zCorrection = 180;
			xAngle += 180;
		}

		transform.rotation = Quaternion.Euler(
			xAngle + originalAngles.x,
			yAngle + originalAngles.y,
			originalAngles.z + zCorrection
		);
	}
}
