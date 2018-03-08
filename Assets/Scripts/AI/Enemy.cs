using UnityEngine;

//Being an enemy implies that you cannot be boarded/controlled by player in any way.
//You are merely a thing that fights and can be killed, but NOT a mech

public class Enemy : MonoBehaviour {

	//public int maxHealth;
	//[SerializeField] private int currentHealth;

	public Targeting targeting; //the component that handles player detection/field of view
	private Vector2 aimingAt; //updated when currently firing

	//public event Action Fire = delegate {} ; //firing is heard by ShotLaunchers

	public ShotLauncher[] launchers;


	// Use this for initialization
	void Start () {
		//currentHealth = maxHealth;
	}

	// Update is called once per frame
	void Update () {
		if (targeting.currentTarget){
			aimingAt = new Vector2 (targeting.currentTarget.transform.position.x, targeting.currentTarget.transform.position.y);
			CycleLaunchers();
		}
	}

	//if a launcher is ready to fire and we have a target, we shoot
	void CycleLaunchers(){
		foreach (ShotLauncher launcher in launchers){
			if (launcher.CheckIfReady()){
				launcher.Fire(aimingAt);
			}
		}
	}
}
