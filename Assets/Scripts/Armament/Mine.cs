using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private GameObject explosion = null;

    public float explosionDestroyDelay = 2f;
    public float damagePerMine = 10f;

    // Use this for initialization
    public void Start()
    {

        //TODO: Random rotation for mixing it up

    }

    // Update is called once per frame
    public void Update()
    {
		// TODO: Are Mines static? Maybe they float around!
        this.transform.position = new Vector3(this.transform.position.x, (float)Utilities.floatEffect(this.transform.position.y,100), this.transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Try to find a Mech script on the hit object
        Mech mechInstance = collision.collider.GetComponent<Mech>();
        if (mechInstance)
        {
			//TODO: Add DoDestruction implementation 
			Debug.Log("hit a mech");
			mechInstance.TakeDamage(damagePerMine);
            ExplodeAndDestroy();
        }
    }

    private void ExplodeAndDestroy() {
        var newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(newExplosion, explosionDestroyDelay);
        Destroy(gameObject);
    }
}
