using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class MechUI : MonoBehaviour
{
	//[SerializeField] private Transform objToFollow = null;
	[SerializeField] private Image hpBar = null;
	[SerializeField] private Text nameText = null;

	void Start ()
	{
		//Assert.IsNotNull( objToFollow );
		Assert.IsNotNull( nameText );
	}

	void LateUpdate ()
	{
		//transform.position = objToFollow.position;
		transform.rotation = Quaternion.identity;
	}

	public void SetHP( float value )
	{
		hpBar.fillAmount = value;
	}

	public void SetName( string value )
	{
		nameText.text = value;
	}
}
