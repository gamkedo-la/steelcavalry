using UnityEngine;
using UnityEngine.Assertions;

public class FadePart : MonoBehaviour
{
	[SerializeField] private float waitTime = 2f;
	[SerializeField] private float fadePerSecond = 0.5f;
	[SerializeField] private bool destroyAtTheEnd = true;
    [SerializeField] private float distanceYtoFade = 100f;

	private Material material;
	private float alpha = 1f;
    private MechBodyParts mechBodyParts;
    private Vector3 impactStartPosition;
    private bool canExplodeIn3D = false;
    private bool isAffectedByGravity = true;

	void Start ()
	{
		material = GetComponent<Renderer>( ).material;
		Assert.IsNotNull( material );

        mechBodyParts = GetComponent<MechBodyParts>();
        impactStartPosition = transform.position;
	}

	void Update( )
	{        
        if (canExplodeIn3D && 
            isAffectedByGravity &&
            Mathf.Abs(impactStartPosition.y - transform.position.y) > distanceYtoFade) {
            Fade();
        }        

        if (!canExplodeIn3D) {
            Fade();
        }
	}
    
    public void EnableFade(bool canExplodeIn3D, bool isAffectedByGravity) {
        this.canExplodeIn3D = canExplodeIn3D;
        this.isAffectedByGravity = isAffectedByGravity;
    }

    void Fade() {
        waitTime -= Time.deltaTime;
        if (waitTime > 0) return;

        var color = material.color;
        alpha = alpha - (fadePerSecond * Time.deltaTime);

        material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);

        if (alpha <= 0 && destroyAtTheEnd) {
            Destroy(gameObject);
        }
        else if (alpha <= 0) {
            enabled = false;
        }
    }
}
