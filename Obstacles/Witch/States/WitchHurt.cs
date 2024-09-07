using Godot;
using Godot.Collections;
using System;

public partial class WitchHurt : WitchState
{
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Vector2 _velocity;
	private bool previouslyOnFloor = false;

	[Export]
	private AudioStream _hurtSFX;

	private bool _phaseSwitched;

	public override async void Enter(Dictionary<string, Variant> _message = null)
	{
		Vector2 knockback = new Vector2(-_witch.Direction * 80f, -75f);
		_witch.Velocity = knockback;
		_witch.IFrameTimer.Start();

		HandleFeedback();

		StringName preHitPhase = _witch.BossPhase;

		_witch.Health -= (int)_message["value"];

		_phaseSwitched = (_witch.BossPhase != preHitPhase);

		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

		_witch.SetSpriteFlash(false);
	}

	public override void PhysicsProcess(double delta)
	{
		if (!_witch.IsOnFloor())
		{
			_velocity = _witch.Velocity;

			if (!_witch.IsOnFloor() && _velocity.Y < Witch.MaxFallSpeed)
				_velocity.Y += gravity * (float)delta;
		}
		else
		{
			if (previouslyOnFloor != _witch.IsOnFloor())
			{
				_witch.SetScale(new Vector2(1.25f, 0.75f));
				_witch.EmitLandingParticles();

				_velocity = Vector2.Zero;

				Dictionary<string, Variant> message = null;
				if (_phaseSwitched)
					message = new Dictionary<string, Variant> { { "start", true } };

				if (_witch.Health > 0)
					StateMachine.TransitionTo(_witch.BossPhase, message);
			}
		}

		previouslyOnFloor = _witch.IsOnFloor();
		_witch.Velocity = _velocity;
	}

	private void HandleFeedback()
	{
		_witch.SetScale(new Vector2(1.15f, 0.9f));
		_witch.SetSpriteFlash(true);
		_witch.SetSpriteFrame(3);

		AudioManager.Instance.PlaySFX(_hurtSFX);

		GameManager.Instance.EmitSignal(GameManager.SignalName.ScreenShake, 0.2f);
		GameManager.Instance.FreezeFrame(0.1f, 0.2f);
	}


}
