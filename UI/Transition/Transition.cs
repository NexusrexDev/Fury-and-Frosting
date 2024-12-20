using Godot;
using System;

public partial class Transition : CanvasLayer
{
	private enum TransitionType
	{
		ENTRANCE,
		EXIT
	}

	[Export]
	private TransitionType _transitionType;

	[Export]
	public SceneNames NextScene = SceneNames.None;

	public override void _Ready()
	{
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		switch (_transitionType)
		{
			case TransitionType.ENTRANCE:
				animationPlayer.Play("Entrance");
				Visible = true;
				break;
			case TransitionType.EXIT:
				animationPlayer.Play("Exit");
				break;
		}
		animationPlayer.AnimationFinished += OnAnimationFinished;
	}

	private void OnAnimationFinished(StringName animationName)
	{
		if (_transitionType == TransitionType.EXIT)
		{
			GetTree().Paused = false;

			if (NextScene != SceneNames.None)
				GetTree().ChangeSceneToPacked(GameManager.Scenes[NextScene]);
			else
				GetTree().ReloadCurrentScene();
		}
		else
			QueueFree();
	}
}
