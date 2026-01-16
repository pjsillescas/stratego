using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UsernameWidget : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI UsernameText;
	[SerializeField]
	private Button LogoutButton;

	[SerializeField]
	private GameObject Widget;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        UsernameText.text = "";
        Widget.SetActive(false);
        LogoutButton.onClick.RemoveAllListeners();
        LogoutButton.onClick.AddListener(LogoutClick);
    }

    private void LogoutClick()
    {
        var commData = CommData.GetInstance();

        if (commData.GetGameId() != 0)
        {
            var backendService = FindFirstObjectByType<BackendService>();
            var gameId = commData.GetGameId();
            var token = commData.GetToken();
            StartCoroutine(backendService.LeaveGame(gameId, token, OnLeftGame, OnError));
        }
        else
        {
            OnLeftGame(null);
        }
    }

    private void OnLeftGame(GameDTO gameDTO)
    {
        CommData.GetInstance().ResetData();
        Debug.Log("go to Login");
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    private void OnError(StrategoErrorDTO error)
    {
        Debug.Log(error.message);
    }

	private void OnEnable()
	{
		StartCoroutine(WaitForNameCoroutine());
		Widget.SetActive(false);
	}

	private IEnumerator WaitForNameCoroutine()
    {
        var waitForSeconds = new WaitForSeconds(0.3f);
		string username;
        do
        {
            yield return waitForSeconds;
            username = CommData.GetInstance().GetMyUsername();
		}
        while (username == "");
        
        Debug.Log($"username {username}");
		Widget.SetActive(true);
		UsernameText.text = username;
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
