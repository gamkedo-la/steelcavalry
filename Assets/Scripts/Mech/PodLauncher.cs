using UnityEngine;
using UnityEngine.Assertions;

public class PodLauncher : MonoBehaviour {

    public Mech myMech;
    public float launchPower;

    private bool isActive = false;
    //private bool isRight;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void HandleFire()
    {
        // if (spawnPoint == null)
        //   return;

        LaunchPod();
    }

    public void SetDir(bool isRight)
    {
        //this.isRight = isRight;
    }

    public void Active(bool isActive)
    {
        this.isActive = isActive;
    }

    protected void LaunchPod() {

        if (!isActive)
            return;
        // IF pod hasn't been launched yet...
        if (myMech.podLaunched)
            return;
        Debug.Log("Pod Fired");
        Rigidbody2D podRb = myMech.pod.GetComponent<Rigidbody2D>();
        // Activate pod RB
        podRb.simulated = true;
        //Fire the pod and disable movement
        podRb.AddRelativeForce(Vector2.right * launchPower, ForceMode2D.Impulse);
        myMech.podLaunched = true;

        //myMech.pods.rigidbody2

    }

}
