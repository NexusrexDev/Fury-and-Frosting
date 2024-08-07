using Godot;
using Godot.Collections;
using System;

public partial class Activator : Area2D
{
	[Export]
	private Array<ActivatablePlatform> activatablePlatforms;

	[Signal]
	public delegate void StateChangeEventHandler(bool active);

	private Timer _timer;

	private bool _activated = false;

	private Sprite2D _sprite2D;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_sprite2D = GetNode<Sprite2D>("Sprite2D");

		AreaEntered += _On_Area_Entered;
		_timer.Timeout += _On_Timer_Timeout;

		foreach (ActivatablePlatform platform in activatablePlatforms)
			this.StateChange += platform.StateChange;
	}

	private void _On_Area_Entered(Area2D area)
	{
		if (area.IsInGroup("Attack") && !_activated)
		{
			EmitSignal(SignalName.StateChange, true);
			_activated = true;
			_sprite2D.Frame = 1;
			_timer.Start();
		}
	}

	private void _On_Timer_Timeout()
	{
		EmitSignal(SignalName.StateChange, false);
		_activated = false;
		_sprite2D.Frame = 0;
	}
}
