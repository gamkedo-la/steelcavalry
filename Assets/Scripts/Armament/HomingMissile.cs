using UnityEngine;
using UnityEngine.Assertions;

public class HomingMissile : MonoBehaviour
{
	[SerializeField] private GameEventAudioEvent audioEvent;
	[SerializeField] private GameObject explosion = null;
	[SerializeField] private GameEventFloat didDamageEvent = null;
	[SerializeField] private float speed = 5f;
	[SerializeField] private float rotatingSpeed= 200f;
	[SerializeField] private float damagePerMissile = 20f;
    [HideInInspector] public Player.PlayerTeam fromTeam;

    private Transform target;
	private Rigidbody2D rb;

	[HideInInspector]
	public int playerNumber;
    GameObject mechShooting;

    MissileLauncher missileLauncher;
    WeaponManager weaponMgr;

    void Start( )
	{
        rb = GetComponent<Rigidbody2D>( );

		Assert.IsNotNull( rb );
		Assert.IsNotNull( explosion );
		Assert.IsNotNull( didDamageEvent );
		Assert.IsNotNull( audioEvent );
        


        //ignore collider of shooting mech if circle collider (i.e. winged mech)
        //Debug.Log("Shooting Mech was " + mechShooting.name);
        //type of collider
        /*
        //TODO: error if hierarchy in MissileLauncher too long (see TODO in MissileLauncher).
        GameObject holder = mechShooting.gameObject;
        Debug.Log("Mech Shooting " + holder.name);
        Collider2D colType = holder.GetComponent<CircleCollider2D>();
        
        if (colType is CircleCollider2D)
        {
            //Debug.Log("IM HERE");
            //TODO: the other mechs can destroy themselves, not when circle collider is set to mech
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), mechShooting.GetComponent<CircleCollider2D>());
        }*/   
        
    }

    void FixedUpdate( )
	{
		Vector2 targetPos = Vector2.zero;
		if (target == null)
		{
			targetPos = Utilities.GetMouseWorldPosition( Input.mousePosition );
		}

		if (Vector2.Distance(targetPos, transform.position) <= 0.1f)
		{
			DoDestruction( transform.position );
			return;
		}

		Vector2 vector = (Vector2)transform.position - targetPos;
		vector.Normalize( );
		float moveVector = Vector3.Cross( vector, transform.right ).z;

		rb.angularVelocity = rotatingSpeed * moveVector;
		rb.velocity = transform.right * speed;
	}

    public void ReceiveMechName(GameObject shootingMech)//determine shooting mech
    {
        //Debug.Log("The firing Mech is " + shootingMech.name);
        mechShooting = shootingMech;
    }

	private void OnCollisionEnter2D( Collision2D collision )
	{
        Debug.Log("Missile Hit " + collision.gameObject.name);

        Player collidedPlayer = collision.collider.GetComponent<Player>();
        Mech collidedMech = collision.collider.GetComponent<Mech>();

        if (collidedPlayer != null || collidedMech != null) {
            collidedPlayer = collidedPlayer == null ? collidedMech.driver : collidedPlayer;
        }

        if (fromTeam != Player.PlayerTeam.Independant && collidedPlayer && collidedPlayer.team == fromTeam)
            return;

        // Try to find a Mech script on the hit object
        HP hp = collision.collider.GetComponent<HP>();
        //Debug.Log("Mech hit was " + hp.gameObject.name);
		if (hp)
		{
			didDamageEvent.Raise( 0.5f );
			hp.TakeDamage(damagePerMissile);
		}

		if ( collision.contacts.Length > 0 ) {
			DoDestruction( collision.contacts[0].point );
		} else {
			DoDestruction( transform.position );
		}

		Mine mine = collision.collider.GetComponent<Mine>();
		if (mine) {
			mine.ExplodeAndDestroy();
		}
	}

	public void DoDestruction( Vector2 point )
	{
		audioEvent.Raise( AudioEvents.Explosion, transform.position );

		var exp = Instantiate( explosion, point, Quaternion.identity );

		Destroy( exp, 2f );
		Destroy( gameObject );
	}

	public void SetTarget( Transform target )
	{
		this.target = target;
	}

	public void SetDamage( float damage )
	{
		damagePerMissile = damage;
	}
}
