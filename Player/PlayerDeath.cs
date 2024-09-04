using Godot;
using System;

public partial class PlayerDeath : Sprite2D
{
	[Export]
	private PackedScene _explosionScene;

	[Export]
	private PackedScene _transitionScene;

	[Export]
	private AudioStream _explosionSFX;

	[Export]
	private AudioStream _hurtSFX;

	public override void _Ready()
	{
		AudioManager.Instance.PlaySFX(_hurtSFX);
	}

	public void Explode()
	{
		ParticleEmitter explosion = _explosionScene.Instantiate<ParticleEmitter>();
		explosion.Position = Position;
		AddSibling(explosion);
		AudioManager.Instance.PlaySFX(_explosionSFX);
		GameManager.Instance.EmitSignal(GameManager.SignalName.ScreenShake, 0.4f);
	}

	public void Transition()
	{
		Transition transition = _transitionScene.Instantiate<Transition>();
		AddSibling(transition);
		QueueFree();
	}
}
