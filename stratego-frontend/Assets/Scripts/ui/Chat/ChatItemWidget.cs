using TMPro;
using UnityEngine;

public class ChatItemWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TextPlayer;
	[SerializeField]
	private TextMeshProUGUI TextMessage;


	public void Init(string player, string message, Color textColor)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
		
		TextPlayer.text = $"{player}: ";
		TextPlayer.color = textColor;
		TextPlayer.alpha = 1f;
		
		TextMessage.text = message;
		TextMessage.alpha = 1f;
	}
}
