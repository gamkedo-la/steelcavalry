using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBodyParts : MonoBehaviour {
    Transform[] bodyPartsTransforms;


	// Use this for initialization
	void Start () {
        bodyPartsTransforms = GetComponentsInChildren<Transform>();
	}

	// Update is called once per frame
	void Update () {

	}

    public void MakeBodyParts (float expForceMin, float expForceMax) {
		for (int i = 0; i < bodyPartsTransforms.Length; i++) {
			GameObject bodyPart = bodyPartsTransforms[i].gameObject;

			SkinnedMeshRenderer bodyPartSkinnedMesh = bodyPart.GetComponent<SkinnedMeshRenderer> ();
			MeshRenderer bodyPartMesh = bodyPart.GetComponent<MeshRenderer> ();

			if (bodyPartSkinnedMesh == null && bodyPartMesh == null)
				continue;

			bodyPart.layer = 12;

			Bounds boundsOfMesh = new Bounds ();

			if (bodyPartSkinnedMesh != null) {
				boundsOfMesh.Encapsulate(bodyPartSkinnedMesh.GetComponent<SkinnedMeshRenderer>().bounds);
			} else if (bodyPartMesh != null) {
				boundsOfMesh.Encapsulate(bodyPartMesh.GetComponent<Renderer>().bounds);
			}

			// a nice tightly fit bbox (will only fit tightly to parts that are mostly axis aligned)
			// works nicely with shapes like "|" or "---" but not something L shaped or like a diagonal line /
			BoxCollider2D bodyPartCollider2D = bodyPart.GetComponent<BoxCollider2D>();
			bodyPartCollider2D = bodyPartCollider2D == null ? bodyPart.AddComponent<BoxCollider2D>() : bodyPartCollider2D;
			bodyPartCollider2D.enabled = true;
			bodyPartCollider2D.bounds.Encapsulate(boundsOfMesh);

			// these circles are wonky - remove:
			CircleCollider2D cCollider2D = bodyPart.GetComponent<CircleCollider2D>();
			if (cCollider2D)
				cCollider2D.enabled = false;

			FadePart fp = bodyPart.GetComponent<FadePart>( );
			if ( fp != null ) fp.enabled = true;

			// turn off any animation
			Animator anim = bodyPart.GetComponent<Animator>();
			if (anim) anim.enabled = false;

            Rigidbody2D bodyPartRb2D = bodyPart.GetComponent<Rigidbody2D>();
            bodyPartRb2D = bodyPartRb2D == null ? bodyPart.AddComponent<Rigidbody2D>() : bodyPartRb2D;
            bodyPartRb2D.AddForce(Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.left * Random.Range(expForceMin, expForceMax));
			bodyPartRb2D.drag = 0.01f;
			bodyPartRb2D.angularDrag = 0.0f; // hmmmmmmmm
			bodyPartRb2D.angularVelocity = 0f;
			bodyPartRb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // stops wild spins? no

			bodyPart.transform.SetParent (null);

        }
    }
}
