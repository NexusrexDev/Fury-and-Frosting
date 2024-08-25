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

	public override void _ExitTree()
	{
		_playerReference.DashState -= OnDashStateChange;
		_playerReference.AttackState -= OnAttackStateChange;
		_playerReference.RageChanged -= OnRageChanged;

		SwitchIcon(_dashIcon, true);
		SwitchIcon(_swordIcon, true);
	}

	private void OnDashStateChange(bool state)
	{
		if (_dashState == state)
			return;

		_dashState = state;
		
		SwitchIcon(_dashIcon, state);

		ScaleIcon(_dashIcon);
	}

	private void OnAttackStateChange(bool state)
	{
		if (_attackState == state)
			return;

		_attackState = state;
		
		SwitchIcon(_swordIcon, state);

		ScaleIcon(_swordIcon);
	}

	private void OnRageChanged(float rage)
	{
		_rageMeter.Value = 24 - Mathf.Remap(rage, 0, 100, 0, 24);
	}

	private void SwitchIcon(TextureRect icon, bool state)
	{
		AtlasTexture texture = (icon.Texture as AtlasTexture);
		float startPos = (state) ? 0 : 16;
		texture.Region = new Rect2(startPos, 0, 16, 16);
	}

	private void ScaleIcon(TextureRect icon)
	{
		icon.Scale = new Vector2(1.1f, 1.1f);
		Tween tween = CreateTween();
		tween.TweenProperty(icon, "scale", new Vector2(1, 1), 0.2f);
	}
}
