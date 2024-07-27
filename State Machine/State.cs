using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class State : Node
{
	public StateMachine StateMachine { get; set; }

	public virtual void Enter(Dictionary<string, Variant> _message = null) { }

	public virtual void Exit() { }

	public virtual void PhysicsProcess(double delta) { }

	public virtual void Update(double delta) { }

	public virtual void HandleInput(InputEvent @event) { }
}
