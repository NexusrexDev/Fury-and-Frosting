using Godot;
using System;

[Tool]
public partial class CameraLimitSetter : Area2D
{
	[Export]
	private Camera2D _camera;

	private Rect2 _newLimit = new Rect2();
	[Export]
	public Rect2 NewLimit
	{
		get { return _newLimit; }
		set
		{
			_newLimit = value;
			QueueRedraw();
		}
	}

	private Color _toolColor = new Color(0, 0, 0);
	[Export]
	public Color ToolColor
	{
		get { return _toolColor; }
		set
		{
			_toolColor = value;
			QueueRedraw();
		}
	}

	public override void _Ready()
	{
		BodyEntered += cameraChange;
	}

	private void cameraChange(Node2D body)
	{
		if (body != null && body.IsInGroup("Player"))
		{
			(_camera as Camera).SetLimitRect(_newLimit);
		}
	}

	public override void _Draw()
	{
		if (Engine.IsEditorHint())
		{
			Vector2 localPosition = _newLimit.Position - GlobalPosition;
			Rect2 drawnRect = new Rect2(localPosition, _newLimit.Size);
			DrawRect(drawnRect, new Color(_toolColor, 0.2f));
			DrawRect(drawnRect, _toolColor, false);
		}
			
	}
}
