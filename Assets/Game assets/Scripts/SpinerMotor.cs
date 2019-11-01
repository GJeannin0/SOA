using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinerMotor : MonoBehaviour
{
	State _state;

	public struct State
	{
		public Vector3 position;
		public Vector3 velocity;
		public bool dashing;
	}

	[SerializeField] private float ejectSpeed = 1f;
	[SerializeField] private float acceleration = 2f;
	[SerializeField] private float velocityDamping = 0.6f;
	[SerializeField] private float velocityWallBounceDamping = 0.1f;
	[SerializeField] private float velocityPlayerBounceDamping = 0.15f;

	private SpinerController spinerController;
	private Vector3 knockBack = Vector3.zero;
	private BoltEntity spiner;

	private void Awake()
	{
		spinerController = GetComponent<SpinerController>();
		_state = new State();
		_state.position = transform.position;
		spiner = GetComponent<BoltEntity>();
	}

	public void SetState(Vector3 position, Vector3 velocity, bool dashing)
	{
		// assign new state
		_state.position = position;
		_state.velocity = velocity;
		_state.dashing = dashing;
	}

	public State Move(bool forward, bool backward, bool left, bool right, bool dash)
	{
		State _newState = new State();

		var movement = Vector3.zero;

		if (forward)
		{
			movement.z += 1;
		}
		if (backward)
		{
			movement.z -= 1;
		}
		if (right)
		{
			movement.x += 1;
		}
		if (left)
		{
			movement.x -= 1;
		}

		_newState.velocity = _state.velocity + movement.normalized * acceleration * BoltNetwork.FrameDeltaTime + knockBack;

		knockBack = Vector3.zero;

		_newState.velocity -= velocityDamping * _newState.velocity * BoltNetwork.FrameDeltaTime;

		_newState.position = _state.position + _newState.velocity * BoltNetwork.FrameDeltaTime;

		_state = _newState;

		return _newState;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			if (spiner.IsOwner)
			{
				float knockBackMagnitude = collision.gameObject.GetComponent<SpinerMotor>().GetVelocity().magnitude;

				if (_state.velocity.magnitude > knockBackMagnitude)
				{
					knockBackMagnitude = _state.velocity.magnitude;
				}

				knockBack = (knockBackMagnitude * (transform.position - collision.gameObject.transform.position).normalized - _state.velocity) * (1 - velocityPlayerBounceDamping);
			}
		}

		if (collision.gameObject.tag == "Wall")  //BoltNetwork.IsServer
		{
			if (spiner.IsOwner)
			{
				knockBack = (_state.velocity.magnitude * collision.gameObject.transform.forward - _state.velocity) * (1 - velocityWallBounceDamping);
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.tag == "Wall")  //BoltNetwork.IsServer
		{
			if (spiner.IsOwner)
			{
				
				knockBack = ((_state.velocity.magnitude+ejectSpeed) * collision.gameObject.transform.forward - _state.velocity) * (1 - velocityWallBounceDamping);
			}
		}
	}

	public Vector3 GetVelocity()
	{
		return _state.velocity;
	}
}
