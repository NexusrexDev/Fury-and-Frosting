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
				_player.SetScale(new Vector2(1.25f, 0.75f));
			
		}
	}

	public override void Update(double delta)
	{
		if (Input.IsActionJustPressed("game_attack") && _player.CanAttack)
			StateMachine.TransitionTo(Attack);
	}

	public override void PhysicsProcess(double delta)
	{
		Vector2 velocity = _player.Velocity;
		//Horizontal movement
		float direction = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
		if (direction != 0)
		{
			velocity.X = direction * Player.Speed;
			_player.Direction = (sbyte)direction;
			_player.SetRotation(4 * _player.Direction);
		}

		//Transitioning to the Idle state
		if (_player.Velocity == Vector2.Zero)
			StateMachine.TransitionTo(Idle);

		_player.Velocity = velocity;

		//Transitioning to the Air state
		//By falling off a platform
		if (!_player.IsOnFloor())
			StateMachine.TransitionTo(Air);

		//By jumping
		if (Input.IsActionJustPressed("game_jump") && _player.IsOnFloor())
			StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "do_jump", true } });

		//Transitioning to the Dash state
		if (Input.IsActionJustPressed("game_dash"))
			StateMachine.TransitionTo(Dash);
	}
}
