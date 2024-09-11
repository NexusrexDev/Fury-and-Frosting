using Godot;
using Godot.Collections;
using System;

public partial class ParticleEmitter : Node2D
{
	[Export]
	private bool _emitOnReady = false;

	[ExportGroup("Destroy Settings")]
	[Export]
	private bool _destroyOnEmit = false;
	[Export]
	private float _destroyTime;

	private Array<CpuParticles2D> cpuParticles2Ds = new Array<CpuParticles2D>();

	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is CpuParticles2D cpuParticles2D)
				cpuParticles2Ds.Add(cpuParticles2D);
		}

		if (_emitOnReady)
			EmitParticles();
	}

	public void EmitParticles()
	{
		foreach (CpuParticles2D cpuParticles2D in cpuParticles2Ds)
			cpuParticles2D.Emitting = true;

		if (_destroyOnEmit)
			GetTree().CreateTimer(_destroyTime).Timeout += () => QueueFree();
	}

	public void StopParticles()
	{
		foreach (CpuParticles2D cpuParticles2D in cpuParticles2Ds)
			cpuParticles2D.Emitting = false;
	}
}
