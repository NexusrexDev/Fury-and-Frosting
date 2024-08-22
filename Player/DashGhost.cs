using Godot;
using System;

public partial class DashGhost : Node2D
{
	private Node2D _level, _player;
	private Sprite2D _reference;
	private Timer _timer;

	public override void _Ready()
	{
		_player = GetOwner<Player>();
		_level = _player.GetParent<Node2D>();
		_timer = GetNode<Timer>("Timer");

		_timer.Timeout += OnTimerTimeout;
	}

	public void Init(Sprite2D reference)
	{
		_reference = reference;
	}

	public void EmitGhosts()
	{
		_timer.Start();
	}

	private void OnTimerTimeout()
	{
		FadeOutSprite ghost = new FadeOutSprite();
		ghost.Texture = _reference.Texture;
		ghost.Scale = _reference.Scale;
		ghost.Rotation = _reference.Rotation;
		ghost.FlipH = _reference.FlipH;
		ghost.Position = _player.Position;
		ghost.Modulate = new Color(1f, 1f, 1f, 0.5f);
		ghost.ZIndex = -1;
		_level.AddChild(ghost);
	}

	public void StopGhosts()
	{
		_timer.Stop();
	}
}
