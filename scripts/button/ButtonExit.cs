using Godot;

namespace CardGame.scripts.button
{
    public class ButtonExit : Button
    {
        public override void _Ready()
        {
            Connect("pressed", this, "OnPressed");
        }

        private void OnPressed()
        {
            GD.Print("Exit");
            GetTree().Quit();
        }

        public override void _Process(float delta)
        {
        }
    }
}