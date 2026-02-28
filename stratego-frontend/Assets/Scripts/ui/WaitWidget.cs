using TMPro;
using UnityEngine;

public class WaitWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI MessageText;
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		MessageText.text = "";
	}

	public void Activate(string message)
	{
		MessageText.text = message;
		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

}
