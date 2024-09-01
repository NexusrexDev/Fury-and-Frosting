using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Camera : Camera2D
{
	[Export]
	private Player _player;

	private Vector2 camPos;

	private Tween _tween;

	private float shakeValue = 0;
	private float limitLerp = 0.2f;

	public override void _Ready()
	{
		GameManager.Instance.ScreenShake += SetShake;
		GameManager.Instance.GameOver += () => { _player = null; };
		TreeExiting += () =>
		{
			GameManager.Instance.ScreenShake -= SetShake;
			GameManager.Instance.GameOver -= () => { _player = null; };
		};
	}

	public override void _Process(double delta)
	{
		handleScreenShake(delta);

		handleCameraFollow();
	}

	public void SetLimitRect(Rect2 rect)
	{
		PositionSmoothingEnabled = true;

		LimitLeft = (int) rect.Position.X;
		LimitTop = (int) rect.Position.Y;
		LimitRight = (int) rect.End.X;
		LimitBottom = (int) rect.End.Y;

		if (_tween != null)
			_tween.Kill();
		_tween = CreateTween();
		_tween.TweenProperty(this, "position_smoothing_enabled", false, 0.4);
	}

	public void SetShake(float value)
	{
		shakeValue = value;
	}

	private void handleScreenShake(double delta)
	{
		if (shakeValue > 0)
		{
			shakeValue = Mathf.Max(0, shakeValue - ((float)delta * 0.8f));
			var shake = new Vector2((float)GD.RandRange(-shakeValue, shakeValue) * 8, (float)GD.RandRange(-shakeValue, shakeValue) * 8);
			Offset = shake;
		}
	}

	private void handleCameraFollow()
	{
		if (_player == null)
			return;

		var xPos = _player.Position.X - ((640 / Zoom.X)/2);
		var yPos = _player.Position.Y - ((360 / Zoom.Y)/2);
		camPos = new Vector2(xPos, yPos);
		Position = camPos;
	}
}
