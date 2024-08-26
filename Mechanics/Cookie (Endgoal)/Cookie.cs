using Godot;
using System;

public partial class Cookie : Area2D
{
	[Export]
	private PackedScene _nextLevel;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Player)
		{
			PackedScene winCanvasRef = GD.Load<PackedScene>(@"res://UI/Cookie Get/win_canvas.tscn");
			WinCanvas winCanvas = winCanvasRef.Instantiate<WinCanvas>();
			winCanvas.NextScene = _nextLevel;
			GetParent().AddChild(winCanvas);
		}
	}
}
