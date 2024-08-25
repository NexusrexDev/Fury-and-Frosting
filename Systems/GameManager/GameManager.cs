using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	[Signal]
	public delegate void GameOverEventHandler();

	[Signal]
	public delegate void ScreenShakeEventHandler(float value);

	public override void _Ready()
	{
		Instance = this;
		ProcessMode = ProcessModeEnum.Always;
	}

	public async void FreezeFrame(float timeScale, float duration)
	{
		Engine.TimeScale = timeScale;
		await ToSignal(GetTree().CreateTimer(duration * timeScale), SceneTreeTimer.SignalName.Timeout);
		Engine.TimeScale = 1.0;
	}
}
