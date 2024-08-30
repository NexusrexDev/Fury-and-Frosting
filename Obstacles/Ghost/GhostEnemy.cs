using Godot;
using System;

public partial class GhostEnemy : Area2D, IDamaging
{
	[Export(PropertyHint.Range, "0, 100")]
	public int Damage { get; set; }

	[Export]
	private PackedScene _explosionScene;

	[Export]
	private AudioStream _explosionSFX;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is PlayerSword)
		{
			ParticleEmitter explosion = _explosionScene.Instantiate<ParticleEmitter>();
			explosion.Position = Position;
			GetParent().AddChild(explosion);
			AudioManager.Instance.PlaySFX(_explosionSFX);
			GameManager.Instance.EmitSignal(GameManager.SignalName.ScreenShake, 0.25f);
			QueueFree();
		}

	}
}
