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

            SkinnedMeshRenderer bodyPartSkinnedMesh = bodyPart.GetComponent<SkinnedMeshRenderer>();
            MeshRenderer bodyPartMesh = bodyPart.GetComponent<MeshRenderer>();

            if (bodyPartSkinnedMesh == null && bodyPartMesh == null) continue;

            CircleCollider2D bodyPartCollider2D = bodyPart.GetComponent<CircleCollider2D>();
            bodyPartCollider2D = bodyPartCollider2D == null ? bodyPart.AddComponent<CircleCollider2D>() : bodyPartCollider2D;
            bodyPartCollider2D.enabled = true;

            Rigidbody2D bodyPartRb2D = bodyPart.GetComponent<Rigidbody2D>();
            bodyPartRb2D = bodyPartRb2D == null ? bodyPart.AddComponent<Rigidbody2D>() : bodyPartRb2D;
            bodyPartRb2D.AddForce(Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.left * Random.Range(expForceMin, expForceMax));

            bodyPart.transform.SetParent(null);            
        }
    }
}
