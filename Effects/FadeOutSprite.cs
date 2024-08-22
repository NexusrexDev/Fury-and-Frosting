using Godot;
using System;

public partial class FadeOutSprite : Sprite2D
{
	public override void _Ready()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 0, 0.25f);
		tween.Finished += () => QueueFree();
	}
}
