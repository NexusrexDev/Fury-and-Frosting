using Godot;
using System;

public partial class Spring : StaticBody2D
{
	private Sprite2D _sprite;
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		Area2D bounceArea = GetNode<Area2D>("BounceArea");
		bounceArea.BodyEntered += _On_BounceArea_Collision;
	}

	public override void _Process(double delta)
	{
		_sprite.Scale = new Vector2(Mathf.MoveToward(_sprite.Scale.X, 1, (float)delta), Mathf.MoveToward(_sprite.Scale.Y, 1, (float)delta));
	}

	private void _On_BounceArea_Collision(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			Player player = body as Player;
			player.SpringJump();
			_sprite.Scale = new Vector2(1.25f, 0.75f);
		}
	}

}
