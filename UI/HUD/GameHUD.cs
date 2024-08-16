using Godot;
using System;
using System.Diagnostics;

public partial class GameHUD : CanvasLayer
{
	[Export]
	private TextureRect _dashIcon, _swordIcon;

	[Export]
	private Player _playerReference;

	public override void _Ready()
	{
		Debug.Assert(_playerReference != null, "The player node must be included");
		_playerReference.DashState += OnDashStateChange;
		_playerReference.AttackState += OnAttackStateChange;
	}

	private void OnDashStateChange(bool state)
	{
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
	}

	private void OnAttackStateChange(bool state)
	{
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
	}
}
