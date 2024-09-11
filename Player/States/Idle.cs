using Godot;
using Godot.Collections;
using System;

public partial class Idle : PlayerState
{
	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		_player.SetRotation(0);
		_player.CanDash = true;
		_player.CanAttack = true;
		if (_message != null)
		{
			if (_message.ContainsKey("landed"))
			{
				_player.Velocity = Vector2.Zero;
				_player.SetScale(new Vector2(1.25f, 0.75f));
				_player.EmitLandingParticles();
			}
		}
	}

	public override void Update(double delta)
	{
		if (!_player.IsControllable)
			return;

		HandleInput();

		if (_player.IsRaging)
			_player.Rage += (float)delta * 10f;
	}

	public override void PhysicsProcess(double delta)
	{
		if (!_player.IsOnFloor())
			StateMachine.TransitionTo(Air);
	}

	private void HandleInput()
	{
		float direction = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
		if (direction != 0)
			StateMachine.TransitionTo(Run, new Dictionary<string, Variant> { { "direction", direction } });

		if (Input.IsActionJustPressed("game_jump") && _player.IsOnFloor())
			StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "do_jump", true } });

		if (Input.IsActionJustPressed("game_attack") && _player.CanAttack)
			StateMachine.TransitionTo(Attack);

		if (Input.IsActionJustPressed("game_dash") && _player.CanDash)
			StateMachine.TransitionTo(Dash);
	}
}
