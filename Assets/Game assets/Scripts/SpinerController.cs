using Bolt;
using UnityEngine;

public class SpinerController : Bolt.EntityBehaviour<ISpinerState>
{
	private Color playerColor;

	bool _forward;
	bool _backward;
	bool _left;
	bool _right;
	bool _dash;

	SpinerMotor _motor;

	void Awake()
	{
		_motor = GetComponent<SpinerMotor>();
	}

	public override void Attached()
	{
		playerColor = Random.ColorHSV();
		GetComponentInChildren<MeshRenderer>().material.color = playerColor;

		// This couples the Transform property of the State with the GameObject Transform
		state.SetTransforms(state.SpinerTransform, transform);
	}

	void PollKeys()
	{
		_forward = Input.GetKey(KeyCode.W);
		_backward = Input.GetKey(KeyCode.S);
		_left = Input.GetKey(KeyCode.A);
		_right = Input.GetKey(KeyCode.D);
		_dash = Input.GetKeyDown(KeyCode.Space);
	}

	void Update()
	{
		PollKeys();
	}

	public override void SimulateController()
	{
		PollKeys();

		ISpinerCommandInput input = SpinerCommand.Create();

		input.Forward = _forward;
		input.Backward = _backward;
		input.Left = _left;
		input.Right = _right;
		input.Dash = _dash;

		entity.QueueInput(input);
	}

	public override void ExecuteCommand(Command command, bool resetState)
	{
		SpinerCommand cmd = (SpinerCommand)command;

		if (resetState)
		{
			// we got a correction from the server, reset (this only runs on the client)
			_motor.SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.Dashing);
		}
		else
		{
			// apply movement (this runs on both server and client)
			SpinerMotor.State motorState = _motor.Move(cmd.Input.Forward, cmd.Input.Backward, cmd.Input.Left, cmd.Input.Right, cmd.Input.Dash);

			// apply movement
			transform.position = motorState.position;

			// copy the motor state to the commands result (this gets sent back to the client)
			cmd.Result.Position = motorState.position;
			cmd.Result.Velocity = motorState.velocity;
			cmd.Result.Dashing = motorState.dashing;
		}

		
	}

	
}