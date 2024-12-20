using Godot;
using System;

public partial class Spring : StaticBody2D, IDisappearable
{
	[Export]
	public bool OneTimeUse = false;

	[Export]
	private PackedScene _explosionReference;

	[Export]
	private AudioStream _explosionSFX;

	private Sprite2D _sprite;

	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Area2D bounceArea = GetNode<Area2D>("BounceArea");
		bounceArea.BodyEntered += _On_BounceArea_Collision;

		if (OneTimeUse)
			_animationPlayer.Play("grow");
	}

	public override void _Process(double delta)
	{
		_sprite.Scale = new Vector2(Mathf.MoveToward(_sprite.Scale.X, 1, (float)delta), Mathf.MoveToward(_sprite.Scale.Y, 1, (float)delta));
	}

	private void _On_BounceArea_Collision(Node2D body)
	{
		if (body is Player player)
		{
			player.Position = new Vector2(player.Position.X, Position.Y - 22);
			player.SpringJump();
			_sprite.Scale = new Vector2(1.25f, 0.75f);

			if (OneTimeUse)
				Disappear(true);
		}
	}

	public void Disappear(bool createSprout)
	{
		ParticleEmitter explosionParticles = _explosionReference.Instantiate<ParticleEmitter>();
		explosionParticles.Position = new Vector2(Position.X, Position.Y - 10);
		AddSibling(explosionParticles);
		AudioManager.Instance.PlaySFX(_explosionSFX);

		if (createSprout)
		{
			PackedScene sproutScene = GD.Load<PackedScene>("res://Mechanics/Spring/Sprout/Sprout.tscn");
			Sprout sprout = sproutScene.Instantiate<Sprout>();
			sprout.Position = Position;
			CallDeferred(MethodName.AddSibling, sprout);
		}

		QueueFree();
	}

}
