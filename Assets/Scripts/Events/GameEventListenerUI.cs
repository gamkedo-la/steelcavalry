using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[System.Serializable]
public enum UIEvent
{
	TurretOn,
	TurretOff,
	LauncherOn,
	LauncherOff,
	ThusterOn,
	ThusterOff,
	ThrowerOn,
	ThrowerOff,
}

[System.Serializable] // So it shows in the inspector
public class UnityEventUI : UnityEvent<UIEvent> { } // We inherit from UnityEvent so we can pass a float

public class GameEventListenerUI : MonoBehaviour
{
	[SerializeField] private GameEventUI gameEvent; // Game event we will listen to
	[SerializeField] private UnityEventUI response;	// Normal Unity event (the one we created above) to call our own code

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
	public void OnEventRaised ( UIEvent parameter )
	{
		response.Invoke( parameter );
	}
}
