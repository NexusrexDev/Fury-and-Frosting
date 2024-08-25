using Godot;
using Godot.Collections;
using System;

public partial class Hurt : PlayerState
{
	public override async void Enter(Dictionary<string, Variant> _message = null)
	{
		_player.Timer.Stop();
		_player.IFrameTimer.Start();
		Vector2 knockback = new Vector2(-_player.Direction * 80f, -75f);
		_player.Velocity = knockback;
		GameManager.Instance.EmitSignal(GameManager.SignalName.ScreenShake, 0.2f);
		GameManager.Instance.FreezeFrame(0.1f, 0.4f);

		_player.Rage += (int)_message["value"];

		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

		StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "canJump", false }, { "hurt", true } });
	}
}