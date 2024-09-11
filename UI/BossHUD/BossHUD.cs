using Godot;
using System;

public partial class BossHUD : CanvasLayer
{
	[Export]
	private TextureProgressBar _healthProgress;

	[Export]
	private Witch _witchReference;

	public override void _Ready()
	{
		_healthProgress.MaxValue = Witch.MaxHealth;
		_witchReference.HealthChange += OnHealthChange;
		_witchReference.Death += OnDeath;
	}

	private void OnDeath()
	{
		QueueFree();
	}

	public override void _ExitTree()
	{
		_witchReference.HealthChange -= OnHealthChange;
		_witchReference.Death -= OnDeath;
	}

	private void OnHealthChange(int newHealth)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(_healthProgress, "value", newHealth, 0.2f);
	}
}
