using Godot;
using Godot.Collections;
using System;

public partial class Air : PlayerState
{
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private bool _jumped = false;
    private ulong _fallTime;
    
    public override void Enter(Dictionary<string, Variant> _message = null)
    {
        if (_message != null)
        {
            if (_message.ContainsKey("do_jump"))
            {
                Vector2 velocity = _player.Velocity;
                velocity.Y = Player.JumpVelocity;
                _player.Velocity = velocity;
                _jumped = true;
            }
            if (_message.ContainsKey("canJump"))
                _jumped = (bool)_message["canJump"];
        }
        else
        {
            _fallTime = Time.GetTicksMsec();
        }
    }

    public override void Exit()
    {
        _jumped = false;
        _fallTime = 0;
    }

    public override void PhysicsProcess(double delta)
    {
        Vector2 velocity = _player.Velocity;
        //Horizontal movement
        float direction = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
        if (direction != 0)
        {
            velocity.X = direction * Player.Speed;
            _player.Direction = (sbyte)direction;
        }

        //Coyote time
        if (Input.IsActionJustPressed("game_jump"))
        {
            if (!_jumped)
            {
                if (Time.GetTicksMsec() - _fallTime < 150)
                {
                    velocity.Y = Player.JumpVelocity;
                    _jumped = true;
                }
            }
        }

        if (!_player.IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
        }

        _player.Velocity = velocity;

        //Transitioning
        if (_player.IsOnFloor())
        {
            if (Mathf.IsEqualApprox(velocity.X, 0))
                StateMachine.TransitionTo("Idle", new Dictionary<string, Variant> { { "landed", true } });
            else
                StateMachine.TransitionTo("Run", new Dictionary<string, Variant> { { "landed", true } });
        }

        //Transitioning to the Dash state
        if (Input.IsActionJustPressed("game_dash"))
            StateMachine.TransitionTo("Dash");
    }

}
