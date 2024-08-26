using Godot;
using Godot.Collections;
using System;

public partial class Attack : PlayerState
{
	[Export]
	private AudioStream _attackSFX;
	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		_player.CanAttack = false;
		_player.Velocity = new Vector2(_player.Velocity.X, 0);

		_player.SetScale(new Vector2(1.15f, 0.9f));
        _player.SetRotation(4 * _player.Direction);
		AudioManager.Instance.PlaySFX(_attackSFX);

        PlayerSword sword = _player.SwordScene.Instantiate() as PlayerSword;
		_player.SwordPosition.AddChild(sword);
		sword.AttackEnd += OnAttackEnd;
	}

	private void OnAttackEnd()
	{
		if (_player.IsOnFloor())
		{
			if (_player.Velocity == Vector2.Zero)
				StateMachine.TransitionTo(Idle);
			else
				StateMachine.TransitionTo(Run);
		}
		else
			StateMachine.TransitionTo(Air, new Dictionary<string, Variant> { { "canJump", false } });
	}
}
