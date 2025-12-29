using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BackendService: MonoBehaviour
{
	private static string URL = "http://127.0.0.1:8080/api";

	private static BackendService instance;

	private void Start()
	{
		if (instance != null)
		{
			Debug.Log("there is more than one backend service");
		}
		
		instance = this;
	}

	public IEnumerator Login(string username, string password, Action<string> onLoggedIn)
	{
		string body = JsonUtility.ToJson(new LoginDTO(username, password));

		using UnityWebRequest request = UnityWebRequest.Put(URL + "/login", body);
		request.SetRequestHeader("Accept", "application/json");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var result = JsonUtility.FromJson<LoginResultDTO>(json);
			onLoggedIn?.Invoke(result.token);
		}
		else
		{

			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}

	public IEnumerator Signup(string username, string password, Action<PlayerDTO> onSignedUp)
	{
		string body = JsonUtility.ToJson(new LoginDTO(username, password));

		using UnityWebRequest request = UnityWebRequest.Put(URL + "/signup", body);
		request.SetRequestHeader("Accept", "application/json");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Signed up. Received: " + json);

			var result = JsonUtility.FromJson<PlayerDTO>(json);
			onSignedUp?.Invoke(result);
		}
		else
		{

			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}

	public IEnumerator GetGameList(string token, Action<List<GameDTO>> onGamesGot)
	{
		using UnityWebRequest request = UnityWebRequest.Get(URL + "/game");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var gameList = JsonUtility.FromJson<GameListDTO>("{\"games\":" + json + "}");
			onGamesGot?.Invoke(gameList.games);
		}
		else
		{

			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}

	private string GetJoinCode()
	{
		const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

		int charAmount = 10; //set those to the minimum and maximum length of your string
		var myString = "";
		for (int i = 0; i < charAmount; i++)
		{
			myString += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
		}

		return myString;
	}

	public IEnumerator CreateGame(string token, Action<GameDTO> onLoggedIn)
	{
		var data = JsonUtility.ToJson(new GameInputDTO(GetJoinCode()));
		using UnityWebRequest request = UnityWebRequest.Put(URL + "/game", data);
		request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var gameDto = JsonUtility.FromJson<GameDTO>(json);
			onLoggedIn?.Invoke(gameDto);
		}
		else
		{
			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}

	public IEnumerator JoinGame(int gameId, string token, Action<GameExtendedDTO> onJoinedGame)
	{
		var data = JsonUtility.ToJson(new GameInputDTO(GetJoinCode()));
		using UnityWebRequest request = UnityWebRequest.Post(URL + $"/game/{gameId}/join", new WWWForm());
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var gameExtendedDto = JsonUtility.FromJson<GameExtendedDTO>(json);
			onJoinedGame?.Invoke(gameExtendedDto);
		}
		else
		{
			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}

	public IEnumerator LeaveGame(int gameId, string token, Action<GameDTO> onLeftGame)
	{
		var data = JsonUtility.ToJson(new GameInputDTO(GetJoinCode()));
		using UnityWebRequest request = UnityWebRequest.Post(URL + $"/game/{gameId}/leave", new WWWForm());
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var gameDto = JsonUtility.FromJson<GameDTO>(json);
			onLeftGame?.Invoke(gameDto);
		}
		else
		{
			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}

	public IEnumerator AddSetup(int gameId, string token, List<List<Rank>> setup, Action<GameStateDTO> onSetupAdded)
	{
		var data = JsonUtility.ToJson(new ArmySetupDTO(setup));
		using UnityWebRequest request = UnityWebRequest.Put(URL + $"/stratego/{gameId}/setup", data);
		request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var gameStateDto = JsonUtility.FromJson<GameStateDTO>(json);
			onSetupAdded?.Invoke(gameStateDto);
		}
		else
		{
			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}

	public IEnumerator AddMovement(int gameId, string token, StrategoMovementDTO movement, Action<GameStateDTO> onMovementAdded)
	{
		var data = JsonUtility.ToJson(movement);
		using UnityWebRequest request = UnityWebRequest.Put(URL + $"/stratego/{gameId}/setup", data);
		request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var gameStateDto = JsonUtility.FromJson<GameStateDTO>(json);
			onMovementAdded?.Invoke(gameStateDto);
		}
		else
		{
			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}

	public IEnumerator GetStatus(int gameId, string token, Action<GameStateDTO> onStatusGot)
	{
		using UnityWebRequest request = UnityWebRequest.Get(URL + $"/stratego/{gameId}/status");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var gameStateDto = JsonUtility.FromJson<GameStateDTO>(json);
			onStatusGot?.Invoke(gameStateDto);
		}
		else
		{
			Debug.LogError("Failed to load JSON: " + request.error);
		}
	}
}
