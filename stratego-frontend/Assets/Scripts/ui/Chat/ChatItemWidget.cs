using TMPro;
using UnityEngine;

public class ChatItemWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TextItem;


	public void Init(string text, Color textColor)
	{
		TextItem.text = text;
		TextItem.color = textColor;
		TextItem.alpha = 1f;
	}
}
