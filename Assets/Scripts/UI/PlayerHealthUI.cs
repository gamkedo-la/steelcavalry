using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class PlayerHealthUI : MonoBehaviour {

	[SerializeField] private Image playerHpBar = null;
	[SerializeField] private Image playerThrustersBar = null;
	[SerializeField] private GameObject healthUI = null;
	[SerializeField] private int maxLives = 3;
	private int lives;

	private GameObject player;
	private string playerTag = "Player";
	private Vector3 spawnPoint;

	// Use this for initialization
	void Start () {
		Assert.IsNotNull(playerHpBar);
		Assert.IsNotNull(playerThrustersBar);
		Assert.IsNotNull(healthUI);
		player = transform.parent.gameObject;

		lives = maxLives;
		spawnPoint = player.transform.position;
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
		lives--;
		if (lives < 1) {
			Destroy(player);
		} else {
			RespawnPlayer();
		}
	}

	private void RespawnPlayer() {
		playerHpBar.fillAmount = 100f;
		player.transform.position = spawnPoint;
	}
}
