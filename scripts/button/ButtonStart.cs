using Godot;

namespace CardGame.scripts.button
{
    public class ButtonStart : Button
    {
        public override void _Ready()
        {
            Connect("pressed", this, "OnPressed");
        }

        private void OnPressed()
        {
            GD.Print("Start");
            GetTree().ChangeScene("res://scenes/GameMenu.tscn");
        }
    }
}
