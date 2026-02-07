using System;
using TMPro;
using UnityEngine;

public class ConsoleLogItem : MonoBehaviour
{
	private const int MAX_STRING_LENGTH = 100;
	
	[SerializeField]
	private TextMeshProUGUI TimeText;
	[SerializeField]
	private TextMeshProUGUI MessageText;

	private string message;
	public void AddMessage(string message)
	{
		this.message = message ?? "";
		TimeText.text = DateTime.Now.ToString("HH:mm:ss");
		MessageText.text = PrepareString(message ?? "");
	}

	private string PrepareString(string message)
	{
		return (message.Length > MAX_STRING_LENGTH) ? message[..MAX_STRING_LENGTH] : message;
	}
}
