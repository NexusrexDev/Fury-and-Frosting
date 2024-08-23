using Godot;
using System;

public partial class MusicAutoPlayer : Node
{
	[Export]
	private MusicTitles _trackName;

	public override void _Ready()
	{
		AudioManager.Instance.PlayMusic(_trackName);
	}
}
