using Godot;
using System;

public partial class WitchFade : Node2D
{
	[Signal]
	public delegate void FadeEndEventHandler();

	[Export]
	private AudioStream _fadeSFX;

	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		if (_fadeSFX != null)
			AudioManager.Instance.PlaySFX(_fadeSFX);

		_animationPlayer.AnimationFinished += OnAnimationEnded;
	}

	private void OnAnimationEnded(StringName animName)
	{
		EmitSignal(SignalName.FadeEnd);
		_animationPlayer.AnimationFinished -= OnAnimationEnded;
		QueueFree();
	}
}
