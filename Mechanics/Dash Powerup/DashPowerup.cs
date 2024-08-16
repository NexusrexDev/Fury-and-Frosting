using Godot;
using System;

public partial class DashPowerup : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += _On_Body_Entered;
	}

	private void _On_Body_Entered(Node2D body)
	{
		if (body is Player)
		{
			Player player = body as Player;
			player.CanDash = true;

			//Cool particles go here

			QueueFree();
		}
	}
}
