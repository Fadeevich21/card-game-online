using Godot;

namespace CardGame.scripts.card
{
    public class Card : Sprite
    {
        public bool IsClicked = false;
        
        public CardColor Color { get; set; }

        public CardType Type { get; set; }


        [Signal]
        public delegate void Clicked(Card card);

        public Card()
        {
            var card = CardGenerate.Generate();
            Color = card.Color;
            Type = card.Type;
            Name = card.Name;
        }
        
        public Card(CardColor color, CardType type)
        {
            Color = color;
            Type = type;
        }

        public override void _Input(InputEvent @event)
        {
            if (!IsClicked) return;
            
            if (!(@event is InputEventMouseButton mouseButton) || !mouseButton.Pressed ||
                mouseButton.ButtonIndex != (int)ButtonList.Left) return;
            if (!GetRect().HasPoint(ToLocal(mouseButton.Position))) return;

            EmitSignal(nameof(Clicked), this);
        }
    }
}