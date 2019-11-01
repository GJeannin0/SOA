using System;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : Bolt.GlobalEventListener
{
	[SerializeField] private GameObject textInput;
	[SerializeField] private GameObject menuPanel;
	[SerializeField] private Button joinSessionButton;


	[SerializeField] private float buttonSpacing;
	private List<Button> joinSessionButtons = new List<Button>();

	private bool joiningPrivate = false;


	private void Awake()
	{
		BoltLauncher.StartClient();
	}

	public void StartClient()
	{
		if (textInput.GetComponent<Text>().text != "")
		{
			// BoltLauncher.StartClient();
			joiningPrivate = true;
		}
	}

	public void StartServer()
	{
		if (textInput.GetComponent<Text>().text != "")
		{
			BoltLauncher.Shutdown();
			BoltLauncher.StartServer();
		}
	}

	public override void BoltStartDone()
	{
		if (BoltNetwork.IsServer)
		{
			string matchName = textInput.GetComponent<Text>().text;
			Bolt.Matchmaking.BoltMatchmaking.CreateSession(matchName, sceneToLoad: "PlayScene");
		}
	}

	// Called when a server is created or destroyed, and every few seconds
	public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
	{
		string matchName = textInput.GetComponent<Text>().text;
		bool foundSession = false;

		foreach (var session in sessionList)
		{
			matchName = textInput.GetComponent<Text>().text;
			UdpSession photonSession = session.Value as UdpSession;
			if (joiningPrivate)
			{		
				if (session.Value.HostName == matchName)
				{
					Bolt.Matchmaking.BoltMatchmaking.JoinSession(matchName);
					foundSession = true;
					joiningPrivate = false;
				}

				/*
				if (photonSession.Source == UdpSessionSource.Photon)
				{
					BoltNetwork.Connect(photonSession);
				}
				*/


				if (!foundSession)
				{
					BoltLauncher.Shutdown();
					SceneManager.LoadScene("Menu");
					joiningPrivate = false;
				}
			}
			matchName = textInput.GetComponent<Text>().text;
			Button joinSessionButtonClone = Instantiate(joinSessionButton);
			joinSessionButtonClone.transform.parent = menuPanel.transform;
			joinSessionButtonClone.transform.localPosition = new Vector3(250, buttonSpacing * joinSessionButtons.Count, 0);
			joinSessionButtonClone.gameObject.SetActive(true);
			joinSessionButtonClone.GetComponentInChildren<Text>().text = "Join: " + photonSession.HostName;

			joinSessionButtonClone.onClick.AddListener(() => JoinSessionOfId(photonSession));

			joinSessionButtons.Add(joinSessionButtonClone);
		}
	}

	public void JoinRandomRoom()
	{
		//BoltLauncher.StartClient();
		Bolt.Matchmaking.BoltMatchmaking.JoinRandomSession();
	}

	public void JoinSessionOfId(UdpSession session)
	{
		//BoltLauncher.StartClient();
		Bolt.Matchmaking.BoltMatchmaking.JoinSession(session.HostName);
	}
}
