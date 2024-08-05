﻿using Godot;
using Godot.Collections;
using System;

public partial class Dash : PlayerState
{
	private float _previousSpeed;
	private int _dashnum = 0;
	
	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		Vector2 velocity = _player.Velocity;
		_previousSpeed = velocity.X;
		velocity.X = Player.Speed * _player.Direction * 2f;
		velocity.Y = 0f;
		_player.Velocity = velocity;

        _player.SetRotation(4 * _player.Direction);
		_player.SetScale(new Vector2(1.2f, 0.8f));

		_player.CanDash = false;

        _player.Timer.WaitTime = 0.25f;
		_player.Timer.Timeout += OnTimerTimeout;
		_player.Timer.Start();
	}

	private void OnTimerTimeout()
	{
		if (_player.IsOnFloor())
		{
			if (Mathf.IsEqualApprox(_previousSpeed, 0f))
				StateMachine.TransitionTo(Idle);
			else
				StateMachine.TransitionTo(Run);
		}
		else
			StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "canJump", false } });
	}

	public override void Exit()
	{
		//Reset the dash
		_player.Timer.Timeout -= OnTimerTimeout;
		Vector2 velocity = _player.Velocity;
		velocity.X = _previousSpeed;
		_player.Velocity = velocity;
	}
}