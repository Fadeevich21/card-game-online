using System;
using System.ComponentModel;

namespace CardGame.scripts.card
{
    public enum CardColor
    {
        [Description("red")] Red = 0,

        [Description("blue")] Blue = 1,

        [Description("green")] Green = 2,

        [Description("yellow")] Yellow = 3
    }

    public enum CardType
    {
        [Description("2")] Two = 0,

        [Description("3")] Three = 1,

        [Description("4")] Four = 2,

        [Description("5")] Five = 3,

        [Description("6")] Six = 4,

        [Description("7")] Seven = 5,

        [Description("8")] Eight = 6,

        [Description("9")] Nine = 7,

        [Description("skip")] Skip = 8,

        [Description("picker")] AddCard = 9
    }

    public abstract class CardsDatabase
    {
        public static string GetDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute =
                (DescriptionAttribute)fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
            return descriptionAttribute?.Description ?? value.ToString();
        }
    }
}