using UnityEngine;
using UnityEngine.Assertions;

public class Destructible : MonoBehaviour
{
	[SerializeField] private PhysicsMaterial2D mat;
	[SerializeField] private int partsInRow = 10;
	[SerializeField] private float pixelsPerUnit = 100f;
	[SerializeField] private float expForceMin = 3f;
	[SerializeField] private float expForceMax = 5f;
	[SerializeField] private float disintegrateDelay = 5f;
	[SerializeField] private float disintegrateDelayDeltaMin = 0.1f;
	[SerializeField] private float disintegrateDelayDeltaMax = 0.3f;
	[SerializeField] private float disintegrateDecayTime = 1f;

	private GameObject spritesRoot;

	void Start ()
	{
		Assert.IsNotNull( mat );
		spritesRoot = GameObject.Find( "Pieces" );
	}

	public void DoDestruction( )
	{
		Texture2D source = GetComponent<SpriteRenderer>( ).sprite.texture;

		float partW = source.width / partsInRow;
		float partH = source.height / partsInRow;

		float wOffset = source.height / pixelsPerUnit / 2;
		float hOffset = source.width / pixelsPerUnit / 2;

		for ( int i = 0; i < partsInRow; i++ )
		{
			for ( int j = 0; j < partsInRow; j++ )
			{
				Sprite spr = Sprite.Create( source, new Rect( i * partW, j * partH, partW, partH ), new Vector2( 0.5f, 0.5f ) );

				GameObject newPiece = new GameObject( );
				newPiece.layer = 8;

				DissolveIntoBackground d = newPiece.AddComponent<DissolveIntoBackground>( );
				d.delay = disintegrateDelay;
				d.delayDelta = Random.Range( disintegrateDelayDeltaMin, disintegrateDelayDeltaMax );
				d.decayTime = disintegrateDecayTime;

				SpriteRenderer sr = newPiece.AddComponent<SpriteRenderer>( );
				sr.sprite = spr;

				BoxCollider2D b = newPiece.AddComponent<BoxCollider2D>( );
				b.sharedMaterial = mat;

				Rigidbody2D rb = newPiece.AddComponent<Rigidbody2D>( );
				rb.mass = 0.01f;
				rb.AddForce( Quaternion.Euler( 0, 0, Random.Range( 0, 360 ) ) * Vector2.left * Random.Range( expForceMin, expForceMax ) );

				newPiece.transform.position = new Vector3( transform.position.x - wOffset + i * (partW / pixelsPerUnit), transform.position.y - hOffset + j * ( partH / pixelsPerUnit ), 0 );
				newPiece.transform.parent = spritesRoot.transform;
			}
		}

		Destroy( gameObject );
	}
}
