using TMPro;
using UnityEngine;

public class ChatItemWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TextItem;


	public void Init(string player, string message, Color textColor)
	{
		TextItem.text = $"{player} : {message}";
		TextItem.color = textColor;
		TextItem.alpha = 1f;
	}
}
