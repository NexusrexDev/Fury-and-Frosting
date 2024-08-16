using Godot;
using System;

public partial class Door : ActivatableObject
{
	public override void Activate()
	{
		//Cool explosions go here
		QueueFree();
	}

	public override void Deactivate() { }
}
