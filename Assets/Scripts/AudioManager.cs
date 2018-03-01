using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum AudioEvents
{
	MechExplosion,
}

[System.Serializable]
public class AudioEventEmiter
{
	public AudioEvents audioEvent;
	public AudioClip soundFile;
	public float volume = 1f;
	public int priority = 128;
	public AnimationCurve volumeFalloff = new AnimationCurve(new Keyframe(0,1), new Keyframe(100, 0.1f));
	public int maxEventsAtOneTime = 10;
}

public class AudioManager : MonoBehaviour
{
	[SerializeField] private Transform player = null;
	[SerializeField] private AudioEventEmiter[] emiters = null;

	private GameObject audioSources;
	private Dictionary<AudioEvents, List<AudioSource>> audioPlayers = new Dictionary<AudioEvents, List<AudioSource>>();

	void Start ()
	{
		Assert.IsNotNull( player );
		Assert.IsNotNull( emiters );
		Assert.AreNotEqual( emiters.Length, 0 );

		audioSources = new GameObject( "Audio Sources" );
		audioSources.transform.parent = transform;

		foreach ( var emiter in emiters )
		{
			audioPlayers.Add( emiter.audioEvent, new List<AudioSource>( ) );
			var sources = audioPlayers[emiter.audioEvent];
			GameObject sourcesGo = new GameObject( emiter.audioEvent.ToString( ) );
			sourcesGo.transform.parent = audioSources.transform;

			for ( int i = 0; i < emiter.maxEventsAtOneTime; i++ )
			{
				var s = sourcesGo.AddComponent<AudioSource>( );
				s.clip = emiter.soundFile;
				s.volume = emiter.volume;
				s.priority = emiter.priority;
				s.playOnAwake = false;

				sources.Add( s );
			}
		}

		Invoke( "PlayTest", 1f );
		Invoke( "PlayTest", 2f );
		Invoke( "PlayTest", 3f );
	}

	private void PlayTest()
	{
		Play( AudioEvents.MechExplosion, new Vector2( Random.Range(0,300), Random.Range( 0, 300 ) ) );
	}

	public void Play (AudioEvents audioEvent, Vector2 pos)
	{
		var sources = audioPlayers[audioEvent];

		foreach ( var s in sources )
		{
			if (!s.isPlaying)
			{
				float distance = Vector2.Distance( player.transform.position, pos );
				var aE = emiters.Select( x => x ).Where( x => x.audioEvent == audioEvent ).First( );
				distance = aE.volumeFalloff.keys[aE.volumeFalloff.keys.Length - 1].value > distance ? aE.volumeFalloff.keys[aE.volumeFalloff.keys.Length - 1].value : distance;
				Debug.Log( distance );
				s.volume = aE.volume * aE.volumeFalloff.Evaluate( distance );

				s.Play( );

				break;
			}
		}
	}
}
