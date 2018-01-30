using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // So this script appears in Unity's context menu
public class GameEventUI : ScriptableObject
{
	// List of all GameEventListenerFloat that want to be notified when this event is fired
	private List<GameEventListenerUI> listeners = new List<GameEventListenerUI>();

	// Called when we, from our code, send an event
	public void Raise( UIEvent parameter )
	{
		// Back to front so we can unsubscribe from within the loop and still be ok
		for ( int i = listeners.Count - 1; i >= 0; i-- )
		{
			listeners[i].OnEventRaised( parameter ); // Notifies GameEventListenerFloat that this event has been fired
		}
	}

	public void Subscribe ( GameEventListenerUI listener )
	{
		listeners.Add( listener ); // This is the place GameEventListenerFloat adds itself
	}

	public void UnSubscribe( GameEventListenerUI listener )
	{
		listeners.Remove( listener ); // And removes itself
	}
}
