using System;

namespace CardGame.scripts.card
{
    [Serializable]
    public class CardData
    {

        public CardColor Color { get; set; }

        public CardType Type { get; set; }

        public string TexturePath { get; set; }
        
        public string TextureCardShirtPath { get; } = "res://assets/PNGs/large/card_back_large.png";
        
        public string Name { get; set; }
    }
}