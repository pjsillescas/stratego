using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BackendService: MonoBehaviour
{
	//private const string URL = "http://192.168.1.14:8080/api";
	private const string URL = "http://localhost:8080/api"; // for builds
	//private const string URL = "http://192.168.1.12:8080/api";

	private static BackendService instance = null;
	public static BackendService GetInstance() => instance;

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

	public IEnumerator Login(string username, string password, Action<string> onLoggedIn, Action<StrategoErrorDTO> onError)
	{
		string body = JsonUtility.ToJson(new LoginDTO(username, password));
		
		using UnityWebRequest request = UnityWebRequest.Put(URL + "/auth/login", body);
		//request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Content-Type", "application/json");
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
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to login: " + request.error);
			
			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

	public IEnumerator Signup(string username, string password, Action<PlayerDTO> onSignedUp, Action<StrategoErrorDTO> onError)
	{
		string body = JsonUtility.ToJson(new LoginDTO(username, password));

		Debug.Log($"body {body}");
		var url = URL + "/auth/signup";
		Debug.Log($"url: {url}");

		using UnityWebRequest request = UnityWebRequest.Put(URL + "/auth/signup", body);
		//request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Content-Type", "application/json");
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
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to sign up: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

	public IEnumerator GetGameList(string token, Action<List<GameDTO>> onGamesGot, Action<StrategoErrorDTO> onError)
	{
		using UnityWebRequest request = UnityWebRequest.Get(URL + "/game");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Game list received: " + json);

			var gameList = JsonUtility.FromJson<GameListDTO>("{\"games\":" + json + "}");
			onGamesGot?.Invoke(gameList.games);
		}
		else
		{
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to get game list: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
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

	public IEnumerator CreateGame(string token, Action<GameDTO> onGameCreated, Action<StrategoErrorDTO> onError)
	{
		var data = JsonUtility.ToJson(new GameInputDTO(GetJoinCode()));
		using UnityWebRequest request = UnityWebRequest.Put(URL + "/game", data);
		//request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Content-Type", "application/json");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Logged in. Received: " + json);

			var gameDto = JsonUtility.FromJson<GameDTO>(json);
			onGameCreated?.Invoke(gameDto);
		}
		else
		{
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to create game: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

	public IEnumerator JoinGame(int gameId, string token, Action<GameExtendedDTO> onJoinedGame, Action<StrategoErrorDTO> onError)
	{
		//var data = JsonUtility.ToJson(new GameInputDTO(GetJoinCode()));
		using UnityWebRequest request = UnityWebRequest.Put(URL + $"/game/{gameId}/join", "");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Game joined. Received: " + json);

			var gameExtendedDto = JsonUtility.FromJson<GameExtendedDTO>(json);
			onJoinedGame?.Invoke(gameExtendedDto);
		}
		else
		{
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to join game: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

	public IEnumerator LeaveGame(int gameId, string token, Action<GameDTO> onLeftGame, Action<StrategoErrorDTO> onError)
	{
		using UnityWebRequest request = UnityWebRequest.Put(URL + $"/game/{gameId}/leave", "");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Game left. Received: " + json);

			var gameDto = JsonUtility.FromJson<GameDTO>(json);
			onLeftGame?.Invoke(gameDto);
		}
		else
		{
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to leave game: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

	public IEnumerator AddSetup(int gameId, string token, List<List<Rank>> setup, Action<GameStateDTO> onSetupAdded, Action<StrategoErrorDTO> onError)
	{
		//var data = JsonUtility.ToJson(new ArmySetupDTO(setup));
		var data = JsonConvert.SerializeObject(new ArmySetupDTO(setup));
		Debug.Log(data);
		using UnityWebRequest request = UnityWebRequest.Put(URL + $"/stratego/{gameId}/setup", data);
		//request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Content-Type", "application/json");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Setup sent. Received: " + json);

			//var gameStateDto = JsonUtility.FromJson<GameStateDTO>(json);
			var gameStateDto = JsonConvert.DeserializeObject<GameStateDTO>(json);
			onSetupAdded?.Invoke(gameStateDto);
		}
		else
		{
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to add setup: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

	public IEnumerator AddMovement(int gameId, string token, StrategoMovementDTO movement, Action<GameStateDTO> onMovementAdded, Action<StrategoErrorDTO> onError)
	{
		var data = JsonUtility.ToJson(movement);
		using UnityWebRequest request = UnityWebRequest.Put(URL + $"/stratego/{gameId}/setup", data);
		//request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Content-Type", "application/json");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Movement sent. Received: " + json);

			//var gameStateDto = JsonUtility.FromJson<GameStateDTO>(json);
			var gameStateDto = JsonConvert.DeserializeObject<GameStateDTO>(json);
			onMovementAdded?.Invoke(gameStateDto);
		}
		else
		{
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to add movement: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

	public IEnumerator GetStatus(int gameId, string token, Action<GameStateDTO> onStatusGot, Action<StrategoErrorDTO> onError)
	{
		using UnityWebRequest request = UnityWebRequest.Get(URL + $"/stratego/{gameId}/status");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Got game status. Received: " + json);

			//var gameStateDto = JsonUtility.FromJson<GameStateDTO>(json);
			var gameStateDto = JsonConvert.DeserializeObject<GameStateDTO>(json);
			onStatusGot?.Invoke(gameStateDto);
		}
		else
		{
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to get status: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

	public IEnumerator GetGame(int gameId, string token, Action<GameExtendedDTO> onGameGot, Action<StrategoErrorDTO> onError)
	{
		using UnityWebRequest request = UnityWebRequest.Get(URL + $"/game/{gameId}");
		request.SetRequestHeader("Authorization", $"Bearer {token}");
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string json = request.downloadHandler.text;

			Debug.Log("Got game. Received: " + json);

			//var gameStateDto = JsonUtility.FromJson<GameStateDTO>(json);
			var gameStateDto = JsonConvert.DeserializeObject<GameExtendedDTO>(json);
			onGameGot?.Invoke(gameStateDto);
		}
		else
		{
			string json = request.downloadHandler.text;

			Debug.LogError("Failed to get game: " + request.error);

			var result = JsonUtility.FromJson<StrategoErrorDTO>(json);
			onError?.Invoke(result);
		}
	}

}
