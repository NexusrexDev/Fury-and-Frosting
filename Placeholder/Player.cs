using Godot;
using Godot.Collections;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public sbyte Direction { get; set; } = 1;

	[Export]
	private Label _velocityLabel;
	[Export]
	public Timer Timer;
	[Export]
	public Timer IFrameTimer;
	
	private StateMachine _stateMachine;

	public override void _Ready()
	{
		_stateMachine = GetNode<StateMachine>("StateMachine");
		Area2D hitbox = GetNode<Area2D>("Hitbox");
		hitbox.AreaEntered += _On_Hitbox_Collision;
	}

	public override void _PhysicsProcess(double delta)
	{
		_velocityLabel.Text = $"Velocity: {Velocity}\nState: {_stateMachine.CurrentState.Name}";
        MoveAndSlide();
    }

	public void Jump()
	{
        _stateMachine.TransitionTo("Air", new Dictionary<string, Variant> { { "do_jump", true } });
    }

	private void _On_Hitbox_Collision(Area2D area)
	{
		if (area.IsInGroup("Damage") && IFrameTimer.IsStopped())
		{
			if (_stateMachine.CurrentState.Name.Equals("Dash"))
				return;
			Timer.Stop();
			_stateMachine.TransitionTo("Hurt");
		}
	}
}
