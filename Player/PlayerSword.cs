using Godot;
using System;

public partial class PlayerSword : Area2D
{
	[Signal]
	public delegate void AttackEndEventHandler();

	public void AttackOver()
	{
		EmitSignal(SignalName.AttackEnd);
		QueueFree();
	}
}
