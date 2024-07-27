using Godot;
using System;

public partial class ExplosiveTile : StaticBody2D
{
	[Export]
	private bool _centerTile;

	private RayCast2D[] _rayCasts = new RayCast2D[4];

	private Sprite2D _sprite;

	private Timer timer;

	public override void _Ready(){
		_sprite = GetNode<Sprite2D>("Sprite2D");

		_rayCasts[0] = GetNode<RayCast2D>("RayLeft");
		_rayCasts[1] = GetNode<RayCast2D>("RayRight");
		_rayCasts[2] = GetNode<RayCast2D>("RayUp");
		_rayCasts[3] = GetNode<RayCast2D>("RayDown");

		Area2D area = GetNode<Area2D>("HitBox");
		area.AreaEntered += _On_Area_Entered;

		timer = GetNode<Timer>("Timer");
	}

	private void _On_Area_Entered(Area2D area)
	{
		if (area.IsInGroup("Attack") && _centerTile)
		{
			Explode(0f);
		}
	}

	public async void Explode(float timeDelay)
	{
		//Explosion effects go here
		if (timeDelay > 0)
		{
			timer.WaitTime = timeDelay;
			timer.Start();
			await ToSignal(timer, "timeout");
		}
		
		GD.Print(Name + " Exploded");
		foreach (var ray in _rayCasts)
		{
			if (ray.IsColliding())
			{
				ExplosiveTile tile = ray.GetCollider() as ExplosiveTile;
				if (tile != null)
				{
					tile.Explode(0.25f);
				}
			}
		}
		QueueFree();
	}
}
