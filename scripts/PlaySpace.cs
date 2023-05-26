using System;
using CardGame.scripts.card;
using CardGame.scripts.player;
using Godot;

namespace CardGame.scripts
{
    public abstract class PlaySpace : Node2D
    {
        private const int CardsGenerate = 5;
        
        private string _player = "";

        private const string Player1 = "player1";
        private const string Player2 = "player2";
        private readonly string[] _players = { Player1, Player2 };
        private int _currentTurnIndex;
        private string CurrentPlayer => _players[_currentTurnIndex];

        public override void _Ready()
        {
            InitCardOnField();

            if (!IsNetworkMaster()) return;
            var playerIndex = new Random().Next(0, 2);
            SetPlayer(_players[playerIndex]);
            Rpc(nameof(SetPlayer), _players[1 - playerIndex]);

            CreatePlayer1();
            Rpc(nameof(CreatePlayer1));
            CreatePlayer2();
            Rpc(nameof(CreatePlayer2));

            SetPositionPlayer1();
            Rpc(nameof(SetPositionPlayer1));
            SetPositionPlayer2();
            Rpc(nameof(SetPositionPlayer2));

            AddCardsPlayer1();
            AddCardsPlayer2();

            NextTurn();
            Rpc(nameof(NextTurn));
        }

        [Remote]
        public void SetPlayer(string player)
        {
            _player = player;
        }

        private void SetLabelCurrentPlayer()
        {
            if (GetNode("CurrentPlayer") is Label labelCurrentPlayer)
                labelCurrentPlayer.Text = _player == CurrentPlayer ? "Your move" : "Opponent's move";
        }

        private void InitCardOnField()
        {
            GD.Print("init card on field");
            var cardOnField = Card.Create();
            if (cardOnField == null) return;
            cardOnField.Name = "CardOnField";
            AddChild(cardOnField);

            if (!IsNetworkMaster()) return;
            var card = CardGenerate.Generate();
            if (card == null) return;
            var position = GetNode<Node2D>("CardOnFieldPosition").Position;
            SetupCardOnField(card.Serialize(), position);
            Rpc(nameof(SetupCardOnField), card.Serialize(), position);
        }

        [Remote]
        public void CreatePlayer1()
        {
            var playerScene = Player.Create();
            playerScene.Name = Player1;
            playerScene.SetNetworkMaster(GetTree().GetNetworkUniqueId());
            AddChild(playerScene);
        }

        [Remote]
        public void SetPositionPlayer1()
        {
            if (!(GetNode(Player1) is Player player)) return;
            if (_player == Player1)
                SetPositionMyPlayer(player);
            else
                SetPositionOpponentPlayer(player);
        }

        private void AddCardsPlayer1()
        {
            if (!(GetNode(Player1) is Player player)) return;
            if (!IsNetworkMaster()) return;
            SetupPlayer(player);
        }

        [Remote]
        public void CreatePlayer2()
        {
            var playerScene = Player.Create();
            playerScene.Name = Player2;
            playerScene.SetNetworkMaster(Singleton.UserId);
            AddChild(playerScene);
        }

        [Remote]
        public void SetPositionPlayer2()
        {
            if (!(GetNode(Player2) is Player player)) return;
            if (_player == Player2)
                SetPositionMyPlayer(player);
            else
                SetPositionOpponentPlayer(player);
        }

        private void AddCardsPlayer2()
        {
            if (!(GetNode(Player2) is Player player)) return;
            if (!IsNetworkMaster()) return;
            SetupPlayer(player);
        }

        private void SetPositionMyPlayer(Player player)
        {
            player.GlobalTransform = GetNode<Node2D>("MyPlayerPos").GlobalTransform;
        }

        private void SetPositionOpponentPlayer(Player player)
        {
            player.GlobalTransform = GetNode<Node2D>("OpponentPlayerPos").GlobalTransform;
            player.RotationDegrees = 180;
        }

        private void SetupPlayer(Player player)
        {
            GD.Print("setup player");
            for (var i = 0; i < CardsGenerate; i++)
            {
                var card = CardGenerate.Generate();
                AddCard(player.Name, card.Serialize());
                Rpc(nameof(AddCard), player.Name, card.Serialize());
                // player.AddCard(card.Serialize());
                // player.Rpc("AddCard", card.Serialize());
            }
        }

        [Remote]
        public  void AddCard(string playerName, byte[] cardDataSerialize)
        {
            var cardData = Card.Deserialize(cardDataSerialize);
            if (_player != playerName)
                cardData.TexturePath = cardData.TextureCardShirtPath;
            if (!(GetNode(playerName) is Player player)) return;
            player.AddCard(cardData);
        }
        
        public void OnCardClicked(Card card)
        {
            if (_player != CurrentPlayer) return;

            GD.Print("on card clicked");
            if (!OnCardClickedChangeCardOnField(card)) return;
            OnCardClickedButtonSetup();

            if (!(GetNode(CurrentPlayer) is Player player)) return;
            player.RemoveCard(card.Name);
            player.Rpc("RemoveCard", card.Name);

            if (CheckEndGame())
            {
                EndGame(GameStatus.Win);
                Rpc(nameof(EndGame), GameStatus.Lose);
            }

            switch (card.Type)
            {
                case CardType.Skip:
                    NextTurn();
                    Rpc(nameof(NextTurn));
                    break;
                case CardType.AddCard:
                {
                    var nextPlayerIndex = (_currentTurnIndex + 1) % _players.Length;
                    var nextPlayerName = _players[nextPlayerIndex];

                    if (!(GetNode(nextPlayerName) is Player nextPlayer)) return;
                    var cardAdded = CardGenerate.Generate();
                    AddCard(nextPlayer.Name, cardAdded.Serialize());
                    Rpc(nameof(AddCard), nextPlayer.Name, cardAdded.Serialize());
                    break;
                }
            }

            NextTurn();
            Rpc(nameof(NextTurn));
        }

        private bool CheckEndGame()
        {
            if (!(GetNode(CurrentPlayer) is Player player)) return false;

            return player.GetNode("Cards").GetChildren().Count == 0;
        }
        
        [Remote]
        public void EndGame(GameStatus status)
        {
            if (!(GD.Load<PackedScene>("res://scenes/EndGame.tscn").Instance() is EndGame endGame)) return;
            endGame.SetGameStatus(status);
            GetTree().GetRoot().AddChild(endGame);

            QueueFree();
        }

        [Remote]
        public void SetupCardOnField(byte[] cardSerialized, Vector2 position)
        {
            GD.Print("setup card on field");

            var cardData = Card.Deserialize(cardSerialized);
            if (cardData == null) return;

            if (!(GetNode("CardOnField") is Card cardOnField)) return;
            cardOnField.GlobalPosition = position;
            cardOnField.SetParamsCardWithoutName(cardData);
        }

        [Remote]
        public void NextTurn()
        {
            if (!(GetNode(CurrentPlayer) is Player prevPlayer)) return;
            prevPlayer.SetCardsClicked(false);
            if (_player == CurrentPlayer)
            {
                GetCardButtonDisabled();
                PassButtonDisabled();
            }

            _currentTurnIndex = (_currentTurnIndex + 1) % _players.Length;
            if (!(GetNode(CurrentPlayer) is Player player)) return;
            player.SetCardsClicked(true);
            if (_player == CurrentPlayer)
            {
                GetCardButtonEnabled();
                PassButtonDisabled();
            }

            SetLabelCurrentPlayer();
        }

        private bool OnCardClickedChangeCardOnField(Card card)
        {
            GD.Print("on card clicked change card on field");
            if (!CheckChangeCardOnField(card)) return false;

            ChangeCardOnField(card.Serialize());
            Rpc(nameof(ChangeCardOnField), card.Serialize());

            return true;
        }

        private void OnCardClickedButtonSetup()
        {
            GetCardButtonEnabled();
            PassButtonDisabled();
        }

        private bool CheckChangeCardOnField(Card card)
        {
            GD.Print("check change card on field");

            var cardOnField = GetNode("CardOnField") as Card;

            return cardOnField != null && (cardOnField.Color == card.Color || cardOnField.Type == card.Type);
        }

        [Remote]
        public void ChangeCardOnField(byte[] cardDataSerialized)
        {
            var cardData = Card.Deserialize(cardDataSerialized);
            GD.Print("change card on field");
            if (!(GetNode("CardOnField") is Card cardOnField)) return;

            cardOnField.Color = cardData.Color;
            cardOnField.Type = cardData.Type;
            cardOnField.SetTexture_(cardData.TexturePath);
        }

        private void SetButtonDisabled(string buttonName, bool isDisabled)
        {
            if (GetNode(buttonName) is Button button) button.Disabled = isDisabled;
        }

        private void GetCardButtonDisabled()
        {
            SetButtonDisabled("GetCard", true);
        }

        private void GetCardButtonEnabled()
        {
            SetButtonDisabled("GetCard", false);
        }

        private void PassButtonDisabled()
        {
            SetButtonDisabled("Pass", true);
        }

        private void PassButtonEnabled()
        {
            SetButtonDisabled("Pass", false);
        }

        private void _on_GetCard_pressed()
        {
            if (!(GetNode(_player) is Player player)) return;

            var card = CardGenerate.Generate();
            AddCard(player.Name, card.Serialize());
            Rpc(nameof(AddCard), player.Name, card.Serialize());
            GetCardButtonDisabled();
            PassButtonEnabled();
        }

        private void _on_Pass_pressed()
        {
            NextTurn();
            Rpc(nameof(NextTurn));
        }
    }
}