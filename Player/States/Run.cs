using Godot;
using Godot.Collections;
using System;

public partial class Run : PlayerState
{
	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		_player.SetRotation(4 * _player.Direction);
		_player.CanDash = true;
		_player.CanAttack = true;
		_player.SetRunParticles(true);
		if (_message != null)
		{
			if (_message.ContainsKey("direction"))
			{
				float direction = (float)_message["direction"];
				Vector2 velocity = _player.Velocity;
				velocity.X = Player.Speed * direction;
				_player.Velocity = velocity;
				_player.Direction = (sbyte)direction;
			}

			if (_message.ContainsKey("landed"))
			{
				_player.SetScale(new Vector2(1.25f, 0.75f));
				_player.EmitLandingParticles();
			}
		}
	}

	public override void Exit()
	{
		_player.SetRunParticles(false);
	}

	public override void Update(double delta)
	{
		if (!_player.IsControllable)
			return;

		HandleInput();
	}

	public override void PhysicsProcess(double delta)
	{
		if (_player.Velocity == Vector2.Zero)
			StateMachine.TransitionTo(Idle);

		if (!_player.IsOnFloor())
			StateMachine.TransitionTo(Air);
	}

	private void HandleInput()
	{
		float direction = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
		if (direction != 0)
			StateMachine.TransitionTo(Run, new Dictionary<string, Variant> { { "direction", direction } });

		if (Input.IsActionJustPressed("game_jump") && _player.IsOnFloor())
			StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "do_jump", true } });

		if (Input.IsActionJustPressed("game_attack") && _player.CanAttack)
			StateMachine.TransitionTo(Attack);

		if (Input.IsActionJustPressed("game_dash") && _player.CanDash)
			StateMachine.TransitionTo(Dash);
	}
}
