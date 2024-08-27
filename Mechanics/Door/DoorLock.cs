using Godot;
using System;

public partial class DoorLock : ObjectActivator
{
	[Export]
	private PackedScene _explosionScene;

	[Export]
	private AudioStream _explosionSFX;

	public override void VisualDisable() { }

	public override void VisualEnable()
	{
		ParticleEmitter explosion = _explosionScene.Instantiate<ParticleEmitter>();
		explosion.Position = Position;
		GetParent().AddChild(explosion);
		AudioManager.Instance.PlaySFX(_explosionSFX);
		GameManager.Instance.EmitSignal(GameManager.SignalName.ScreenShake, 0.25f);
		QueueFree();
	}
}
