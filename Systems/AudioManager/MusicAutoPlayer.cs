using Godot;
using System;

public partial class MusicAutoPlayer : Node
{
	[Export]
	private MusicTitles _trackName;

	[Export]
	private float _fadeTime = 0f;

	[Export]
	private bool _playOnStart = true;

	public override void _Ready()
	{
		if (_playOnStart)
			PlayMusic();
	}

	public void PlayMusic()
	{
		AudioManager.Instance.PlayMusic(_trackName, _fadeTime);
	}
}
