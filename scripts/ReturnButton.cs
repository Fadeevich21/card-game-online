using Godot;
using System;

public class ReturnButton : Button
{
    public override void _Ready()
    {
        this.Connect("pressed", this, "OnPressed");
    }

    private void OnPressed()
    {
        GD.Print("Settings");
        
        var streamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        streamPlayer.Stream = GD.Load<AudioStream>("res://musics/click_button.wav");
        streamPlayer.Play();

        GetTree().ChangeScene("res://scenes/Control.tscn");
    }

    public override void _Process(float delta)
    {
    }
}
