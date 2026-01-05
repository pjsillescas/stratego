using UnityEngine;

public class CommData : MonoBehaviour
{
	private int gameId;
	private string token;
	private bool isHost;

	public void ResetData()
	{
		Debug.Log("resetdata");
		gameId = 0;
		token = "";
		isHost = false;
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
