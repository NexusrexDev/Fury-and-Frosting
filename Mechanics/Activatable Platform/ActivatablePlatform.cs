using Godot;
using System;

[GlobalClass]
public partial class ActivatablePlatform : ActivatableObject
{
	[Export]
	private Vector2 _startPosition;

	private Vector2 _endPosition, _shapeSize, _spritePosition;

	[Export]
	private Texture2D _texture;
	
	private CollisionShape2D _collisionShape;
	private Tween _tween;

	public override void _Ready()
	{
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		_shapeSize = (_collisionShape.Shape as RectangleShape2D).Size;
		
		_endPosition = new Vector2(-_shapeSize.X/2, -_shapeSize.Y/2);
		_startPosition = new Vector2(_endPosition.X + _startPosition.X, _endPosition.Y + _startPosition.Y);
		_spritePosition = _startPosition;
		Visible = false;

		CollisionLayer = 0;
	}

	public override void _Process(double delta)
	{
		if (Visible)
			QueueRedraw();
	}

	public override void _Draw()
	{
		Vector2 spriteWidth = new Vector2(_shapeSize.X, 16);
		float sliceSize = _texture.GetSize().X / 3;
		Vector2 middleHeight = new Vector2(_shapeSize.X, _shapeSize.Y - (sliceSize*2));
		
		DrawTextureRectRegion(_texture, new Rect2(_spritePosition, new Vector2(_shapeSize.X, 16)), new Rect2(Vector2.Zero, spriteWidth));
		DrawTextureRectRegion(_texture, new Rect2(_spritePosition + new Vector2(0, sliceSize), middleHeight), new Rect2(new Vector2(0, 16), spriteWidth));
		DrawTextureRectRegion(_texture, new Rect2(_spritePosition + new Vector2(0, _shapeSize.Y - sliceSize), new Vector2(_shapeSize.X, sliceSize)), new Rect2(new Vector2(0, 32), spriteWidth));
	}

	public override void Activate()
	{
		if (_tween != null)
			_tween.Kill();
		CollisionLayer = 1;
		Visible = true;
		_tween = CreateTween();
		_tween.TweenProperty(this, "_spritePosition", _endPosition, 0.2f).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
	}

	public override void Deactivate()
	{
		if (_tween != null)
			_tween.Kill();
		CollisionLayer = 0;
		_tween = CreateTween();
		_tween.TweenProperty(this, "_spritePosition", _startPosition, 0.2f).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
		_tween.Finished += _On_Tween_Complete;
	}

	private void _On_Tween_Complete()
	{
		Visible = false;
		_tween.Finished -= _On_Tween_Complete;
	}
}
