using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[System.Serializable] // So it shows in the inspector
public class UnityEventFloat : UnityEvent<float> { } // We inherit from UnityEvent so we can pass a float

public class GameEventListenerFloat : MonoBehaviour
{
	[SerializeField] private GameEventFloat gameEvent; // Game event we will listen to
	[SerializeField] private UnityEventFloat response;	// Normal Unity event (the one we created above) to call our own code

	void Start ()
	{
		Assert.IsNotNull( gameEvent ); // Just making sure the event is set up in the inspector
	}

	private void OnEnable( )
	{
		// We register ourselves with UnityEventFloat - we want to be notified when the event is called
		gameEvent.Subscribe( this );
	}

	private void OnDisable( )
	{
		gameEvent.UnSubscribe( this );  // And now we unregister ourselves
	}

	// This is called by UnityEventFloat when the event is fired
	public void OnEventRaised ( float parameter )
	{
		response.Invoke( parameter );
	}
}
