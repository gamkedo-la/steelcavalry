using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MouseCursor : MonoBehaviour
{    
	[Header( "Objects" )]
	[SerializeField] private SpriteRenderer[] sprites = null;

	[Header( "Color" )]
	[SerializeField] private Color Color1;
	[SerializeField] private Color Color2;
	[SerializeField] private float changeTime = 1f;

	[Header( "Other" )]
	[SerializeField] private bool showHardwareCursor = false;
	[SerializeField] private float normalSubPos = 0.12f;
	[SerializeField] private float hitSubPos = 0.16f;

	private List<GameObject> missiles;
	private bool noMissilesInFlight = true;
	private float changeTimeLeft = 0f;
	private bool colo1Selected = true;

	void Start ()
	{
		Assert.IsNotNull( sprites );
		Assert.AreNotEqual( sprites.Length, 0 );

		missiles = new List<GameObject>( );
	}

	void FixedUpdate( )
	{
		SetNewCursorPosition( );
		ChangeToDefaultColor( );
		BlinkToIndicateMissiles( );

		CheckForMissiles( );
	}

	public void AddMissile( GameObject missile )
	{
		missiles.Add( missile );
	}

	public void OnHitEvent( float strength )
	{
		float time = strength / 10f;

		DoHitAnimation( false );
		CancelInvoke( "ReturnToNormal" );
		Invoke( "ReturnToNormal", time );
	}

	private void ReturnToNormal( )
	{
		DoHitAnimation( true );
	}

	private void SetNewCursorPosition( )
	{
		Cursor.visible = showHardwareCursor;
		transform.position = Utilities.GetMouseWorldPosition( Input.mousePosition );
	}

	private void ChangeToDefaultColor( )
	{
		if ( !noMissilesInFlight ) return;

			changeTimeLeft = 0;

		foreach ( var sprite in sprites )
		{
			sprite.color = Color.white;
		}
	}

	private void BlinkToIndicateMissiles( )
	{
		if ( noMissilesInFlight ) return;

		changeTimeLeft -= Time.deltaTime;
		if ( changeTimeLeft <= 0 )
		{
			changeTimeLeft = changeTime;

			Color c = colo1Selected ? Color1 : Color2;
			colo1Selected = !colo1Selected;

			foreach ( var sprite in sprites )
			{
				sprite.color = c;
			}
		}
	}

	private void CheckForMissiles( )
	{
		for ( int i = missiles.Count - 1; i >= 0; i-- )
		{
			if ( missiles[i] == null )
				missiles.RemoveAt( i );
		}

		if ( missiles.Count == 0 )
			noMissilesInFlight = true;
		else
			noMissilesInFlight = false;
	}

	private void DoHitAnimation( bool normal)
	{
		float newPos = normal ? normalSubPos : hitSubPos;

		foreach ( var sprite in sprites )
		{
			Vector2 spriteSubPos = sprite.gameObject.transform.localPosition;

			spriteSubPos.x = spriteSubPos.x > 0 ? newPos : -newPos;
			spriteSubPos.y = spriteSubPos.y > 0 ? newPos : -newPos;

			sprite.gameObject.transform.localPosition = spriteSubPos;
		}
	}
}
