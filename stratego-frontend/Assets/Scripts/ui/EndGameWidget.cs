using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI EndGameText;
	[SerializeField]
	private Button MainMenuButton;

	private GameManager gameManager;
	private bool isHost;
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		gameObject.SetActive(false);
		
		gameManager = FindFirstObjectByType<GameManager>();
		isHost = gameManager.GetIsHost();
		MainMenuButton.onClick.RemoveAllListeners();
		MainMenuButton.onClick.AddListener(MainMenuClick);
	}

	public void Activate(bool isHost)
	{
		gameObject.SetActive(true);
		if(isHost == this.isHost)
		{
			EndGameText.text = "You Lost";
		}
		else
		{
			EndGameText.text = "You Won";
		}

		gameManager.DisableGame();
	}

	private void MainMenuClick()
	{
		SceneManager.LoadScene("MainMenu");
	}

	// Update is called once per frame
	void Update()
	{

	}
}
