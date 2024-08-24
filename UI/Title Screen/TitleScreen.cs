using Godot;
using System;

public partial class TitleScreen : Node2D
{
	[Export]
	private Button _startButton;

	[Export]
	private PackedScene _transitionScene;

	[Export]
	private PackedScene _nextScene;

	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_startButton.GrabFocus();
		_startButton.Pressed += ButtonPressed;
	}

	private void ButtonPressed()
	{
		AudioManager.Instance.StopMusic(0.1f);
		_startButton.ReleaseFocus();
		_animationPlayer.Play("started");
		Transition transitionNode = _transitionScene.Instantiate<Transition>();
		transitionNode._nextScene = _nextScene;
		AddChild(transitionNode);
	}
}
