using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum AudioEvents
{
	MechExplosion,
	LaserBoltShot,
	DudeBoltShot,
}

[System.Serializable]
public class AudioEventEmiter
{
	[Tooltip("Used to give the name to the array item in the Inspector")]
	[HideInInspector] public string ReadOnlyName;
	public AudioEvents audioEvent;
	public AudioClip soundFile;
	public float volume = 1f;
	public int priority = 128;
	public float minPitch = 0.9f;
	public float maxPitch = 1.1f;
	public AnimationCurve volumeFalloff = new AnimationCurve(new Keyframe(0,1), new Keyframe(100, 0.1f));
	public int maxEventsAtOneTime = 10;
}

class AudioPlayer
{
	public AudioEventEmiter AudioEventEmiter;
	public AudioSource AudioSource;
}

public class AudioManager : MonoBehaviour
{
	[SerializeField] private Transform player = null;
	[SerializeField] private AudioEventEmiter[] emiters = null;

	private GameObject audioSources;
	private Dictionary<AudioEvents, List<List<AudioPlayer>>> audioPlayers = new Dictionary<AudioEvents, List<List<AudioPlayer>>>();

	void Start ()
	{
		Assert.IsNotNull( player );
		Assert.IsNotNull( emiters );
		Assert.AreNotEqual( emiters.Length, 0 );

		audioSources = new GameObject( "Audio Sources" );
		audioSources.transform.parent = transform;

		foreach ( var emiter in emiters )
		{
			List<AudioPlayer> list;
			if ( audioPlayers.ContainsKey( emiter.audioEvent ) )
			{
				var l = audioPlayers[emiter.audioEvent];

				list = new List<AudioPlayer>();
				l.Add( list );
			}
			else
			{
				var l = new List<List<AudioPlayer>>( );
				audioPlayers.Add( emiter.audioEvent, l );

				list = new List<AudioPlayer>( );
				l.Add( list );
			}

			GameObject sourcesGo = new GameObject( emiter.audioEvent.ToString( ) );
			sourcesGo.transform.parent = audioSources.transform;

			for ( int i = 0; i < emiter.maxEventsAtOneTime; i++ )
			{
				AudioPlayer ap = new AudioPlayer( );
				ap.AudioEventEmiter = emiter;

				var s = sourcesGo.AddComponent<AudioSource>( );
				s.clip = emiter.soundFile;
				s.volume = emiter.volume;
				s.priority = emiter.priority;
				s.playOnAwake = false;

				ap.AudioSource = s;

				list.Add( ap );
			}
		}
	}

	void OnValidate( )
	{
		foreach ( var emiter in emiters )
		{
			emiter.ReadOnlyName = emiter.audioEvent.ToString( );
		}
	}

	public void Play (AudioEvents audioEvent, Vector2 pos)
	{
		var eventAudioPlayersList = audioPlayers[audioEvent];
		var eventAudioPlayers = eventAudioPlayersList[Random.Range( 0, eventAudioPlayersList.Count )];

		foreach ( var ap in eventAudioPlayers )
		{
			if (!ap.AudioSource.isPlaying)
			{
				float distance = Vector2.Distance( player.transform.position, pos );
				var aE = ap.AudioEventEmiter;
				distance = aE.volumeFalloff.keys[aE.volumeFalloff.keys.Length - 1].value > distance ? aE.volumeFalloff.keys[aE.volumeFalloff.keys.Length - 1].value : distance;
				ap.AudioSource.volume = aE.volume * aE.volumeFalloff.Evaluate( distance );

				ap.AudioSource.pitch = Random.Range( ap.AudioEventEmiter.minPitch, ap.AudioEventEmiter.maxPitch );
				ap.AudioSource.Play( );

				break;
			}
		}
	}
}
