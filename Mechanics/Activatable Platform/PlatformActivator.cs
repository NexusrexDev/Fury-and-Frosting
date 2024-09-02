using Godot;
using Godot.Collections;
using System;

public partial class PlatformActivator : ObjectActivator
{
	private Sprite2D _sprite2D;

	[Export]
	private ParticleEmitter _emitter;

	[Export]
	private Array<AudioStream> _enableSFX;

	[Export]
	private Array<AudioStream> _disableSFX;

	private Tween _tween;

	public override void _Ready()
	{
		base._Ready();
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		_deactivatable = true;
	}

	public override void VisualEnable()
	{
		_sprite2D.Frame = 1;
		_emitter.EmitParticles();

		if (_tween != null)
			_tween.Kill();
		_sprite2D.Scale = new Vector2(1.5f, 1.5f);
		_tween = CreateTween();
		_tween.TweenProperty(_sprite2D, "scale", new Vector2(1, 1), 0.2f);

		foreach (AudioStream SFXFile in _enableSFX)
			AudioManager.Instance.PlaySFX(SFXFile);

		GameManager.Instance.EmitSignal(GameManager.SignalName.ScreenShake, 0.15f);
	}

	public override void VisualDisable()
	{
		_sprite2D.Frame = 0;
		_emitter.StopParticles();

		foreach (AudioStream SFXFile in _disableSFX)
			AudioManager.Instance.PlaySFX(SFXFile);
	}
}
