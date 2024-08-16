using Godot;
using Godot.Collections;
using System;

public abstract partial class ObjectActivator : Area2D
{
	[Export]
	private Array<ActivatableObject> activatableObjects = new Array<ActivatableObject>();

	[Signal]
	public delegate void StateChangeEventHandler(bool active);

	private Timer _timer;

	private bool _activated = false;

	protected bool _deactivatable = false;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		if (_timer != null)
			_timer.Timeout += _On_Timer_Timeout;

		AreaEntered += _On_Area_Entered;

		foreach (ActivatableObject activatableObject in activatableObjects)
			StateChange += activatableObject.StateChange;
	}

	private void _On_Area_Entered(Area2D area)
	{
		if (area is PlayerSword && !_activated)
		{
			EmitSignal(SignalName.StateChange, true);
			_activated = true;
			VisualEnable();

			if (_deactivatable)
				_timer.Start();
		}
	}

	private void _On_Timer_Timeout()
	{
		EmitSignal(SignalName.StateChange, false);
		_activated = false;
		VisualDisable();
	}

	public abstract void VisualEnable();

	public abstract void VisualDisable();
}
