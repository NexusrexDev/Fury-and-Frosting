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

	public void Explode()
	{
		ParticleEmitter explosion = _explosionScene.Instantiate<ParticleEmitter>();
		explosion.Position = Position;
		GetParent().AddChild(explosion);
		AudioManager.Instance.PlaySFX(_explosionSFX);
		GameManager.Instance.EmitSignal(GameManager.SignalName.ScreenShake, 0.4f);
	}

	public void Transition()
	{
		Transition transition = _transitionScene.Instantiate<Transition>();
		GetParent().AddChild(transition);
		QueueFree();
	}
}
