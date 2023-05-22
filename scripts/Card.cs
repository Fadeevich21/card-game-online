using Godot;
using System;
using Object = Godot.Object;

public class Card : Sprite
{
    private Power _power;
    public Player Player;

    public override void _Ready()
    {
        _power = new Power(0, 10);
    }

    public override void _Process(float delta)
    {
    }

    public override void _Input(InputEvent @event)
    {

        if (!(@event is InputEventMouseButton mouseButton) ||
            !mouseButton.Pressed || mouseButton.ButtonIndex != (int)ButtonList.Left) return;
        if (!GetRect().HasPoint(ToLocal(mouseButton.Position))) return;

        GD.Print("card click");

        Execute();
        QueueFree();
        GetTree().SetInputAsHandled();
    }

    private void Execute()
    {
        Player.TakeDamage(_power.GetDamage());
    }
}