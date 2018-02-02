using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class MechUI : MonoBehaviour
{
	[SerializeField] private Image hpBar = null;
	[SerializeField] private Text nameText = null;

	[SerializeField] private Image playerHpBar = null;
	[SerializeField] private Image playerThrustersBar = null;

	[SerializeField] private GameObject playerUI = null;
	[SerializeField] private GameObject npcUI = null;

	private bool isPlayer = false;

	void Start ()
	{
		Assert.IsNotNull( hpBar );
		Assert.IsNotNull( nameText );
		Assert.IsNotNull( playerHpBar );
		Assert.IsNotNull( playerThrustersBar );
		Assert.IsNotNull( playerUI );
		Assert.IsNotNull( npcUI );

		SetUI( );
	}

	void LateUpdate ()
	{
		transform.rotation = Quaternion.identity;
	}

	public void IsPlayerDriving( bool player )
	{
		isPlayer = player;

		SetUI( );
	}

	public void SetHP( float value )
	{
		hpBar.fillAmount = value;
		playerHpBar.fillAmount = value;
	}

	public void SetName( string value )
	{
		nameText.text = value;
	}

	private void SetUI()
	{
		if( isPlayer )
		{
			npcUI.SetActive( false );
			playerUI.SetActive( true );
		}
		else
		{
			npcUI.SetActive( true );
			playerUI.SetActive( false );
		}
	}
}
