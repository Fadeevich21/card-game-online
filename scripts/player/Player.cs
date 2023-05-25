using CardGame.scripts.card;
using Godot;

namespace CardGame.scripts.player
{
    public abstract class Player : Node2D
    {
        [Remote]
        public void AddCard(Card card)
        {
            var cardsNode = GetNode("Cards");
            if (!(GD.Load<PackedScene>("res://scenes/Card.tscn").Instance() is Card cardChild)) return;
            cardChild.Connect(nameof(Card.Clicked), this, nameof(OnCardClicked));
            cardChild.IsClicked = true;
            cardChild.Color = card.Color;
            cardChild.Type = card.Type;
            cardChild.Texture = card.Texture;

            cardsNode.AddChild(cardChild);
            ChangePositionCards();
        }

        public void OnCardClicked(Card card)
        {
            GD.Print("OnCardClicked");
            if (!(GetParent() is PlaySpace parent)) return;
            if (!parent.PlayerRemoveCard(card)) return;

            RemoveCard(card.Name);
            Rpc(nameof(RemoveCard), card.Name);

            GetTree().SetInputAsHandled();
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
            GD.Print("remove card 1");
            GD.Print("remove card 2");
            var cards = GetNodeOrNull("Cards");
            GD.Print("remove card 3");
            var card = cards?.GetNodeOrNull(cardName);
            GD.Print("remove card 4");
            if (card == null) return;
            GD.Print("remove card 5");
            cards.RemoveChild(card);

            GD.Print("remove card 6");
            ChangePositionCards();
        }

        private void ChangePositionCards()
        {
            var cards = GetNode("Cards").GetChildren();
            var cardsCount = cards.Count;
            if (cardsCount == 0) return;

            var centerPos = GlobalPosition;
            const float radiusX = 200.0f;
            const float radiusY = 100.0f;
            var angleStep = 180.0f / (cardsCount + 1);

            for (var i = 0; i < cardsCount; i++)
            {
                var angle = angleStep * (i + 1);
                var posX = centerPos.x - radiusX * Mathf.Cos(Mathf.Deg2Rad(angle));
                var posY = centerPos.y;
                if (Mathf.Abs(RotationDegrees) >= 180)
                    posY += radiusY * Mathf.Sin(Mathf.Deg2Rad(angle));
                else
                    posY -= radiusY * Mathf.Sin(Mathf.Deg2Rad(angle));

                ((Card)cards[i]).Position = new Vector2(posX, posY);
            }
        }
    }
}