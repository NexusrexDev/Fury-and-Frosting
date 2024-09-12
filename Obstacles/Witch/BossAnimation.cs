using Godot;
using System;

public partial class BossAnimation : AnimationPlayer
{
	[Export]
	private AudioStream _laughSFX;

	[Export]
	private PackedScene _fadeAnimation;

	public override void _Ready()
	{
		switch (GameManager.Instance.BossIntroPlayed)
		{
			case false:
				Play("BossIntro");
				GameManager.Instance.BossIntroPlayed = true;
				break;
			case true:
				Play("BossIdle");
				break;
		}
	}

	public void PlayLaughSFX()
	{
		AudioManager.Instance.PlaySFX(_laughSFX);
	}

	public void CreateFade()
	{
		WitchFade witchFade = _fadeAnimation.Instantiate<WitchFade>();
		witchFade.Scale = new Vector2(1, 1);
		witchFade.Position = new Vector2(296, 152);
		AddSibling(witchFade);
	}
}
