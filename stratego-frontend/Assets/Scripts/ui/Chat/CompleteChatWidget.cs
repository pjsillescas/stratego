using UnityEngine;
using UnityEngine.UI;

public class CompleteChatWidget : MonoBehaviour
{
	[SerializeField]
	private Color NormalButtonColor;
	[SerializeField]
	private Color MessageButtonColor;

	[SerializeField]
	private Button ToggleChatButton;
	[SerializeField]
	private CanvasGroup ChatGroup;

	private bool isShowChat;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Hide();
		SetButtonColor(NormalButtonColor);
		ToggleChatButton.onClick.RemoveAllListeners();
		ToggleChatButton.onClick.AddListener(ToggleChatClick);
	}

	private void OnEnable()
	{
		ChatWidget.OnMessageReceived += OnMessageReceived;
	}

	private void OnDisable()
	{
		ChatWidget.OnMessageReceived -= OnMessageReceived;
	}

	private void OnMessageReceived(object sender, string message)
	{
		if (!isShowChat)
		{
			SetButtonColor(MessageButtonColor);
		}
	}


	private void ToggleChatClick()
	{
		SetButtonColor(NormalButtonColor);
		if (isShowChat)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	private void SetButtonColor(Color color)
	{
		ToggleChatButton.GetComponent<Image>().color = color;
	}

	private void Hide()
	{
		isShowChat = false;
		ChatGroup.alpha = 0f;
		ChatGroup.blocksRaycasts = false;
	}

	private void Show()
	{
		isShowChat = true;
		ChatGroup.alpha = 1f;
		ChatGroup.blocksRaycasts = true;
	}

	// Update is called once per frame
	void Update()
	{

	}
}
