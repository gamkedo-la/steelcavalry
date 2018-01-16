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

	void Update( )
	{
		SetNewCursorPosition( );
		ChangeToDefaultColor( );
		BlinkToIndicateMissiles( );
	}

	void FixedUpdate( )
	{
		CheckForMissiles( );
	}

	public void AddMissile( GameObject missile )
	{
		missiles.Add( missile );
	}

	private void SetNewCursorPosition( )
	{
		Cursor.visible = showHardwareCursor;
		transform.position = (Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition );
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
}
