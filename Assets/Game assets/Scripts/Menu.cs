using System;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : Bolt.GlobalEventListener
{
	[SerializeField] private GameObject textInput;

	public void StartClient()
	{
		BoltLauncher.StartClient();
	}

	public void StartServer()
	{
		BoltLauncher.StartServer();
	}

	public override void BoltStartDone()
	{
		if (BoltNetwork.IsServer)
		{
			string matchName = textInput.GetComponent<Text>().text;
			Bolt.Matchmaking.BoltMatchmaking.CreateSession(matchName, sceneToLoad: "PlayScene");
		}
	}
	

	public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
	{
		bool foundSession = false;

		foreach (var session in sessionList)
		{
			UdpSession photonSession = session.Value as UdpSession;
			string matchName = textInput.GetComponent<Text>().text;

			if (session.Value.HostName == matchName)
			{
				Bolt.Matchmaking.BoltMatchmaking.JoinSession(matchName);
				foundSession = true;
			}

			/*
			if (photonSession.Source == UdpSessionSource.Photon)
			{
				BoltNetwork.Connect(photonSession);
			}
			*/
		}

		if (!foundSession)
		{
			BoltLauncher.Shutdown();
			SceneManager.LoadScene("Menu");
		}
	}
}
