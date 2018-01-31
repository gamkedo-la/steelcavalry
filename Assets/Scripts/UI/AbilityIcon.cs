using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class AbilityIcon : MonoBehaviour
{
	[SerializeField] private UIEvent respondToEventOn;
	[SerializeField] private UIEvent respondToEventOff;
	[SerializeField] private CanvasGroup group;
	[SerializeField] private Image background;
	[SerializeField] private float disableState = 0.4f;

	void Start ()
	{
		Assert.IsNotNull( group );
		Assert.IsNotNull( background );

		group.alpha = disableState;
	}

	void Update ()
	{

	}

	public void SetState( UIEvent eventType, float percentFull )
	{
		if (eventType == respondToEventOn)
		{
			group.alpha = 1.0f;
			background.fillAmount = percentFull;
		}

		if ( eventType == respondToEventOff )
		{
			group.alpha = disableState;
			background.fillAmount = percentFull;
		}
	}
}
