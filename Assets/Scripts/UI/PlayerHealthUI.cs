using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class PlayerHealthUI : MonoBehaviour {

	[SerializeField] private Image playerHpBar = null;
	[SerializeField] private Image playerThrustersBar = null;
	[SerializeField] private GameObject healthUI = null;

	private GameObject player;
	private string playerTag = "Player";

	// Use this for initialization
	void Start () {
		Assert.IsNotNull(playerHpBar);
		Assert.IsNotNull(playerThrustersBar);
		Assert.IsNotNull(healthUI);
		player = GameObject.FindWithTag(playerTag);
	}
	
	void LateUpdate () {
		transform.rotation = Quaternion.identity;
	}

	public void SetHP ( float value ) {
		Debug.Log("SetHP received " + value);
		playerHpBar.fillAmount = value;
	}

	public void SetHealthVisibility (bool canShowUI){
		healthUI.SetActive(canShowUI);
	}

	public void DestroyPlayer() {
		Destroy(player);
	}
}
