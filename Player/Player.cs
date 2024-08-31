using Godot;
using Godot.Collections;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 150.0f;
	public const float JumpVelocity = -325.0f;
	public const float MaxFallSpeed = 500.0f;

	[Signal]
	public delegate void RageChangedEventHandler(float rage);

	[Signal]
	public delegate void DashStateEventHandler(bool newState);

	[Signal]
	public delegate void AttackStateEventHandler(bool newState);

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
			_runParticles.Position = new Vector2(-8 * direction, 7);
			_runParticles.Direction = new Vector2(-direction, 1);
			_runParticles.TangentialAccelMax = (direction == 1) ? 20f : -6f;
			_runParticles.TangentialAccelMin = (direction == 1) ? 6f : -20f;
			_dashParticles.Position = new Vector2(32 * direction, 0);
			CollisionRayCast.TargetPosition = new Vector2(9 * direction, 0);
		}
	}

	private float _rage = 0.0f;
	public float Rage
	{
		get { return _rage; }
		set
		{
			_rage = Mathf.Clamp(value, 0, 100);
			EmitSignal(SignalName.RageChanged, _rage);
			if (_rage >= 100)
				GameOver();
		}
	}

	private bool _canDash = true, _canAttack = true;
	public bool CanDash
	{
		get { return _canDash; }
		set
		{
			_canDash = value;
			EmitSignal(SignalName.DashState, _canDash);
		}
	}
	public bool CanAttack
	{
		get { return _canAttack; }
		set
		{
			_canAttack = value;
			EmitSignal(SignalName.AttackState, _canAttack);
		}
	}

	[Export]
	public bool IsControllable = false;

	[Export]
	public Timer Timer;
	[Export]
	public Timer IFrameTimer;
	[Export]
	public PackedScene SwordScene;
	[Export]
	public Marker2D SwordPosition;
	[Export]
	public RayCast2D CollisionRayCast;
	
	private StateMachine _stateMachine;

	private Sprite2D _sprite;

	private CpuParticles2D _runParticles;
	private ParticleEmitter _landParticles, _dashParticles;
	private DashGhost _dashGhost;

	[Export]
	private PackedScene _deathAnimation;

	public override void _Ready()
	{
		_stateMachine = GetNode<StateMachine>("StateMachine");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		SetSpriteFlash(false);
		SetSpriteAlpha(1);

		Area2D hitbox = GetNode<Area2D>("Hitbox");
		hitbox.AreaEntered += _On_Hitbox_Collision;

		IFrameTimer.Timeout += () => { SetSpriteAlpha(1); };

		_runParticles = GetNode<CpuParticles2D>("RunParticles");
		_landParticles = GetNode<ParticleEmitter>("LandParticles");
		_dashParticles = GetNode<ParticleEmitter>("DashParticles");
		_dashGhost = GetNode<DashGhost>("DashGhost");
		_dashGhost.Init(_sprite);
	}

	public override void _Process(double delta)
	{
		_sprite.Scale = new Vector2(Mathf.MoveToward(_sprite.Scale.X, 1, (float)delta), Mathf.MoveToward(_sprite.Scale.Y, 1, (float)delta));
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public void SpringJump()
	{
		CanDash = true;
		CanAttack = true;
		_stateMachine.TransitionTo(PlayerState.Air, new Dictionary<string, Variant> { { "do_springJump", true } });
	}

	private void GameOver()
	{
		GameManager.Instance.EmitSignal(GameManager.SignalName.GameOver);
		PlayerDeath playerDeath = _deathAnimation.Instantiate<PlayerDeath>();
		playerDeath.Position = Position;
		playerDeath.FlipH = direction == -1;
		GetParent().AddChild(playerDeath);
		QueueFree();
	}

	public void SetScale(Vector2 scale)
	{
		_sprite.Scale = scale;
	}

	public void SetRotation(float rotation)
	{
		_sprite.RotationDegrees = rotation;
	}

	public void SetSpriteFlash(bool value)
	{
		(_sprite.Material as ShaderMaterial).SetShaderParameter("Enabled", value);
	}

	public void SetSpriteAlpha(float value)
	{
		_sprite.Modulate = new Color(1, 1, 1, value);
	}

	public void SetRunParticles(bool value)
	{
		_runParticles.Emitting = value;
	}

	public void EmitLandingParticles()
	{
		_landParticles.EmitParticles();
	}

	public void SetDashParticles(bool value)
	{
		if (value)
		{
			_dashParticles.EmitParticles();
			_dashGhost.EmitGhosts();
		}
		else
		{
			_dashParticles.StopParticles();
			_dashGhost.StopGhosts();
		}
	}

	private void _On_Hitbox_Collision(Area2D area)
	{
		if (area is IDamaging damaging && IFrameTimer.IsStopped())
		{
			Timer.Stop();
			_stateMachine.TransitionTo(PlayerState.Hurt, new Dictionary<string, Variant> { { "value", (damaging).Damage } });
		}
	}
}
