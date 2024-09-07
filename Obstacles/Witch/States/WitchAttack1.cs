using Godot;
using Godot.Collections;
using System;

public partial class WitchAttack1 : WitchState
{
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private const float _jumpVelocity = -380.0f;
	private const float _horizontalVelocity = 170f;
	private const float _jumpRotation = 461f;

	private Vector2 _velocity;
	private bool previouslyOnFloor = true;

	private float angle = 0;

	private Tween _tween;

	[Export]
	private AudioStream jumpSFX;

	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		base.Enter(_message);
		_phaseTimer.WaitTime = 1;
		_phaseTimer.Start();
	}

	public override void PhaseHandle()
	{
		_witch.SetSpriteFrame(0);
		if (_phaseStep == 0 || _phaseStep == 1)
			Jump();
		else if (_phaseStep == 2)
			ChargeShot();
	}

	public override void PhysicsProcess(double delta)
	{
		if (!_witch.IsOnFloor())
		{
			_velocity = _witch.Velocity;

			angle += _jumpRotation * (float)delta * _witch.Direction;
			_witch.SetRotation(angle);

			if (!_witch.IsOnFloor() && _velocity.Y < Witch.MaxFallSpeed)
				_velocity.Y += _gravity * (float)delta;
		}
		else
		{
			if (previouslyOnFloor != _witch.IsOnFloor())
			{
				_witch.SetScale(new Vector2(1.25f, 0.75f));
				angle = 0;
				_witch.SetRotation(angle);
				_witch.EmitLandingParticles();

				_velocity = Vector2.Zero;

				_phaseTimer.Start();
			}
		}

		previouslyOnFloor = _witch.IsOnFloor();
		_witch.Velocity = _velocity;
	}

	private void Jump()
	{
		_witch.SwitchCollisions(ColliderState.Invincible);
		_witch.SetSpriteFrame(0);
		_velocity.Y = _jumpVelocity;
		_velocity.X = _horizontalVelocity * _witch.Direction;
		_witch.SetScale(new Vector2(0.75f, 1.25f));
		AudioManager.Instance.PlaySFX(jumpSFX);
		_phaseTimer.WaitTime = 0.4f;
		_phaseStep++;
	}

	private void ChargeShot()
	{
		_witch.Direction = (sbyte)-_witch.Direction;
		_witch.SetTargetScale(Vector2.One * 0.9f);

		if (_tween != null)
			_tween.Kill();
		_tween = CreateTween();
		_tween.TweenCallback(Callable.From(() => { _witch.CreateProjectile(false); })).SetDelay(0.25f);
		_tween.TweenCallback(Callable.From(Dizzy)).SetDelay(0.5f);
	}

	private void Dizzy()
	{
		_witch.SwitchCollisions(ColliderState.Vulnerable);
		_witch.SetRotation(0);
		_witch.SetScale(new Vector2(1.15f, 0.85f));
		_witch.SetSpriteFrame(2);
		_phaseTimer.WaitTime = 2f;
		_phaseTimer.Start();
		_phaseStep = 0;
	}
}
