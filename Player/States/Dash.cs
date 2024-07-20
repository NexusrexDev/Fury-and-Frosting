using Godot;
using Godot.Collections;
using System;

public partial class Dash : PlayerState
{
    private float _previousSpeed;

    public override async void Enter(Dictionary<string, Variant> _message = null)
    {
        Vector2 velocity = _player.Velocity;
        _previousSpeed = velocity.X;
        velocity.X = Player.Speed * _player.Direction * 2f;
        velocity.Y = 0f;
        _player.Velocity = velocity;

        await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
        
        if (_player.IsOnFloor())
            StateMachine.TransitionTo("Idle");
        else
            StateMachine.TransitionTo("Air", new Dictionary<string, Variant> { { "canJump", false } });
    }

    public override void Exit()
    {
        //Reset the dash
        Vector2 velocity = _player.Velocity;
        velocity.X = _previousSpeed;
        _player.Velocity = velocity;
    }
}