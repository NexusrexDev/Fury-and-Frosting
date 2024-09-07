using Godot;
using System;

public partial class WitchProjectile : Area2D, IDamaging
{
	[Export(PropertyHint.Range, "0,100")]
	public int Damage { get; set; }

	[Export(PropertyHint.Range, "0,360,radians_as_degrees")]
	public float Direction;

	[Export(PropertyHint.Range, "0,600")]
	private float _speed;

	private Vector2 _motionVector;

	public override void _Ready()
	{
		_motionVector = Vector2.FromAngle(Direction);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 motion = _motionVector * _speed * (float)delta;
		Position += motion;
	}
}
