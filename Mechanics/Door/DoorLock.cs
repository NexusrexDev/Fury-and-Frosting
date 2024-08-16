using Godot;
using System;

public partial class DoorLock : ObjectActivator
{
	public override void VisualDisable() { }

	public override void VisualEnable()
	{
		//Visual code here
		QueueFree();
	}
}
