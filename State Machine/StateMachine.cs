using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class StateMachine : Node
{
	[Signal]
	public delegate void StateChangedEventHandler(string newState);

	[Export]
	public State InitialState { set; get; }

	public State CurrentState { set; get; }

	public override async void _Ready()
	{
		Debug.Assert(InitialState != null, "The InitialState must be set.");

		CurrentState = InitialState;
		
		foreach (State stateChildren in GetChildren())
		{
			stateChildren.StateMachine = this;
		}

		await ToSignal(Owner, Node.SignalName.Ready);

		CurrentState.Enter();
	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentState.PhysicsProcess(delta);
	}

	public override void _Process(double delta)
	{
		CurrentState.Update(delta);
	}

	public override void _Input(InputEvent @event)
	{
		CurrentState.HandleInput(@event);
	}

	public void TransitionTo(string newState, Dictionary<string, Variant> message = null)
	{
		State nextState = GetNode(newState) as State;
		if (nextState == null)
		{
			GD.PrintErr($"The state '{newState}' does not exist.");
			return;
		}

		CurrentState.Exit();
		CurrentState = nextState;
		CurrentState.Enter(message);
		EmitSignal(SignalName.StateChanged, newState);
	}
}
