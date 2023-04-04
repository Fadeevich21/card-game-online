using Godot;
using System;

public class HSlider : Godot.HSlider
{
    private readonly int _busIndex = AudioServer.GetBusIndex("SFX");
    
    public override void _Ready()
    {
        float value = AudioServer.GetBusVolumeDb(_busIndex);
        this.SetValue(value);
   
        this.Connect("value_changed", this,"OnChanged");
    }

    private void OnChanged(float value)
    {
        AudioServer.SetBusVolumeDb(_busIndex, value);
    }
    
    public override void _Process(float delta)
    {
    }
}