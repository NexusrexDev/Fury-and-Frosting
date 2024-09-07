using Godot;
using Godot.Collections;
using System;

public partial class WitchAttack2 : WitchState
{
	private Vector2 _velocity;

	private enum FadeType
	{
		Appear,
		Disappear
	}

	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		base.Enter(_message);

		if (_message != null)
		{
			//Sprout generation code goes here
		}

		_phaseTimer.WaitTime = 0.5f;
		_phaseTimer.Start();
	}

	public override void PhaseHandle()
	{
		_witch.SetSpriteFrame(0);
		if (_phaseStep < 3)
			Teleport();
		else
			Dizzy();
	}

	private async void Teleport()
	{
		_witch.SetSpriteFrame(0);
		_witch.SwitchCollisions(ColliderState.Invincible);
		_witch.SetRotation(0);

		WitchFade fadeOut = CreateFade(FadeType.Disappear, _witch.Position);
		_witch.Position = new Vector2(-10, -10);

		await ToSignal(fadeOut, WitchFade.SignalName.FadeEnd);

		Vector2 randomPosition = new Vector2(_witch.NumberGenerator.RandiRange(24, 296), 56);
		_witch.Direction = (sbyte)((randomPosition.X > _witch.PlayerReference.Position.X) ? -1 : 1);

		WitchFade fadeIn = CreateFade(FadeType.Appear, randomPosition);
		await ToSignal(fadeIn, WitchFade.SignalName.FadeEnd);
		_witch.Position = randomPosition;

		_witch.SetTargetScale(Vector2.One * 0.9f);
		await ToSignal(GetTree().CreateTimer(0.2f), Timer.SignalName.Timeout);

		_witch.CreateProjectile(true);

		_phaseTimer.WaitTime = 0.3f;
		_phaseTimer.Start();
		_phaseStep++;
	}

	private WitchFade CreateFade(FadeType type, Vector2 position)
	{
		WitchFade fadeObject;
		switch(type)
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
