using Godot;
using System;

public class ButtonExit : Button
{
    public override void _Ready()
    {
        this.Connect("pressed", this, "OnPressed");
    }

    private void OnPressed()
    {
        GD.Print("Exit");
        
        var streamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        streamPlayer.Stream = GD.Load<AudioStream>("res://musics/click_button.wav");
        streamPlayer.Play();
        
        GetTree().Quit();
    }

    public override void _Process(float delta)
    {
    }
}