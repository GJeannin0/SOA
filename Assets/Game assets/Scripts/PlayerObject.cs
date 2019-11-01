using UnityEngine;

public class PlayerObject
{
    public BoltEntity spiner;
	public BoltConnection connection;

	public bool IsServer
	{
		get { return connection == null; }
	}

	public bool IsClient
	{
		get { return connection != null; }
	}

	public void Spawn()
	{
		if (!spiner)
		{
			spiner = BoltNetwork.Instantiate(BoltPrefabs.Spiner, RandomPosition(), Quaternion.identity);

			if (IsServer)
			{
				spiner.TakeControl();
			}
			else
			{
				spiner.AssignControl(connection);
			}
		}

		// teleport entity to a random spawn position
		spiner.transform.position = RandomPosition();
	}

	Vector3 RandomPosition()
	{
		return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
	}
}

