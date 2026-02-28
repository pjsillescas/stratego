using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameItemWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI DescriptionText;
	[SerializeField]
	private TextMeshProUGUI DateText;

	[SerializeField]
	private Button JoinButton;

	private int gameId;
	private Action<int> joinGame;

	private void Awake()
	{
		JoinButton.onClick.RemoveAllListeners();
		JoinButton.onClick.AddListener(JoinButtonClick);
	}

	public void Activate()
	{
		JoinButton.interactable = true;
	}

	public void Deactivate()
	{
		JoinButton.interactable = false;
	}

	private void JoinButtonClick()
	{
		joinGame(gameId);
	}

	public void SetGameData(GameDTO gameDto, Action<int> joinGame)
	{
		gameId = gameDto.id;
		DateText.text = DateTime.Parse(gameDto.creationDate).ToString("dd/MM/yyyy HH:mm:ss");
		DescriptionText.text = gameDto.name;
		this.joinGame = joinGame;
	}
}
