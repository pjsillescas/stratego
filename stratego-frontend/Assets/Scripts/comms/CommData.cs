using UnityEngine;

public class CommData : MonoBehaviour
{
	private int gameId;
	private string token;
	private bool isHost;
	private string myUsername;
	private string opponentUsername;

	public void ResetData()
	{
		Debug.Log("resetdata");
		gameId = 0;
		token = null;
		isHost = false;
		myUsername = "";
		opponentUsername = "";
	}

	public void SetToken(string token)
	{
		this.token = token;
	}

	public string GetToken() => token;

	public void SetGameId(int gameId)
	{
		Debug.Log($"set gameid {gameId}");
		this.gameId = gameId;
	}

	public int GetGameId() => gameId;

	public void SetIsHost(bool isHost)
	{
		this.isHost = isHost;
	}

	public bool GetIsHost() => isHost;

	private static CommData instance = null;

	public static CommData GetInstance() => instance;

	public string GetMyUsername() => myUsername;
	public void SetMyUsername(string username)
	{
		myUsername = username;
	}

	public string GetOpponentUsername() => opponentUsername;
	public void SetOpponentUsername(string username)
	{
		opponentUsername = username;
	}
	
	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		instance = this;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
