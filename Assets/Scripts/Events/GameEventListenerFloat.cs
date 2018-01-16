using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventFloat : UnityEvent<float> {}

public class GameEventListenerFloat : MonoBehaviour
{
	[SerializeField] private GameEventFloat gameEvent;
	[SerializeField] private UnityEventFloat response;

	void Start ()
	{
		Assert.IsNotNull( gameEvent );
	}

	private void OnEnable( )
	{
		gameEvent.Subscribe( this );
	}

	private void OnDisable( )
	{
		gameEvent.UnSubscribe( this );
	}

	public void OnEventRaised ( float parameter )
	{
		response.Invoke( parameter );
	}
}
