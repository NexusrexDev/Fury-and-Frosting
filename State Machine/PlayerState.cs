using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class PlayerState : State
{
	protected Player _player;

	//State constants
	public static StringName Idle = new StringName("Idle");
	public static StringName Run = new StringName("Run");
	public static StringName Air = new StringName("Air");
	public static StringName Attack = new StringName("Attack");
	public static StringName Dash = new StringName("Dash");
	public static StringName Hurt = new StringName("Hurt");


	public override async void _Ready()
	{
		await ToSignal(Owner, Node.SignalName.Ready);
		_player = Owner as Player;
		Debug.Assert(_player != null, "The owner must be a Player node.");
	}
}
