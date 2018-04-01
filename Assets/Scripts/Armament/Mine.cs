using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private GameObject explosion = null;
	[SerializeField] private GameEventAudioEvent explosionAudio = null;

	public bool deployed = false;
    public bool floatLR = false;
    public bool floatUD = false;
    public float explosionDestroyDelay = 2f;
    public float damagePerMine = 10f;
    public double floatWeight = 100;

    // Use this for initialization
    public void Start()
    {

        //TODO: Random rotation for mixing it up

    }

    // Update is called once per frame
    public void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        
		if(floatLR && floatUD)
        {
            this.transform.position = new Vector3((float)Utilities.floatEffect(this.transform.position.x,floatWeight), (float)Utilities.floatEffect(this.transform.position.y,floatWeight), this.transform.position.z);
        }
        else if(floatUD)
        {
            this.transform.position = new Vector3(this.transform.position.x, (float)Utilities.floatEffect(this.transform.position.y,floatWeight), this.transform.position.z);
        }
        else if(floatLR)
        {
            this.transform.position = new Vector3((float)Utilities.floatEffect(this.transform.position.x,floatWeight), this.transform.position.y, this.transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string colliderName = collision.collider.gameObject.name;
        if (colliderName.Contains("Shell")) return;
        ExplodeAndDestroy();
    }

    public void ExplodeAndDestroy()
	{
		explosionAudio.Raise( AudioEvents.MineExplosion, transform.position );

        var newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        var explosionParticles = newExplosion.gameObject.transform.GetChild(0);
        var explosionEnlarger = explosionParticles.GetComponent<ExplosionEnlarger>();
        explosionEnlarger.SetDamage(damagePerMine);

        Destroy(newExplosion, explosionDestroyDelay);
        Destroy(gameObject);
    }
}
