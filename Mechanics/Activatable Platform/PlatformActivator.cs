using Godot;
using Godot.Collections;
using System;

public partial class PlatformActivator : ObjectActivator
{
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[ExportGroup("Throwable Settings")]
	[Export]
	private bool _throwable = false;
	private Vector2 _startPosition;
	private bool _thrown = false;
	private VisibleOnScreenNotifier2D _notifier;
	private Vector2 _velocity;
	[Export]
	private PackedScene _explosionScene;

	private Sprite2D _sprite2D;

	[ExportGroup("Visual Settings")]
	[Export]
	private ParticleEmitter _emitter;
	[Export]
	private Array<AudioStream> _enableSFX;
	[Export]
	private Array<AudioStream> _disableSFX;

	private Tween _tween;
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		base._Ready();
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		_deactivatable = true;

		if (_throwable)
		{
			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			_animationPlayer.Stop();

			_notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
			_notifier.ScreenExited += OnScreenExited;

			_startPosition = Position;
		}
	}

	public override void _ExitTree()
	{
		if (_throwable)
			_notifier.ScreenExited -= OnScreenExited;
	}

	public override void _Process(double delta)
	{
		if (!_throwable || !_thrown || _activated)
			return;

		if (_velocity.Y < 500f)
			_velocity.Y += _gravity * (float)delta;

		Position += _velocity * (float)delta;
	}

	private void OnScreenExited()
	{
		_thrown = false;
		Position = _startPosition;
		_velocity = Vector2.Zero;
	}

	public void Throw()
	{
		_thrown = true;
		_velocity = new Vector2(0, -450f);
	}

	public override void VisualEnable()
	{
		SetVisuals();

		if (_tween != null)
			_tween.Kill();
		_sprite2D.Scale = new Vector2(1.5f, 1.5f);
		_tween = CreateTween();
		_tween.TweenProperty(_sprite2D, "scale", new Vector2(1, 1), 0.2f);

		PlaySFX(_enableSFX, true);

		GameManager.Instance.EmitSignal(GameManager.SignalName.ScreenShake, 0.15f);

		if (_throwable)
			_animationPlayer.Play("hover");
	}

	public override void VisualDisable()
	{
		SetVisuals();

		PlaySFX(_disableSFX, false);

        if (_throwable)
        {
            ParticleEmitter explosion = _explosionScene.Instantiate<ParticleEmitter>();
			explosion.Position = Position;
			AddSibling(explosion);

			Position = _startPosition;
			_animationPlayer.Stop();
		}
    }

	private void SetVisuals()
	{
		_sprite2D.Frame = _activated ? 1 : 0;
		if (_activated)
			_emitter.EmitParticles();
		else
			_emitter.StopParticles();
	}

	private void PlaySFX(Array<AudioStream> SFXArray, bool hiPass)
	{
		foreach (AudioStream SFXFile in SFXArray)
			AudioManager.Instance.PlaySFX(SFXFile);
		AudioManager.Instance.SetMusicHiPass(hiPass);
	}
}
