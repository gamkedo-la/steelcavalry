﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{

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
			//TODO: Add explosion effect
			Destroy(gameObject);
        }


    }
}
