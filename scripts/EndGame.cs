using Godot;

namespace CardGame.scripts
{
    public enum GameStatus
    {
        Win,
        Lose
    }
    
    public abstract class EndGame : Node2D
    {
        public void SetGameStatus(GameStatus status)
        {
            GD.Print("set");
            if (!(GetNode("GameStatus") is Label gameStatusLabel)) return;
            GD.Print("change");
            gameStatusLabel.Text = status == GameStatus.Win ? "You win" : "You lose";
        }
        
        private void _on_Exit_pressed()
        {
            GetTree().Quit();
        }
    }
}