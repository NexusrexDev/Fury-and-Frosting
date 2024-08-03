using Godot;
using Godot.Collections;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 150.0f;
	public const float JumpVelocity = -325.0f;
	public const float MaxFallSpeed = 500.0f;

	private sbyte direction = 1;
	public sbyte Direction
	{
		get { return direction; }
		set
		{
			direction = value;
			SwordPosition.Scale = new Vector2(direction, 1);
			SwordPosition.Position = new Vector2(direction * 12, 0);
			_sprite.FlipH = direction == -1;
		}
	}

	[Export]
	private Label _velocityLabel;
	[Export]
	public Timer Timer;
	[Export]
	public Timer IFrameTimer;
	[Export]
	public PackedScene SwordScene;
	[Export]
	public Marker2D SwordPosition;

	public AnimationPlayer AnimationPlayer;
	
	private StateMachine _stateMachine;

	private Sprite2D _sprite;

	public override void _Ready()
	{
		_stateMachine = GetNode<StateMachine>("StateMachine");
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		Area2D hitbox = GetNode<Area2D>("Hitbox");
		hitbox.AreaEntered += _On_Hitbox_Collision;
	}

	public override void _Process(double delta)
	{
		_sprite.Scale = new Vector2(Mathf.MoveToward(_sprite.Scale.X, 1, (float)delta), Mathf.MoveToward(_sprite.Scale.Y, 1, (float)delta));
	}

	public override void _PhysicsProcess(double delta)
	{
		_velocityLabel.Text = $"Velocity: {Velocity}\nState: {_stateMachine.CurrentState.Name}";
		MoveAndSlide();
	}

	public void Jump()
	{
		_stateMachine.TransitionTo(PlayerState.Air, new Dictionary<string, Variant> { { "do_jump", true } });
	}

	public void SetScale(Vector2 scale)
	{
		_sprite.Scale = scale;
	}

	public void SetRotation(float rotation)
	{
		_sprite.RotationDegrees = rotation;
	}

	private void _On_Hitbox_Collision(Area2D area)
	{
		if (area.IsInGroup("Damage") && IFrameTimer.IsStopped())
		{
			if (_stateMachine.CurrentState.Name.Equals("Dash"))
				return;
			Timer.Stop();
			_stateMachine.TransitionTo(PlayerState.Hurt);
		}
	}
}
