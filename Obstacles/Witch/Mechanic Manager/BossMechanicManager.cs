using Godot;
using Godot.Collections;
using System;

public partial class BossMechanicManager : Node2D
{
	[Export]
	private Witch _witchReference;

	[Export]
	private PackedScene _sproutReference;

	private Marker2D _sproutMarkerLeft, _sproutMarkerRight;

	private MagicWallCreator _magicWallCreatorRight, _magicWallCreatorLeft;

	[Export]
	private PlatformActivator _platformActivatorRight, _platformActivatorLeft;

	private AnimationPlayer _animationPlayer;

	private Node2D _wallWarning;

	[Export]
	private AudioStream _warningSFX;

	private int _currentPhase = 1;

	public override void _Ready()
	{
		_sproutMarkerLeft = GetNode<Marker2D>("SproutMarkerLeft");
		_sproutMarkerRight = GetNode<Marker2D>("SproutMarkerRight");
		_magicWallCreatorLeft = GetNode<MagicWallCreator>("MagicWallCreatorLeft");
		_magicWallCreatorRight = GetNode<MagicWallCreator>("MagicWallCreatorRight");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_wallWarning = GetNode<Node2D>("WallWarning");

		_witchReference.PhaseChange += OnPhaseChange;
		_witchReference.ThrowActivator += OnThrowActivator;
		_witchReference.WallActivator += OnWallActivator;
		_witchReference.Death += OnDeath;
	}

	private void OnDeath()
	{
		QueueFree();
	}

	private async void OnWallActivator(int lane)
	{
		AudioManager.Instance.PlaySFX(_warningSFX);
		_wallWarning.Scale = new Vector2(-lane, 1);
		_wallWarning.Position = new Vector2((lane==-1)?0 : 320, 128);
		_animationPlayer.Play("warning");

		await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

		switch (lane)
		{
			case -1:
				_magicWallCreatorLeft.CreateWall();
				break;
			case 1:
				_magicWallCreatorRight.CreateWall();
				break;
		}
	}

	private void OnThrowActivator(int lane)
	{
		switch (lane)
		{
			case -1:
				_platformActivatorRight.Throw();
				break;
			case 1:
				_platformActivatorLeft.Throw();
				break;
		}
	}

	private async void OnPhaseChange(int phaseNumber)
	{
		_currentPhase = phaseNumber;
		switch (_currentPhase)
		{
			case 2:
				await ToSignal(GetTree().CreateTimer(0.75f), "timeout");
				SproutCreator(_sproutMarkerLeft);
				SproutCreator(_sproutMarkerRight);
				break;
			case 3:
				SproutRemover();
				break;
		}
	}

	private void SproutCreator(Marker2D marker)
	{
		Sprout sprout = _sproutReference.Instantiate<Sprout>();
		sprout.Position = marker.Position;
		AddChild(sprout);
	}

	private void SproutRemover()
	{
		Array<Node> children = GetChildren();
		foreach (Node child in children)
		{
			if (child is IDisappearable disappearable)
			{
				disappearable.Disappear(false);
			}
		}
	}

	public override void _ExitTree()
	{
		_witchReference.PhaseChange -= OnPhaseChange;
		_witchReference.ThrowActivator -= OnThrowActivator;
	}
}
