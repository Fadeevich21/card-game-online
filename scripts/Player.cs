using CardGame.scripts.card;
using Godot;

namespace CardGame.scripts.player
{
    public abstract class Player : Node2D
    {
        public static Player Create()
        {
            return GD.Load<PackedScene>("res://scenes/Player.tscn").Instance() as Player;
        }
        
        public void AddCard(CardData cardData)
        {
            GD.Print("add card");
            var cards = GetNode("Cards");
            var cardChild = Card.Create();
            if (cardChild == null) return;

            cardChild.SetParamsCard(cardData);
            cardChild.Connect(nameof(Card.Clicked), this, nameof(OnCardClicked));
            cardChild.IsClicked = true;

            cards.AddChild(cardChild);
            ChangePositionCards();
        }

        public void OnCardClicked(Card card)
        {
            GD.Print("on card clicked player");
            var parent = GetParent() as PlaySpace;
            parent?.OnCardClicked(card);
        }

        public void SetCardsClicked(bool flag)
        {
            var cards = GetNode("Cards").GetChildren();
            foreach (var card in cards)
                ((Card)card).IsClicked = flag;
        }

        [Remote]
        public void RemoveCard(string cardName)
        {
            GD.Print("remove card");
            var cards = GetNodeOrNull("Cards");
            var card = cards?.GetNodeOrNull(cardName);
            if (card == null) return;

            cards.RemoveChild(card);
            ChangePositionCards();
        }

        private void ChangePositionCards()
        {
            var cards = GetNode("Cards").GetChildren();
            var cardsCount = cards.Count;

            var angleStep = 180.0f / (cardsCount + 1);
            var centerPos = GlobalPosition;
            const float radiusX = 200.0f;
            const float radiusY = 100.0f;

            for (var i = 0; i < cardsCount; i++)
            {
                var angle = angleStep * (i + 1);
                var posX = centerPos.x - radiusX * Mathf.Cos(Mathf.Deg2Rad(angle));
                var posY = centerPos.y;
                var coefficient = 1;
                if (Mathf.Abs(RotationDegrees) < 180)
                    coefficient = -1;

                posY += coefficient * radiusY * Mathf.Sin(Mathf.Deg2Rad(angle));
                ((Card)cards[i]).Position = new Vector2(posX, posY);
            }
        }
    }
}