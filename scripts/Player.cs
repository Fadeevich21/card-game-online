using Godot;
using System;
using System.Collections.Generic;

public class Player : Sprite
{
    private Health _health;
    private Power _power;
    private ProgressBar _hpBar;
    private readonly List<Card> _cards = new List<Card>();

    public override void _Ready()
    {
        _health = new Health(100);
        _power = new Power(0, 10);

        _hpBar = GetNode<ProgressBar>("ProgressBar");
        _hpBar.SetMax(_health.MaxValue);
        _hpBar.SetMin(0);
        _hpBar.SetValue(_health.Value);
    }

    public override void _Process(float delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventMouseButton mouseButton) ||
            !mouseButton.Pressed || mouseButton.ButtonIndex != (int)ButtonList.Left) return;

        GD.Print("player click");
        
        var packedScene = ResourceLoader.Load<PackedScene>("res://scenes/Card.tscn");
        var card = (Card)packedScene.Instance();
        card.Position = mouseButton.Position;
        card.Player = this;
        _cards.Add(card);
        AddChild(card);

        // var damage = _power.GetDamage();
        // _health.Value -= damage.Value;
        // _hpBar.SetValue(_health.Value);
    }

    public void TakeDamage(Damage damage)
    {
        _health.Value -= damage.Value;
        _hpBar.SetValue(_health.Value);
    }
}