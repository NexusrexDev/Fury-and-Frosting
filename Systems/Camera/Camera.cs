using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Camera : Camera2D
{
	//Desired features:
	//Screenshake

	[Export]
	private Player _player;

	private Vector2 camPos;

	private Tween _tween;

	private float shakeValue = 0;
	private float limitLerp = 0.2f;

	public override void _Process(double delta)
	{
		handleScreenShake(delta);

		handleCameraFollow();
	}

	public void SetLimitRect(Rect2 rect)
	{
		if (_tween != null)
			_tween.Kill();
		_tween = CreateTween();
		_tween.TweenProperty(this, "limit_left", rect.Position.X, limitLerp);
		_tween.SetParallel();
		_tween.TweenProperty(this, "limit_top", rect.Position.Y, limitLerp);
		_tween.TweenProperty(this, "limit_right", rect.End.X, limitLerp);
		_tween.TweenProperty(this, "limit_bottom", rect.End.Y, limitLerp);
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

		var xPos = _player.Position.X - (320 / 2);
		var yPos = _player.Position.Y - (180 / 2);
		camPos = new Vector2(xPos, yPos);
		Position = camPos;
	}
}
