using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	private List<GameEventListener> listeners = new List<GameEventListener>();

	public void Raise( )
	{
		for ( int i = listeners.Count - 1; i >= 0; i-- )
		{
			listeners[i].OnEventRaised( );
		}
	}

	public void Subscribe ( GameEventListener listener )
	{
		listeners.Add( listener );
	}

	public void UnSubscribe( GameEventListener listener )
	{
		listeners.Remove( listener );
	}
}
