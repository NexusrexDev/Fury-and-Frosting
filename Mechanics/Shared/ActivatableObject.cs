using Godot;
using System;

[GlobalClass]
public abstract partial class ActivatableObject : StaticBody2D
{
	public void StateChange(bool active)
	{
		if (active)
			Activate();
		else
			Deactivate();
	}

	public abstract void Activate();

	public abstract void Deactivate();
}
