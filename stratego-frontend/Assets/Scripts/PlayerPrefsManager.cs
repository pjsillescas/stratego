using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPrefsManager
{
	private const string TOKEN_KEY = "stratego-token";
	private const string GAME_ID_KEY = "stratego-game-id";
	private const string IS_HOST_KEY = "is-host-id";

	public static void Reset()
	{
		Debug.Log($"Reset playerprefs");
		PlayerPrefs.DeleteAll();
	}

	public static void SetToken(string token)
	{
		PlayerPrefs.SetString(TOKEN_KEY, token);
	}

	public static string GetToken()
	{
		return PlayerPrefs.GetString(TOKEN_KEY);
	}

	public static void SetGameId(int gameId)
	{
		Debug.Log($"set game id {gameId}");
		PlayerPrefs.SetInt(GAME_ID_KEY, gameId);
	}
	public static int GetGameId()
	{
		var gid = PlayerPrefs.GetInt(GAME_ID_KEY);
		Debug.Log($"get gameid {gid}");
		return PlayerPrefs.GetInt(GAME_ID_KEY);
	}
	public static void SetIsHost(bool isHost)
	{
		PlayerPrefs.SetInt(IS_HOST_KEY, isHost ? 1 : 0);
	}
	public static bool GetIsHost()
	{
		return PlayerPrefs.GetInt(IS_HOST_KEY) == 1;
	}
}
