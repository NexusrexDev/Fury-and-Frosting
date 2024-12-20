using Godot;
using Godot.Collections;
using System;

public enum SceneNames
{
	None = -1,
	NexLogo,
	TitleScreen,
	StorySegment,
	Level1,
	Level2,
	Level3,
	Level4,
	BossFight,
	CreditsSegment
}

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	[Signal]
	public delegate void GameOverEventHandler();

	[Signal]
	public delegate void ScreenShakeEventHandler(float value);

	public static readonly Dictionary<SceneNames, PackedScene> Scenes = new Dictionary<SceneNames, PackedScene>
	{
		{ SceneNames.NexLogo, GD.Load<PackedScene>("res://UI/Title Screen/Nex logo/nex_logo.tscn") },
		{ SceneNames.TitleScreen, GD.Load<PackedScene>("res://UI/Title Screen/titlescreen.tscn") },
		{ SceneNames.StorySegment, GD.Load<PackedScene>("res://UI/Story Segment/story_segment.tscn") },
		{ SceneNames.Level1, GD.Load<PackedScene>("res://Levels/level_1.tscn") },
		{ SceneNames.Level2, GD.Load<PackedScene>("res://Levels/level_2.tscn") },
		{ SceneNames.Level3, GD.Load<PackedScene>("res://Levels/level_3.tscn") },
		{ SceneNames.Level4, GD.Load<PackedScene>("res://Levels/level_4.tscn") },
		{ SceneNames.BossFight, GD.Load<PackedScene>("res://Levels/boss_fight.tscn") },
		{ SceneNames.CreditsSegment, GD.Load<PackedScene>("res://UI/Ending Segment/ending_segment.tscn") }
	};

	public bool BossIntroPlayed = false;

	public override void _Ready()
	{
		Instance = this;
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.F4))
		{
			Image image = GetViewport().GetTexture().GetImage();
			image.SavePng($"user://Screenshot-{Time.GetUnixTimeFromDatetimeString}.png");
		}
	}

	public async void FreezeFrame(float timeScale, float duration)
	{
		Engine.TimeScale = timeScale;
		await ToSignal(GetTree().CreateTimer(duration * timeScale), SceneTreeTimer.SignalName.Timeout);
		Engine.TimeScale = 1.0;
	}
}
