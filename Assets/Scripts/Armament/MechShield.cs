using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechShield : MonoBehaviour {
    void OnDisable()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            //TODO: Add some kind of effect
            Destroy(other.gameObject);
        }
    }
}
