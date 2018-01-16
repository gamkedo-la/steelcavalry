using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	[SerializeField] private GameEvent gameEvent;
	[SerializeField] private UnityEvent response;

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

	public void OnEventRaised ()
	{
		response.Invoke( );
	}
}
