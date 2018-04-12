using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MechBodyParts : MonoBehaviour {
    [SerializeField] private float screenshakePower = 15f;
    [SerializeField] private bool canExplodeIn3D = false;
    [SerializeField] private bool isAffectedByGravity = true;
    
    Transform[] bodyPartsTransforms;

	// Use this for initialization
	void Start () {
        bodyPartsTransforms = GetComponentsInChildren<Transform>();
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("Space")) {
            isAffectedByGravity = false;
        }

        if (sceneName.Contains("Base")) {
            canExplodeIn3D = false;
        }
    }
    
    public void MakeBodyParts (float expForceMin, float expForceMax) {
        Camera.main.GetComponent<MainCamera>().ShakeTheCam(screenshakePower);

        for (int i = 0; i < bodyPartsTransforms.Length; i++) {
			GameObject bodyPart = bodyPartsTransforms[i].gameObject;

			SkinnedMeshRenderer bodyPartSkinnedMesh = bodyPart.GetComponent<SkinnedMeshRenderer> ();
			MeshRenderer bodyPartMesh = bodyPart.GetComponent<MeshRenderer> ();

			if (bodyPartSkinnedMesh == null && bodyPartMesh == null)
				continue;

			bodyPart.layer = 12;

            // turn off any animation
            Animator anim = bodyPart.GetComponent<Animator>();
            if (anim) anim.enabled = false;

            // these circles are wonky - remove:
            Collider2D cCollider2D = bodyPart.GetComponent<Collider2D>();
            if (cCollider2D)
                Destroy(bodyPart.GetComponent<Collider2D>());

            Bounds boundsOfMesh = new Bounds();

            if (bodyPartSkinnedMesh != null) {
                boundsOfMesh.Encapsulate(bodyPartSkinnedMesh.GetComponent<SkinnedMeshRenderer>().bounds);
            }
            else if (bodyPartMesh != null) {
                boundsOfMesh.Encapsulate(bodyPartMesh.GetComponent<Renderer>().bounds);
            }

            if (canExplodeIn3D) {
                SphereCollider bodyPartCollider = bodyPart.GetComponent<SphereCollider>();
                bodyPartCollider = bodyPartCollider == null ? bodyPart.AddComponent<SphereCollider>() : bodyPartCollider;
                bodyPartCollider.enabled = true;
                bodyPartCollider.bounds.Encapsulate(boundsOfMesh);

                Rigidbody bodyPartRb = bodyPart.GetComponent<Rigidbody>();
                bodyPartRb = bodyPartRb == null ? bodyPart.AddComponent<Rigidbody>() : bodyPartRb;
                bodyPartRb.AddForce(Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.left * Random.Range(expForceMin, expForceMax));
                                
                bodyPartRb.useGravity = isAffectedByGravity;
                
                bodyPartRb.drag = 1.0f;
                bodyPartRb.angularDrag = 0.05f; 

                bodyPart.transform.SetParent(null);
            }
            else {
                // a nice tightly fit bbox (will only fit tightly to parts that are mostly axis aligned)
                // works nicely with shapes like "|" or "---" but not something L shaped or like a diagonal line /
                BoxCollider2D bodyPartCollider2D = bodyPart.GetComponent<BoxCollider2D>();
                bodyPartCollider2D = bodyPartCollider2D == null ? bodyPart.AddComponent<BoxCollider2D>() : bodyPartCollider2D;
                bodyPartCollider2D.enabled = true;
                bodyPartCollider2D.bounds.Encapsulate(boundsOfMesh);                

                Rigidbody2D bodyPartRb2D = bodyPart.GetComponent<Rigidbody2D>();
                bodyPartRb2D = bodyPartRb2D == null ? bodyPart.AddComponent<Rigidbody2D>() : bodyPartRb2D;
                bodyPartRb2D.AddForce(Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.left * Random.Range(expForceMin, expForceMax));
                bodyPartRb2D.drag = 0.5f;
                bodyPartRb2D.angularDrag = 0.9f; // hmmmmmmmm
                bodyPartRb2D.angularVelocity = 0f;
                bodyPartRb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // stops wild spins? no

                bodyPartRb2D.transform.SetParent(null);
            }

            FadePart fp = bodyPart.GetComponent<FadePart>();
            fp = fp == null ? bodyPart.AddComponent<FadePart>() : fp;
            if (fp != null) {
                fp.enabled = true;
                fp.EnableFade(canExplodeIn3D, isAffectedByGravity);
            }
        }
    }
}
