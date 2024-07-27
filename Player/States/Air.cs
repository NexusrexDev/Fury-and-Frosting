using Godot;
using Godot.Collections;
using System;

public partial class Air : PlayerState
{
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private bool _hasJumped = false, _isHurt = false, _hControl = true;
	private ulong _fallTime;
	
	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		if (_message != null)
		{
			if (_message.ContainsKey("do_jump"))
			{
				Vector2 velocity = _player.Velocity;
				velocity.Y = Player.JumpVelocity;
				_player.Velocity = velocity;
				_hasJumped = true;
			}

			if (_message.ContainsKey("canJump"))
				_hasJumped = !(bool)_message["canJump"];

			if (_message.ContainsKey("hurt"))
				_isHurt = (bool)_message["hurt"];
		}
		else
		{
			_fallTime = Time.GetTicksMsec();
		}
	}

	public override void Exit()
	{
		_hasJumped = false;
		_fallTime = 0;
		_isHurt = false;
		_hControl = true;
	}

	public override void Update(double delta)
	{
		if (Input.IsActionJustPressed("game_attack"))
			StateMachine.TransitionTo("Attack");
	}

	public override void PhysicsProcess(double delta)
	{
		Vector2 velocity = _player.Velocity;
		//Horizontal movement
		float direction = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
		if (direction != 0 && _hControl)
		{
			velocity.X = direction * Player.Speed;
			_player.Direction = (sbyte)direction;
			_isHurt = false;
		}

		//Jumping
		if (Input.IsActionJustPressed("game_jump"))
		{
			if (!_hasJumped) //Coyote time
			{
				if (Time.GetTicksMsec() - _fallTime < 150)
				{
					velocity.Y = Player.JumpVelocity;
					_hasJumped = true;
				}
			}
			else
			{
				if (_player.IsOnWallOnly()) //Wall jump
				{
					velocity.Y = Player.JumpVelocity;
					velocity.X = -_player.Direction * Player.Speed;
					_player.Direction = (sbyte)-_player.Direction;
					_hControl = false;
				}
			}
		}

		if (!_player.IsOnFloor())
			velocity.Y += gravity * (float)delta;

		_player.Velocity = velocity;

		//Transitioning
		if (_player.IsOnFloor())
		{
			if (_isHurt)
				StateMachine.TransitionTo("Idle", new Dictionary<string, Variant> { { "landed", true } });
			else
			{
				if (Mathf.IsEqualApprox(Mathf.Abs(velocity.X), 300))
					StateMachine.TransitionTo("Run", new Dictionary<string, Variant> { { "landed", true } });
				else
					StateMachine.TransitionTo("Idle", new Dictionary<string, Variant> { { "landed", true } });
			}
		}

		//Transitioning to the Dash state
		if (Input.IsActionJustPressed("game_dash"))
			StateMachine.TransitionTo("Dash");
	}

}
