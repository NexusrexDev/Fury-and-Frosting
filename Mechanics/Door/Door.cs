using Godot;
using System;

public partial class Door : ActivatableObject
{
	[Export]
	private AnimatedSprite2D _sprite;

	public override void Activate()
	{
		_sprite.Play("open");
		_sprite.AnimationFinished += () => QueueFree();
	}

	public override void Deactivate() { }
}
