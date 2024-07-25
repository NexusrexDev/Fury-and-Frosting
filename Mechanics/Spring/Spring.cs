using Godot;
using System;

public partial class Spring : StaticBody2D
{
	public override void _Ready()
	{
		Area2D bounceArea = GetNode<Area2D>("BounceArea");
        bounceArea.BodyEntered += _On_BounceArea_Collision;
	}

	private void _On_BounceArea_Collision(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            Player player = body as Player;
            player.Jump();
        }
    }

}
