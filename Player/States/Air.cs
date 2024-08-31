using Godot;
using Godot.Collections;
using System;

public partial class Air : PlayerState
{
	[Export]
	private AudioStream _jumpSFX;

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
				Jump(false);

			if (_message.ContainsKey("do_wallJump"))
				Jump(true);

			if (_message.ContainsKey("do_springJump"))
				Jump(false, 1.3f);

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

	private void Jump(bool isWallJump, float velocityMultiplier = 1f)
	{
		_intermediaryVelocity = _player.Velocity;
		_intermediaryVelocity.Y = Player.JumpVelocity * velocityMultiplier;

		if (isWallJump)
		{
			_intermediaryVelocity.X = -_player.Direction * Player.Speed;
			_player.Direction = (sbyte)-_player.Direction;

			_hControl = false;
			_player.CanAttack = true;
			_player.CanDash = true;
		}

		_applyVelocity = true;
		_player.Velocity = _intermediaryVelocity;

		_hasJumped = true;
		_player.SetScale(new Vector2(0.75f, 1.25f));
		AudioManager.Instance.PlaySFX(_jumpSFX);
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
		if (!_player.IsControllable)
			return;

		HandleInput();
	}

	public override void PhysicsProcess(double delta)
	{
		Vector2 velocity = _player.Velocity;

		if (_applyVelocity)
		{
			velocity = _intermediaryVelocity;
			_applyVelocity = false;
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
	}

	private void HandleInput()
	{
		float direction = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
		if (direction != 0 && _hControl)
		{
			_intermediaryVelocity = _player.Velocity;
			_intermediaryVelocity.X = direction * Player.Speed;
			_applyVelocity = true;
			_player.Direction = (sbyte)direction;
			_isHurt = false;
		}

		if (Input.IsActionJustPressed("game_jump"))
		{
			if (!_hasJumped)
			{
				if (Time.GetTicksMsec() - _fallTime < 150)
					StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "do_jump", true } });
			}
			
			if (_player.CollisionRayCast.IsColliding())
				StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "do_wallJump", true } });
		}

		if (Input.IsActionJustPressed("game_attack") && _player.CanAttack)
			StateMachine.TransitionTo(Attack);

		if (Input.IsActionJustPressed("game_dash") && _player.CanDash)
			StateMachine.TransitionTo(Dash);
	}
}
