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


    private GameManager gameManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        UsernameText.text = "";
        Widget.SetActive(false);
        gameManager = FindFirstObjectByType<GameManager>();
        LogoutButton.onClick.RemoveAllListeners();
        LogoutButton.onClick.AddListener(LogoutClick);
    }

    private void LogoutClick()
    {
        gameManager.LeaveGame();
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
        
		Widget.SetActive(true);
		UsernameText.text = username;
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
