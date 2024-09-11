using Godot;
using System;

public partial class MagicWallCreator : Marker2D
{
	[Export]
	private PackedScene _projectileReference;

	[Export]
	private int _wallHeight = 4;

	[Export]
	private AudioStream _shootSFX;

	[Export(PropertyHint.Range, "0,360,radians_as_degrees")]
	private float direction = 0;

	public void CreateWall()
	{
		for (int i = 0; i < _wallHeight; i++)
		{
			WitchProjectile projectile = _projectileReference.Instantiate<WitchProjectile>();
			projectile.Position = new Vector2(Position.X, Position.Y - (i * 16));
			projectile.Direction = direction;
			CallDeferred(MethodName.AddSibling, projectile);
		}

		AudioManager.Instance.PlaySFX(_shootSFX);
	}
}
