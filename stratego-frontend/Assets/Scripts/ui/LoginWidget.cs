using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI UsernameText;
	[SerializeField]
	private TextMeshProUGUI PasswordText;

	[SerializeField]
	private TMP_InputField UsernameInput;
	[SerializeField]
	private TMP_InputField PasswordInput;

	[SerializeField]
	private TextMeshProUGUI ErrorText;

	[SerializeField]
	private Button LoginButton;
	[SerializeField]
	private Button SignupButton;

	[SerializeField]
	private GameListWidget GameListWidget;

	private BackendService backendService;

	private void OnError(StrategoErrorDTO errorDto)
	{
		ErrorText.enabled = true;

		ErrorText.text = errorDto.message;
	}

	private void DisableError()
	{
		ErrorText.enabled = false;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (CommData.GetInstance().GetToken() != null)
		{
			ActivateGameListWidget();
		}
		else
		{
			CommData.GetInstance().ResetData();
			DisableError();
			SignupButton.onClick.RemoveAllListeners();
			SignupButton.onClick.AddListener(ButtonSignupClick);
			LoginButton.onClick.RemoveAllListeners();
			LoginButton.onClick.AddListener(ButtonLoginClick);

			backendService = FindFirstObjectByType<BackendService>();
		}
	}

	private void ButtonSignupClick()
	{
		Debug.Log("signup click");
		StartCoroutine(backendService.Signup(UsernameInput.text, PasswordInput.text, OnSignedUp, OnError));
	}

	private void ButtonLoginClick()
	{
		Debug.Log("login click");
		StartCoroutine(backendService.Login(UsernameInput.text, PasswordInput.text, OnLoggedIn, OnError));
	}

	private void OnSignedUp(PlayerDTO player)
	{
		Debug.Log("onsignedup");
		CommData.GetInstance().SetMyUsername(player.username);
		DisableError();
		ButtonLoginClick();
	}

	private void OnLoggedIn(string token)
	{
		DisableError();
		CommData.GetInstance().SetMyUsername(UsernameInput.text);
		CommData.GetInstance().SetToken(token);
		Debug.Log($"token: {token}");
		ActivateGameListWidget();
	}

	private void ActivateGameListWidget()
	{
		GameListWidget.gameObject.SetActive(true);
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
