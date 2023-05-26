using Godot;
using System;

namespace CardGame.scripts.card
{
    public abstract class CardGenerate
    {
        public static Card Generate()
        {
            GD.Print("generate card");
            var color = GetRandomCardColor();
            var type = GetRandomCardType();
            var texturePath = "res://assets/PNGs/large/"
                                          + CardsDatabase.GetDescription(color) + "_"
                                          + CardsDatabase.GetDescription(type) + "_large.png";
            var name = GetRandomCardName();

            var card = new Card();
            card.SetParamsCard(color, type, texturePath, name);
            card.Scale = new Vector2(0.15f, 0.15f);

            return card;
        }

        private static string GetRandomCardName()
        {
            return "card_" + new Random(OS.GetTicksMsec()).Next();
        }

        private static CardColor GetRandomCardColor()
        {
            return RandomCardElement<CardColor>.GetRandomElement();
        }

        private static CardType GetRandomCardType()
        {
            return RandomCardElement<CardType>.GetRandomElement();
        }

        private static class RandomCardElement<T>
        {
            public static T GetRandomElement()
            {
                var rand = new Random();
                var values = Enum.GetValues(typeof(T));
                var element = (T)values.GetValue(rand.Next(values.Length));

                return element;
            }
        }
    }
}