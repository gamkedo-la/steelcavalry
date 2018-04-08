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

	private Image icon;
	private bool isOn = false;

	void Start ()
	{
		Assert.IsNotNull( group );
		Assert.IsNotNull( background );

		group.alpha = disableState;

		icon = transform.Find("Icon").GetComponent<Image>();
	}

	public void SetStateDefault( )
	{
		group.alpha = 1.0f;
		background.fillAmount = 1;
		isOn = true;
	}

	public void SetState( UIEvent eventType, float percentFull )
	{
		if (eventType == respondToEventOn)
		{
			group.alpha = 1.0f;
			background.fillAmount = percentFull;
			isOn = true;
		}

		if ( eventType == respondToEventOff )
		{
			group.alpha = disableState;
			background.fillAmount = percentFull;
			isOn = false;
		}
	}

	public void SetIcon(string iconName) {
		string path = "Sprites/" + iconName;
		Sprite spriteToUse = Resources.Load<Sprite>(path);
		if(icon) {
			icon.sprite = spriteToUse;
		} else {
			Debug.LogWarning("ability icon " + iconName + " not found");
		}

		if ( isOn ) group.alpha = 1.0f;
	}
}
