using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class WitchState : State
{
	protected Witch _witch;
	protected int _phaseStep = 0;
	[Export]
	protected Timer _phaseTimer;

	//State constants
	public static StringName Attack1 = new StringName("Attack1");
	public static StringName Attack2 = new StringName("Attack2");
	public static StringName Attack3 = new StringName("Attack3");
	public static StringName Hurt = new StringName("Hurt");


	public override async void _Ready()
	{
		await ToSignal(Owner, Node.SignalName.Ready);
		_witch = Owner as Witch;
		Debug.Assert(_witch != null, "The owner must be a Witch node.");
	}

	public override void Enter(Dictionary<string, Variant> _message = null)
	{
		if (_phaseTimer != null)
			_phaseTimer.Timeout += PhaseHandle;
	}

	public virtual void PhaseHandle() { }

	public override void Exit()
	{
		if (_phaseTimer == null)
			return;

		_phaseStep = 0;
		_phaseTimer.Stop();
		_phaseTimer.Timeout -= PhaseHandle;
	}
}
