using UnityEngine;
using UnityEngine.Assertions;

public class ShotBreaksIntoParticle : MonoBehaviour
{
	[SerializeField] private GameEventFloat didDamageEvent = null;
	[SerializeField] private GameObject pfx;
	[SerializeField] private float damagePerShot = 10.0f;

	private string nameOfMechPlayerIsIn;
	private string nameOfObjectHit;
    
    public Player.PlayerTeam fromTeam;


	void Start()
	{
		Assert.IsNotNull( didDamageEvent );
	}

	void OnCollisionEnter2D(Collision2D bumpFacts) {
        /*Debug.Log("Shot hit: " + bumpFacts.collider.gameObject.name +
		"Reminder: using Physics2D Layer ignore shenanigans for demo");*/

        Player collidedPlayer = bumpFacts.collider.GetComponent<Player>();
        Mech collidedMech = bumpFacts.collider.GetComponent<Mech>();

        if (collidedPlayer != null || collidedMech != null) {
            collidedPlayer = collidedPlayer == null ? collidedMech.driver : collidedPlayer;
        }

		if(collidedPlayer) {
			nameOfMechPlayerIsIn = collidedPlayer.GetComponent<Player>().getNameOfMechPlayerIsIn();

            if (nameOfMechPlayerIsIn.Length == 0) {
                nameOfMechPlayerIsIn = collidedPlayer.gameObject.name;
            }

			nameOfObjectHit = bumpFacts.collider.gameObject.name;
            
			// If the shot is from the player, ignore it
			if(fromTeam != Player.PlayerTeam.Independant && collidedPlayer.team == fromTeam && nameOfMechPlayerIsIn == nameOfObjectHit) {
                return;
			}            
		}

		// Try to find a Mech script on the hit object
		HP hp = bumpFacts.collider.GetComponent<HP>();
		if (hp)
		{
			didDamageEvent.Raise( 0.1f );
			hp.TakeDamage(damagePerShot);
		}

		GameObject pfxGO = Instantiate(pfx, transform.position, transform.rotation);
		pfxGO.transform.SetParent(LitterContainer.instanceTransform);
		if (gameObject) Destroy(gameObject);
	}

	public void SetDamage( float damage )
	{
		damagePerShot = damage;
	}
}
