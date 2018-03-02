using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // So this script appears in Unity's context menu
public class GameEventAudioEvent : ScriptableObject
{
	// List of all GameEventListenerFloat that want to be notified when this event is fired
	private List<GameEventListenerAudioEvent> listeners = new List<GameEventListenerAudioEvent>();

	// Called when we, from our code, send an event
	public void Raise( AudioEvents eventType, Vector2 position )
	{
		// Back to front so we can unsubscribe from within the loop and still be ok
		for ( int i = listeners.Count - 1; i >= 0; i-- )
		{
			listeners[i].OnEventRaised( eventType, position ); // Notifies GameEventListenerFloat that this event has been fired
		}
	}

	public void Subscribe ( GameEventListenerAudioEvent listener )
	{
		listeners.Add( listener ); // This is the place GameEventListenerFloat adds itself
	}

	public void UnSubscribe( GameEventListenerAudioEvent listener )
	{
		listeners.Remove( listener ); // And removes itself
	}
}
