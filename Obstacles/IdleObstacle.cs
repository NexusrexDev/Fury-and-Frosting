using Godot;
using System;

public partial class IdleObstacle : Area2D, IDamaging
{
	[Export(PropertyHint.Range, "0, 100")]
	public int Damage { get; set; }
}
