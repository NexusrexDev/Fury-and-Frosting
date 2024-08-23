using Godot;
using System;

public partial class MusicAutoPlayer : Node
{
	[Export]
	private MusicTitles _trackName;

	[Export]
	private float _fadeTime = 0f;

	public override void _Ready()
	{
		AudioManager.Instance.PlayMusic(_trackName, _fadeTime);
	}
}
