using Godot;
using System;

public partial class Sprout : Area2D, IDisappearable
{
	[Export]
	private PackedScene _springReference;

	[Export]
	private PackedScene _explosionReference;

	[Export]
	private AudioStream _explosionSFX;

	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is WitchProjectile && !_animationPlayer.IsPlaying())
			Disappear(true);
	}

	public void Disappear(bool createSpring)
	{
		ParticleEmitter explosionParticles = _explosionReference.Instantiate<ParticleEmitter>();
		explosionParticles.Position = new Vector2(Position.X, Position.Y - 10);
		AddSibling(explosionParticles);
		AudioManager.Instance.PlaySFX(_explosionSFX);

		if (createSpring)
		{
			Spring spring = _springReference.Instantiate<Spring>();
			spring.Position = Position;
			spring.OneTimeUse = true;
			CallDeferred(MethodName.AddSibling, spring);
		}

		QueueFree();
	}
}
