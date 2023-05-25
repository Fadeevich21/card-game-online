using CardGame.scripts.card;
using CardGame.scripts.player;
using Godot;

namespace CardGame.scripts
{
    public abstract class PlaySpace : Node2D
    {
        private readonly string[] _players = { "player1", "player2" };
        private int _currentTurnIndex;
        private string CurrentPlayer => _players[_currentTurnIndex];

        public override void _Ready()
        {
            InitGetCardButton();
            InitPassButton();

            var cardOnField = GD.Load<PackedScene>("res://scenes/Card.tscn").Instance();
            cardOnField.Name = "CardOnField";
            AddChild(cardOnField);
            if (IsNetworkMaster())
            {
                var cardOnFieldPos = GetNode<Node2D>("CardOnFieldPosition").Position;
                var card = CardGenerate.Generate();
                InitFirstCardOnField(card, cardOnFieldPos);
                Rpc(nameof(InitFirstCardOnField), card, cardOnFieldPos);
            }

            var player1Scene = GD.Load<PackedScene>("res://scenes/Player.tscn").Instance();
            player1Scene.Name = "player1";
            player1Scene.SetNetworkMaster(GetTree().GetNetworkUniqueId());
            AddChild(player1Scene);
            if (GetNode("player1") is Player player1)
            {
                player1.GlobalTransform = GetNode<Node2D>("Player1Pos").GlobalTransform;
                player1.RotationDegrees = 180;
                if (IsNetworkMaster())
                    for (var i = 0; i < 5; i++)
                    {
                        var card = CardGenerate.Generate();
                        player1.AddCard(card);
                        player1.Rpc("AddCard", card);
                    }
            }

            var player2Scene = GD.Load<PackedScene>("res://scenes/Player.tscn").Instance();
            player2Scene.Name = "player2";
            player2Scene.SetNetworkMaster(Singleton.UserId);
            AddChild(player2Scene);
            if (GetNode("player2") is Player player2)
            {
                player2.GlobalTransform = GetNode<Node2D>("Player2Pos").GlobalTransform;
                if (IsNetworkMaster())
                    for (var i = 0; i < 5; i++)
                    {
                        var card = CardGenerate.Generate();
                        player2.AddCard(card);
                        player2.Rpc("AddCard", card);
                    }
            }

            PassButtonDisabled();
            NextTurn();
            Rpc(nameof(NextTurn));
        }

        public bool PlayerRemoveCard(Card card)
        {
            if (!OnCardClickedChangeCardOnField(card)) return false;
            OnCardClickedButtonSetup();

            NextTurn();
            Rpc(nameof(NextTurn));

            return true;
        }

        private void InitGetCardButton()
        {
            var getCardButton = GetNode("GetCard");
            getCardButton.Connect("pressed", this, nameof(OnGetCardButtonPressed));
        }

        private void InitPassButton()
        {
            var passButton = GetNode("Pass");
            passButton.Connect("pressed", this, nameof(OnPassButtonPressed));
        }

        [Remote]
        public void InitFirstCardOnField(Card card, Vector2 position)
        {
            if (!(GetNode("CardOnField") is Card cardOnField)) return;
            cardOnField.GlobalPosition = position;
            ChangeCardOnField(card);
        }

        [Remote]
        public void NextTurn()
        {
            var prevPlayer = GetNode(CurrentPlayer) as Player;
            prevPlayer?.SetCardsClicked(false);

            _currentTurnIndex = (_currentTurnIndex + 1) % _players.Length;
            var player = GetNode(CurrentPlayer) as Player;
            player?.SetCardsClicked(true);
        }


        [Remote]
        public void ChangeCardOnField(Card card)
        {
            if (!(GetNode("CardOnField") is Card cardOnField)) return;

            cardOnField.Color = card.Color;
            cardOnField.Type = card.Type;
            cardOnField.Texture = card.Texture;
        }

        private bool OnCardClickedChangeCardOnField(Card card)
        {
            GD.Print("check change card start");
            if (!CheckChangeCardOnField(card)) return false;
            GD.Print("check change card end");

            ChangeCardOnField(card);
            Rpc(nameof(ChangeCardOnField), card);

            return true;
        }

        private void OnCardClickedButtonSetup()
        {
            GetCardButtonEnabled();
            PassButtonDisabled();
        }

        private bool CheckChangeCardOnField(Card card)
        {
            var cardOnField = GetNode("CardOnField") as Card;
            GD.Print(card.Color);
            GD.Print(card.Type);
            GD.Print(cardOnField.Color == card.Color);
            GD.Print(cardOnField.Type == card.Type);
            
            return cardOnField != null && (cardOnField.Color == card.Color || cardOnField.Type == card.Type);
        }

        private void ButtonDisabled(string buttonName)
        {
            if (GetNode(buttonName) is Button button) button.Disabled = true;
        }

        private void ButtonEnabled(string buttonName)
        {
            if (GetNode(buttonName) is Button button) button.Disabled = false;
        }

        private void GetCardButtonDisabled()
        {
            ButtonDisabled("GetCard");
        }

        private void GetCardButtonEnabled()
        {
            ButtonEnabled("GetCard");
        }

        private void PassButtonDisabled()
        {
            ButtonDisabled("Pass");
        }

        private void PassButtonEnabled()
        {
            ButtonEnabled("Pass");
        }

        public void OnGetCardButtonPressed()
        {
            GD.Print("get card");
            if (!(GetNode(CurrentPlayer) is Player player)) return;

            var card = CardGenerate.Generate();
            player.AddCard(card);
            player.Rpc("AddCard", card);
            GetCardButtonDisabled();
            PassButtonEnabled();
        }

        public void OnPassButtonPressed()
        {
            GD.Print("pass");
            PassButtonDisabled();
            GetCardButtonEnabled();
            NextTurn();
            Rpc(nameof(NextTurn));
        }
    }
}