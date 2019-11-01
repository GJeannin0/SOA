using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "PlayScene")]
public class ServerCallbacks : Bolt.GlobalEventListener
{
	void Awake()
	{
		PlayerObjectsRegistry.CreateServerPlayer();
	}

	public override void Connected(BoltConnection connection)
	{
		PlayerObjectsRegistry.CreateClientPlayer(connection);
	}

	public override void SceneLoadLocalDone(string map)
	{
		PlayerObjectsRegistry.ServerPlayer.Spawn();
	}

	public override void SceneLoadRemoteDone(BoltConnection connection)
	{
		PlayerObjectsRegistry.GetPlayer(connection).Spawn();
	}
}
