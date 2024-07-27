using Godot;
using System;

[GlobalClass]
public partial class ActivatablePlatform : StaticBody2D
{
	[Export]
	private Vector2 _startPosition;

	private Sprite2D _sprite;
	private CollisionShape2D _collisionShape;
	private Tween _tween;

	public override void _Ready()
	{
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_sprite = GetNode<Sprite2D>("Sprite2D");

		_sprite.GlobalPosition = _startPosition;
		CollisionLayer = 0;
		
		_sprite.Visible = false;
	}

	public void Activate()
	{
		if (_tween != null)
			_tween.Kill();
		CollisionLayer = 1;
		_sprite.Visible = true;
		_tween = CreateTween();
		_tween.TweenProperty(_sprite, "position", Vector2.Zero, 0.2f).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
	}

	public void Deactivate()
	{
		if (_tween != null)
			_tween.Kill();
		CollisionLayer = 0;
		_tween = CreateTween();
		_tween.TweenProperty(_sprite, "global_position", _startPosition, 0.2f).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
		_tween.Finished += _On_Tween_Complete;
	}

	private void _On_Tween_Complete()
	{
		_sprite.Visible = false;
		_tween.Finished -= _On_Tween_Complete;
	}
}
