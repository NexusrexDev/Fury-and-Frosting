using Godot;
using System;

[Tool]
public partial class LevelGrid : Node2D
{
	public override void _Draw()
	{
		if (Engine.IsEditorHint())
		{
			Color gridColor = new(0.2f, 0, 0);
			for (int i = 0; i < 10; i++)
				for (int j = -1; j < 2; j++)
					DrawRect(new Rect2(i * 320, j * 180, 320, 180), gridColor, false);
		}

	}
}
