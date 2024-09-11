using Godot;
using System;

public partial class TitleScreen : Node2D
{
	[Export]
	private AudioStream _startSFX;

	[ExportGroup("Buttons")]
	[Export]
	private Button _startButton;
	[Export]
	private Button _exitButton;

	[ExportGroup("Transition Settings")]
	[Export]
	private PackedScene _transitionScene;
	[Export]
	private SceneNames _nextScene;

	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_startButton.GrabFocus();
		_startButton.Pressed += ButtonPressed;
		_exitButton.Pressed += () => { GetTree().Quit(); };
	}

	private void ButtonPressed()
	{
		AudioManager.Instance.StopMusic(0.1f);
		_startButton.ReleaseFocus();
		AudioManager.Instance.PlaySFX(_startSFX);
		_animationPlayer.Play("started");
		Transition transitionNode = _transitionScene.Instantiate<Transition>();
		transitionNode.NextScene = _nextScene;
		AddChild(transitionNode);
	}
}
