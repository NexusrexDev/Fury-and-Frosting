using Godot;
using System;

public partial class WinCanvas : CanvasLayer
{
	[Export]
	private PackedScene _nextScene;

	[Export]
	private PackedScene _transitionScene;

	public override void _Ready()
	{
		GetTree().Paused = true;
	}

	public void AnimationEnd()
	{
		Transition transitionNode = _transitionScene.Instantiate<Transition>();
		transitionNode.NextScene = _nextScene;
		AddChild(transitionNode);
	}
}
