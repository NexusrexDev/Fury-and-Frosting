using Godot;
using System;
using System.Diagnostics;

public partial class GameHUD : CanvasLayer
{
	[Export]
	private TextureRect _dashIcon, _swordIcon;

	[Export]
	private TextureProgressBar _rageMeter;

	[Export]
	private Player _playerReference;

	private bool _dashState = true, _attackState = true;

	public override void _Ready()
	{
		Debug.Assert(_playerReference != null, "The player node must be included");

		_playerReference.DashState += OnDashStateChange;
		_playerReference.AttackState += OnAttackStateChange;
		_playerReference.RageChanged += OnRageChanged;
	}

	private void OnDashStateChange(bool state)
	{
		if (_dashState == state)
			return;

		_dashState = state;
		AtlasTexture texture = (_dashIcon.Texture as AtlasTexture);
		switch (state)
		{
			case true:
				texture.Region = new Rect2(0, 0, 16, 16);
				break;
			case false:
				texture.Region = new Rect2(16, 0, 16, 16);
				break;
		}
		ScaleIcon(_dashIcon);
	}

	private void OnAttackStateChange(bool state)
	{
		if (_attackState == state)
			return;

		_attackState = state;
		AtlasTexture texture = (_swordIcon.Texture as AtlasTexture);
		switch (state)
		{
			case true:
				texture.Region = new Rect2(0, 0, 16, 16);
				break;
			case false:
				texture.Region = new Rect2(16, 0, 16, 16);
				break;
		}
		ScaleIcon(_swordIcon);
	}

	private void OnRageChanged(float rage)
	{
		_rageMeter.Value = 24 - Mathf.Remap(rage, 0, 100, 0, 24);
	}

	private void ScaleIcon(TextureRect icon)
	{
		icon.Scale = new Vector2(1.1f, 1.1f);
		Tween tween = CreateTween();
		tween.TweenProperty(icon, "scale", new Vector2(1, 1), 0.2f);
	}
}
