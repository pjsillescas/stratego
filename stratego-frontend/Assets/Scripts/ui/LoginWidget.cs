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
	private Button LoginButton;
	[SerializeField]
	private Button SignupButton;

	private BackendService backendService;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		SignupButton.onClick.AddListener(ButtonSignupClick);
		LoginButton.onClick.AddListener(ButtonLoginClick);

		backendService = FindFirstObjectByType<BackendService>();
	}

	private void ButtonSignupClick()
	{
		Debug.Log("signup click");
		StartCoroutine(backendService.Signup(UsernameInput.text, PasswordInput.text, OnSignedUp));
	}

	private void ButtonLoginClick()
	{
		Debug.Log("login click");
		StartCoroutine(backendService.Login(UsernameInput.text, PasswordInput.text, OnLoggedIn));
	}

	private void OnSignedUp(PlayerDTO player)
	{
		Debug.Log("onsignedup");
		ButtonLoginClick();
	}

	private void OnLoggedIn(string token)
	{
		Debug.Log($"token: {token}");
	}

	// Update is called once per frame
	void Update()
	{

	}
}
