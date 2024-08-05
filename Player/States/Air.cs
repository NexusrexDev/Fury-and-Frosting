using Godot;
using Godot.Collections;
using System;

public partial class Air : PlayerState
{
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private bool _hasJumped = false, _isHurt = false, _hControl = true, _applyVelocity;
	private ulong _fallTime;
	private Vector2 _intermediaryVelocity;
	
	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		_player.SetRotation(0);
		if (_message != null)
		{
			if (_message.ContainsKey("do_jump"))
			{
				_intermediaryVelocity = _player.Velocity;
				_intermediaryVelocity.Y = Player.JumpVelocity;
				_applyVelocity = true;
				_player.Velocity = _intermediaryVelocity;
				
				_hasJumped = true;
				_player.SetScale(new Vector2(0.75f, 1.25f));
			}

			if (_message.ContainsKey("do_wallJump"))
			{
				_intermediaryVelocity = _player.Velocity;
				_intermediaryVelocity.Y = Player.JumpVelocity;
				_intermediaryVelocity.X = -_player.Direction * Player.Speed;
				_player.Direction = (sbyte)-_player.Direction;
				_applyVelocity = true;

				_hControl = false;
				_hasJumped = true;
				_player.SetScale(new Vector2(0.75f, 1.25f));

				_player.CanAttack = true;
				_player.CanDash = true;
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
		_applyVelocity = false;
	}

	public override void Update(double delta)
	{
		if (Input.IsActionJustPressed("game_attack") && _player.CanAttack)
			StateMachine.TransitionTo(Attack);
	}

	public override void PhysicsProcess(double delta)
	{
		Vector2 velocity = _player.Velocity;

		if (_applyVelocity)
		{
			velocity = _intermediaryVelocity;
			_applyVelocity = false;
		}

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
					StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "do_jump", true } });
			}
			else
			{
				if (_player.IsOnWallOnly()) //Wall jump
					StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "do_wallJump", true } });
			}
		}

		if (!_player.IsOnFloor() && velocity.Y < Player.MaxFallSpeed)
			velocity.Y += gravity * (float)delta;

		_player.Velocity = velocity;

		//Transitioning
		if (_player.IsOnFloor())
		{
			if (_isHurt)
				StateMachine.TransitionTo(Idle, new Dictionary<string, Variant> { { "landed", true } });
			else
			{
				if (Mathf.IsEqualApprox(Mathf.Abs(velocity.X), Player.Speed))
					StateMachine.TransitionTo(Run, new Dictionary<string, Variant> { { "landed", true } });
				else
					StateMachine.TransitionTo(Idle, new Dictionary<string, Variant> { { "landed", true } });
			}
		}

		//Transitioning to the Dash state
		if (Input.IsActionJustPressed("game_dash") && _player.CanDash)
			StateMachine.TransitionTo(Dash);
	}

}
