namespace CardGame.scripts
{
    public class Control : Godot.Control
    {
        private void _on_ButtonStart_pressed()
        {
            GetTree().ChangeScene("res://scenes/GameMenu.tscn");
        }

        private void _on_ButtonExit_pressed()
        {
            GetTree().Quit();
        }
    }
}