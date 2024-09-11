using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public enum ColliderState
{
	Invincible,
	Vulnerable
}

public partial class Witch : CharacterBody2D
{
	public const float MaxFallSpeed = 500.0f;
	private const sbyte _healthPerPhase = 3;
	public const int MaxHealth = _healthPerPhase * 3;

	[Signal]
	public delegate void HealthChangeEventHandler(int newHealth);

	[Signal]
	public delegate void DeathEventHandler();

	[Signal]
	public delegate void PhaseChangeEventHandler(int phaseNumber);

	[Signal]
	public delegate void ThrowActivatorEventHandler(int position);

	[Signal]
	public delegate void WallActivatorEventHandler(int lane);

	private Array<StringName> phaseNames = new Array<StringName> { WitchState.Attack1, WitchState.Attack2, WitchState.Attack3 };

	public StringName BossPhase 
	{ 
		get 
		{
			int currentPhase = (MaxHealth - _health) / _healthPerPhase;
			return phaseNames[currentPhase];
		} 
	}

	private int _health = MaxHealth;
	public int Health 
	{
		get { return _health; }
		set 
		{ 
			_health = value;
			EmitSignal(SignalName.HealthChange, _health);
			if (_health <= 0)
				Die();
		}
	}

	private sbyte direction = -1;
	public sbyte Direction
	{
		get { return direction; }
		set
		{
			direction = value;
			_sprite.FlipH = direction == 1;
		}
	}

	private StateMachine _stateMachine;

	private Sprite2D _sprite;

	private Vector2 _targetScale = Vector2.One;

	private ParticleEmitter _landParticles;

	private Area2D _hitBox, _hurtBox;

	public RandomNumberGenerator NumberGenerator = new RandomNumberGenerator();

	public Timer IFrameTimer;

	[Export]
	private PackedScene _projectileReference;

	[Export]
	private Marker2D _positionMarker;

	[Export]
	public Player PlayerReference;

	[Export]
	public PackedScene FadeInReference, FadeOutReference;

	[Export]
	private AudioStream _shootSFX;

	[Export]
	private PackedScene _deathAnimationReference;

	public override void _Ready()
	{
		_stateMachine = GetNode<StateMachine>("StateMachine");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		SetSpriteFlash(false);

		_hitBox = GetNode<Area2D>("Hitbox");
		_hitBox.AreaEntered += _On_Hitbox_Collision;
		_hurtBox = GetNode<Area2D>("Hurtbox");

		IFrameTimer = GetNode<Timer>("IFrameTimer");

		_landParticles = GetNode<ParticleEmitter>("LandParticles");
	}

	public override void _Process(double delta)
	{
		_sprite.Scale = new Vector2(Mathf.MoveToward(_sprite.Scale.X, _targetScale.X, (float)delta), Mathf.MoveToward(_sprite.Scale.Y, _targetScale.Y, (float)delta));
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public void SetScale(Vector2 scale)
	{
		_sprite.Scale = scale;
	}

	public void SetTargetScale(Vector2 scale)
	{
		_targetScale = scale;
	}

	public void SetRotation(float rotation)
	{
		_sprite.RotationDegrees = rotation;
	}

	public void SetSpriteFrame(int frame)
	{
		_sprite.Frame = frame;
	}

	public void SetSpriteFlash(bool value)
	{
		(_sprite.Material as ShaderMaterial).SetShaderParameter("Enabled", value);
	}

	public void EmitLandingParticles()
	{
		_landParticles.EmitParticles();
	}

	public void CreateProjectile(bool aimed)
	{
		WitchProjectile witchProjectile = _projectileReference.Instantiate<WitchProjectile>();
		witchProjectile.Position = _positionMarker.GlobalPosition;
		float direction;
		switch (aimed)
		{
			case true:
				direction = GetAngleTo(PlayerReference.Position);
				break;
			case false:
				direction = Direction == 1 ? 0 : Mathf.DegToRad(180);
				break;
		}
		SetTargetScale(Vector2.One);
		SetScale(Vector2.One * 1.25f);
		SetSpriteFrame(1);
		SetRotation(Direction * 4);
		AudioManager.Instance.PlaySFX(_shootSFX);
		witchProjectile.Direction = direction;
		AddSibling(witchProjectile);
	}

	public void SwitchCollisions(ColliderState colliderState)
	{
		switch (colliderState)
		{
			case ColliderState.Vulnerable:
				_hurtBox.Monitorable = false;
				_hitBox.Monitoring = true;
				break;
			case ColliderState.Invincible:
				_hurtBox.Monitorable = true;
				_hitBox.Monitoring = false;
				break;
		}
	}

	private void _On_Hitbox_Collision(Area2D area)
	{
		if (area is PlayerSword && IFrameTimer.IsStopped())
		{
			_stateMachine.TransitionTo(WitchState.Hurt, new Dictionary<string, Variant> { { "value", 1 } });
		}
	}

	private void Die()
	{
		EmitSignal(SignalName.Death);
		WitchDeath witchDeath = _deathAnimationReference.Instantiate<WitchDeath>();
		witchDeath.PlayerReference = PlayerReference;
		witchDeath.Position = Position;
		witchDeath.FlipH = Direction == 1;
		CallDeferred(MethodName.AddSibling, witchDeath);
		QueueFree();
		GetTree().Paused = true;
		AudioManager.Instance.StopAllSFX();
		AudioManager.Instance.StopMusic();
		AudioManager.Instance.SetMusicHiPass(false);
	}
}
