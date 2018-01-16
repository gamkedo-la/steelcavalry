using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEventFloat : ScriptableObject
{
	private List<GameEventListenerFloat> listeners = new List<GameEventListenerFloat>();

	public void Raise( float parameter )
	{
		for ( int i = listeners.Count - 1; i >= 0; i-- )
		{
			listeners[i].OnEventRaised( parameter );
		}
	}

	public void Subscribe ( GameEventListenerFloat listener )
	{
		listeners.Add( listener );
	}

	public void UnSubscribe( GameEventListenerFloat listener )
	{
		listeners.Remove( listener );
	}
}
