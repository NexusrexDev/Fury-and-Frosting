using Godot;
using System;

public partial class ReadyCanvas : CanvasLayer
{
	[Export]
	private Player _player;

	public void OnAnimationEnd()
	{
		if (_player != null)
			_player.IsControllable = true;

		QueueFree();
	}
}
