using Godot;
using System;

public partial class WinCanvas : CanvasLayer
{
	[Export]
	public SceneNames NextScene;

	[Export]
	private PackedScene _transitionScene;

	public override void _Ready()
	{
		GetTree().Paused = true;
	}

	public void AnimationEnd()
	{
		Transition transitionNode = _transitionScene.Instantiate<Transition>();
		transitionNode.NextScene = NextScene;
		AddChild(transitionNode);
	}
}
