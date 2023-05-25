using Godot;
using System;
using System.ComponentModel;

namespace CardGame.scripts.card
{
    public abstract class CardGenerate
    {
        public static Card Generate()
        {
            var color = GetRandomCardColor();
            var type = GetRandomCardType();
            var card = new Card(color, type);
            card.Texture = GD.Load<Texture>("res://assets/PNGs/large/" + GetDescription(color) + "_" + GetDescription(type) + "_large.png");
            card.Scale = new Vector2(0.15f, 0.15f);
            card.Name = "card_" + new Random(OS.GetTicksMsec()).Next(); 
            
            return card;
        }

        private static CardColor GetRandomCardColor()
        {
            return RandomCardElement<CardColor>.GetRandomElement();
        }

        private static CardType GetRandomCardType()
        {
            return RandomCardElement<CardType>.GetRandomElement();
        }

        private static string GetDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute)fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
            return descriptionAttribute?.Description ?? value.ToString();
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