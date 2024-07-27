using Godot;
using Godot.Collections;
using System;

public partial class Hurt : PlayerState
{
	public override async void Enter(Dictionary<string, Variant> _message = null)
	{
		_player.Timer.Stop();
		_player.IFrameTimer.Start();
		Vector2 knockback = new Vector2(-_player.Direction * 100f, -75f);
		_player.Velocity = knockback;

		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

		StateMachine.TransitionTo("Air", new Dictionary<string, Variant> { { "canJump", false }, { "hurt", true } });
	}
}