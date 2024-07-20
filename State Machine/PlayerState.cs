using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class PlayerState : State
{
    protected Player _player;

    public override async void _Ready()
    {
        await ToSignal(Owner, "ready");
        _player = Owner as Player;
        Debug.Assert(_player != null, "The owner must be a Player node.");
    }
}
