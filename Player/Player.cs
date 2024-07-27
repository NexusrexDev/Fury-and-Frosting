using Godot;
using Godot.Collections;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	private sbyte direction = 1;
	public sbyte Direction
	{
		get { return direction; }
		set
		{
			direction = value;
			damageBox.Position = new Vector2(48 * direction, 0);
		}
	}

	[Export]
	private Label _velocityLabel;
	[Export]
	public Timer Timer;
	[Export]
	public Timer IFrameTimer;
	[Export]
	public Area2D damageBox;

	public AnimationPlayer AnimationPlayer;
	
	private StateMachine _stateMachine;

	public override void _Ready()
	{
		_stateMachine = GetNode<StateMachine>("StateMachine");
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Area2D hitbox = GetNode<Area2D>("Hitbox");
		hitbox.AreaEntered += _On_Hitbox_Collision;
		damageBox.Monitorable = false;
		damageBox.Visible = false;
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

	public void SetDamageBox(bool active)
	{
		damageBox.Monitorable = active;
		damageBox.Visible = active;
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
