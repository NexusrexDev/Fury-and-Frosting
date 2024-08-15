using Godot;
using System;

public partial class PlatformActivator : ObjectActivator
{
	private Sprite2D _sprite2D;

	public override void _Ready()
	{
		base._Ready();
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		_deactivatable = true;
	}

	public override void VisualEnable()
	{
		_sprite2D.Frame = 1;
	}

	public override void VisualDisable()
	{
		_sprite2D.Frame = 0;
	}
}
