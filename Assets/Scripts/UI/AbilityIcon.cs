using UnityEngine;
using UnityEngine.Assertions;

public class AbilityIcon : MonoBehaviour
{
	[SerializeField] private UIEvent respondToEventOn;
	[SerializeField] private UIEvent respondToEventOff;
	[SerializeField] private CanvasGroup group;
	[SerializeField] private float disableState = 0.4f;

	void Start ()
	{
		Assert.IsNotNull( group );

		group.alpha = disableState;
	}

	void Update ()
	{

	}

	public void SetState( UIEvent eventType )
	{
		if (eventType == respondToEventOn)
		{
			group.alpha = 1.0f;
		}

		if ( eventType == respondToEventOff )
		{
			group.alpha = disableState;
		}
	}
}
