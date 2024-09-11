using Godot;
using Godot.Collections;
using System;
using static Godot.TextServer;

public partial class WitchAttack3 : WitchState
{
	private const int _teleportStop = 3;
	private Vector2 _velocity;

	private int _lane = 1;

	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		base.Enter(_message);

		if (_message != null)
		{
			_witch.EmitSignal(Witch.SignalName.PhaseChange, 3);
		}

		_phaseTimer.WaitTime = 0.5f;
		_phaseTimer.Start();
	}

	public override void PhaseHandle()
	{
		_witch.SetSpriteFrame(0);
		if (_phaseStep < _teleportStop)
			Teleport(_phaseStep != _teleportStop-1, _lane);
		else if (_phaseStep == _teleportStop)
			WallShot();
		else
			Dizzy();
	}

	private async void Teleport(bool random, int lane = 0)
	{
		_witch.SetSpriteFrame(0);
		_witch.SwitchCollisions(ColliderState.Invincible);
		_witch.SetRotation(0);

		WitchFade fadeOut = CreateFade(FadeType.Disappear, _witch.Position);
		_witch.Position = new Vector2(-10, -10);

		await ToSignal(fadeOut, WitchFade.SignalName.FadeEnd);

		Vector2 position;
		if (random)
		{
			position = new Vector2(_witch.NumberGenerator.RandiRange(24, 296), 56);
			_witch.Direction = (sbyte)((position.X > _witch.PlayerReference.Position.X) ? -1 : 1);
		}
		else
		{
			switch (lane)
			{
				case -1:
					position = new Vector2(64, 56);
					_witch.Direction = 1;
					break;
				case 1:
					position = new Vector2(256, 56);
					_witch.Direction = -1;
					break;
				default:
					position = new Vector2(160, 56);
					break;
			}
		}

		WitchFade fadeIn = CreateFade(FadeType.Appear, position);
		await ToSignal(fadeIn, WitchFade.SignalName.FadeEnd);
		_witch.Position = position;

		_phaseTimer.WaitTime = 0.2f;
		_phaseTimer.Start();
		_phaseStep++;
	}

	private WitchFade CreateFade(FadeType type, Vector2 position)
	{
		WitchFade fadeObject;
		switch (type)
		{
			case FadeType.Appear:
				fadeObject = _witch.FadeInReference.Instantiate<WitchFade>();
				break;
			case FadeType.Disappear:
				fadeObject = _witch.FadeOutReference.Instantiate<WitchFade>();
				break;
			default:
				fadeObject = new WitchFade();
				break;
		}
		fadeObject.Scale = new Vector2(-_witch.Direction, 1);
		fadeObject.Position = position;
		AddSibling(fadeObject);
		return fadeObject;
	}

	private async void WallShot()
	{
		_witch.SetTargetScale(Vector2.One * 0.85f);
		await ToSignal(GetTree().CreateTimer(0.15f), "timeout");

		_witch.EmitSignal(Witch.SignalName.WallActivator, _lane);

		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

		_witch.EmitSignal(Witch.SignalName.ThrowActivator, _lane);

		await ToSignal(GetTree().CreateTimer(0.8f), "timeout");

		_witch.SetTargetScale(Vector2.One);
		_witch.SetScale(Vector2.One * 1.25f);
		_witch.SetSpriteFrame(1);
		_witch.SetRotation(_witch.Direction * 4);

		_phaseTimer.WaitTime = 0.3f;
		_phaseTimer.Start();
		_phaseStep++;
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
		_lane = -_lane;
	}

}
