using Godot;
using System;

public partial class NexLogo : Node2D
{
	[Export]
	private PackedScene _nextScene;

	public void PlayMusic()
	{
		AudioManager.Instance.PlayMusic(MusicTitles.NexJingle);
	}

	public void AnimationEnd()
	{
		GetTree().ChangeSceneToPacked(_nextScene);
	}
}
