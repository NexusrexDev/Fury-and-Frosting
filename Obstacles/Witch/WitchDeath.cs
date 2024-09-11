using Godot;
using System;

public partial class WitchDeath : Sprite2D
{
	[Export]
	private PackedScene _explosionScene, _cookieDescentScene;

	[Export]
	private AudioStream _explosionSFX, _bigExplosionSFX;

	private bool _bigExplosion = false;
	private float _explosionRadius = 0f;

	private Tween _tween;

	public Player PlayerReference;

	public override void _Ready()
	{
		MiniExplosions();
	}

	private async void MiniExplosions()
	{
		for (int i = 0; i < 10; i++)
		{
			AudioManager.Instance.PlaySFX(_explosionSFX);
			ParticleEmitter explosion = _explosionScene.Instantiate<ParticleEmitter>();
			RandomNumberGenerator numberGenerator = new RandomNumberGenerator();
			int pos = numberGenerator.RandiRange(-8, 8); 
			explosion.Position = Position + new Vector2(pos, pos);
			AddSibling(explosion);
			Scale = new Vector2(1.25f, 1.25f);
			await ToSignal(GetTree().CreateTimer(0.5f / (i+1)), "timeout");
		}

		BigExplosion();
	}

	private void BigExplosion()
	{
		_bigExplosion = true;
		AudioManager.Instance.PlaySFX(_bigExplosionSFX);

		_tween = CreateTween();
		_tween.TweenProperty(this, "_explosionRadius", 320, 1f);
		_tween.TweenCallback(Callable.From(FadeOut));
	}

	private void FadeOut()
	{
		Texture = null;
		_tween = CreateTween();
		_tween.TweenProperty(this, "modulate:a", 0, 1f);
		_tween.TweenCallback(Callable.From(End));
	}

	private void End()
	{
		PlayerReference.IsRaging = false;
		PlayerReference.Rage = 0;
		Node2D cookieDescent = _cookieDescentScene.Instantiate<Node2D>();
		CallDeferred(MethodName.AddSibling, cookieDescent);
		AudioManager.Instance.PlayMusic(MusicTitles.CookieDescent, 0.2f);
		GetTree().Paused = false;
		QueueFree();
	}

	public override void _Process(double delta)
	{
		if (_bigExplosion)
			QueueRedraw();
		else
			Scale = new Vector2(Mathf.MoveToward(Scale.X, 1, (float)delta), Mathf.MoveToward(Scale.Y, 1, (float)delta));
	}

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, _explosionRadius, Colors.White);
	}
}
