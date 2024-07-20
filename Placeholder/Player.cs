using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public sbyte Direction { get; set; } = 1;

	[Export]
	private Label _velocityLabel;
	
	private StateMachine _stateMachine;

	public override void _Ready()
	{
		_stateMachine = GetNode<StateMachine>("StateMachine");
	}

	public override void _PhysicsProcess(double delta)
	{
		_velocityLabel.Text = $"Velocity: {Velocity}\nState: {_stateMachine.CurrentState.Name}";
        MoveAndSlide();
    }
}
