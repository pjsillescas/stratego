using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsoleLog : MonoBehaviour
{
	private const int MESSAGE_THRESHOLD = 200;

	[SerializeField]
	private GameObject LogItemPrefab;
	[SerializeField]
	private Transform ContentTransform;

	private List<ConsoleLogItem> messages;
	public void AddMessage(string message)
	{
		var item = Instantiate(LogItemPrefab, ContentTransform).GetComponent<ConsoleLogItem>();
		item.transform.SetAsFirstSibling();
		item.AddMessage(message);
		messages.Add(item);

		if (messages.Count > MESSAGE_THRESHOLD)
		{
			var lastElement = messages.LastOrDefault();
			messages.Remove(lastElement);
			Destroy(lastElement.gameObject);
		}
	}

	void Awake()
	{
		messages = new();
		Application.logMessageReceived += LogHandler;
	}

	private void Start()
	{
		InputManager.OnDebugConsoleToggle += ToggleConsole;
		gameObject.SetActive(false);
	}

	private void ToggleConsole(object sender, EventArgs args)
	{
		Debug.Log("toggle console");
		gameObject.SetActive(!gameObject.activeInHierarchy);
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= LogHandler;
		InputManager.OnDebugConsoleToggle -= ToggleConsole;
	}

	private void OnEnable()
	{
		//Application.logMessageReceived += LogHandler;
	}

	private void OnDisable()
	{
		//Application.logMessageReceived -= LogHandler;
	}

	private void LogHandler(string condition, string stackTrace, LogType type)
	{
		AddMessage(condition);
	}

}
