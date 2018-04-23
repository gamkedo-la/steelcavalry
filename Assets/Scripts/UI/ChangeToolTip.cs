using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeToolTip : MonoBehaviour {
	public Text toolTip;
	
	// Update is called once per frame
	void OnDestroy() {
		toolTip.text = LocalizationManager.instance.GetLocalizedValue("tooltip_enter_mech");
	}
}
