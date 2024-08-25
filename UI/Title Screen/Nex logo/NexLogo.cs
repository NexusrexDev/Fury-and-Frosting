using Godot;
using System;

public partial class NexLogo : Node2D
{
	[Export]
	private PackedScene _nextScene;

	[Export]
	private PackedScene _transitionScene;

	public void PlayMusic()
	{
		AudioManager.Instance.PlayMusic(MusicTitles.NexJingle);
	}

	public void AnimationEnd()
	{
		Transition transitionNode = _transitionScene.Instantiate<Transition>();
		transitionNode.NextScene = _nextScene;
		AddChild(transitionNode);
	}
}
