using System.ComponentModel;

namespace CardGame.scripts.card
{
    public enum CardColor
    {
        [Description("red")]
        Red,
        
        [Description("blue")]
        Blue,
        
        [Description("green")]
        Green,
        
        [Description("yellow")]
        Yellow
    }

    public enum CardType
    {
        [Description("2")]
        Two,

        [Description("3")]
        Three,
        
        [Description("4")]
        Four,
        
        [Description("5")]
        Five,
        
        [Description("6")]
        Six,
        
        [Description("7")]
        Seven,
        
        [Description("8")]
        Eight,
        
        [Description("9")]
        Nine,
        
        [Description("skip")]
        Skip,
        
        [Description("picker")]
        AddTwoCards
    }
}