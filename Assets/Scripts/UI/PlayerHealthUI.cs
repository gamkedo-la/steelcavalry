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
	[SerializeField] private Text livesText;
	private bool isPlayer = false;
	private int lives;

	private GameObject player;
	private string playerTag = "Player";
	private Vector3 spawnPoint;

	private InGameMenu inGameMenu;

	// Use this for initialization
	void Start () {
		Assert.IsNotNull(playerHpBar);
		Assert.IsNotNull(playerThrustersBar);
		Assert.IsNotNull(healthUI);
		player = transform.parent.gameObject;

		lives = maxLives;
		spawnPoint = player.transform.position;

		// it's the player and not an enemy ai
		if(!player.GetComponent<AI>()) {
			isPlayer = true;
			GameObject LivesTextObject = GameObject.Find("Lives text");
			inGameMenu = GameObject.Find("In Game Menu UI").GetComponent<InGameMenu>();

			if (LivesTextObject) {				
				livesText = LivesTextObject.GetComponent<Text>();
				SetLivesText();
			}
		}
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
		SetLivesText();
		if (lives < 1) {
			inGameMenu.MissionFailed();
			Destroy(player);
		} else {
			RespawnPlayer();
		}
	}

	private void RespawnPlayer() {
		playerHpBar.fillAmount = 100f;
		player.transform.position = spawnPoint;
	}

	private void SetLivesText() {
		if(livesText) {			
			livesText.text = "x" + lives;
		}
	}
}
