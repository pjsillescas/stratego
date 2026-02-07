using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI UsernameText;
	[SerializeField]
	private TextMeshProUGUI PasswordText;
	[SerializeField]
	private Toggle ShowPasswordToggle;

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

	private BackendService backendService;
	private InputActions inputActions;

	private int selectedUI;
	private GameObject[] uiElements;

	private void OnError(StrategoErrorDTO errorDto)
	{
		ErrorText.enabled = true;

		ErrorText.text = errorDto.message;
	}

	private void DisableError()
	{
		ErrorText.enabled = false;
	}

	private void Awake()
	{
		inputActions = new InputActions();
		uiElements = new GameObject[4] {
			UsernameInput.gameObject,
			PasswordInput.gameObject,
			LoginButton.gameObject,
			SignupButton.gameObject,
		};
	}

	private void OnEnable()
	{
		inputActions.Enable();
	}

	private void OnDisable()
	{
		inputActions.Disable();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		SelectUIElement(0);

		if (CommData.GetInstance().GetToken() != null)
		{
			GoToMainMenu();
		}
		else
		{
			CommData.GetInstance().ResetData();
			DisableError();
			SignupButton.onClick.RemoveAllListeners();
			SignupButton.onClick.AddListener(ButtonSignupClick);
			LoginButton.onClick.RemoveAllListeners();
			LoginButton.onClick.AddListener(ButtonLoginClick);
			ShowPasswordToggle.isOn = false;
			DoChangePasswordInput(ShowPasswordToggle.isOn);
			ShowPasswordToggle.onValueChanged.RemoveAllListeners();
			ShowPasswordToggle.onValueChanged.AddListener(OnShowPasswordToggleChanged);

			backendService = FindFirstObjectByType<BackendService>();
		}
	}

	private void DoChangePasswordInput(bool isShowPassword)
	{
		PasswordInput.contentType = (isShowPassword) ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
	}

	private void OnShowPasswordToggleChanged(bool isShowPassword)
	{
		DoChangePasswordInput(isShowPassword);
		PasswordInput.ActivateInputField();
	}

	private void ButtonSignupClick()
	{
		StartCoroutine(backendService.Signup(UsernameInput.text, PasswordInput.text, OnSignedUp, OnError));
	}

	private void ButtonLoginClick()
	{
		StartCoroutine(backendService.Login(UsernameInput.text, PasswordInput.text, OnLoggedIn, OnError));
	}

	private void OnSignedUp(PlayerDTO player)
	{
		CommData.GetInstance().SetMyUsername(player.username);
		DisableError();
		ButtonLoginClick();
	}

	private void OnLoggedIn(string token)
	{
		DisableError();
		CommData.GetInstance().SetMyUsername(UsernameInput.text);
		CommData.GetInstance().SetToken(token);
		GoToMainMenu();
	}

	private void GoToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	// Update is called once per frame
	void Update()
	{
		if (inputActions.UI.ChangeUI.WasPressedThisFrame())
		{
			ChangeUI();
		}
	}

	private void ChangeUI()
	{
		SelectUIElement((selectedUI + 1) % uiElements.Count());
	}

	private void SelectUIElement(int newElement)
	{
		selectedUI = newElement;
		var selectedElement = uiElements[selectedUI];
		EventSystem.current.SetSelectedGameObject(selectedElement, null);

		if (selectedElement.TryGetComponent(out TMP_InputField input))
		{
			input.ActivateInputField();
		}
	}
}
