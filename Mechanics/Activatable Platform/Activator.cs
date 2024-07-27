using Godot;
using System;

public partial class Activator : Area2D
{
	[Export]
	private ActivatablePlatform[] activatablePlatforms;

	private Timer _timer;

	private bool _activated = false;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");

		AreaEntered += _On_Area_Entered;
		_timer.Timeout += _On_Timer_Timeout;
	}

	private void _On_Area_Entered(Area2D area)
	{
		if (area.IsInGroup("Attack") && !_activated)
		{
			foreach (ActivatablePlatform platform in activatablePlatforms)
			{
				platform.Activate();
			}
			_activated = true;
			_timer.Start();
		}
	}

	private void _On_Timer_Timeout()
	{
		foreach (ActivatablePlatform platform in activatablePlatforms)
		{
			platform.Deactivate();
		}
		_activated = false;
	}
}
