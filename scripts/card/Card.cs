using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Godot;

namespace CardGame.scripts.card
{
    [Serializable]
    public class Card : Sprite
    {
        private CardData _data = new CardData();
        
        public bool IsClicked = false;

        public CardColor Color
        {
            get => _data.Color;
            set => _data.Color = value;
        }

        public CardType Type
        {
            get => _data.Type;
            set => _data.Type = value;
        }

        [Signal]
        public delegate void Clicked(Card card);

        public void SetTexture_(string texturePath)
        {
            Texture = GetTexture(texturePath);
            _data.TexturePath = texturePath;
        }

        public void SetTextureShirt()
        {
            Texture = GetTexture(_data.TextureCardShirtPath);
        }
        
        public void SetName_(string name)
        {
            Name = name;
            _data.Name = name;
        }
        
        public void SetParamsCard(CardColor color, CardType type, string texturePath, string name)
        {
            Color = color;
            Type = type;
            SetName_(name);
            SetTexture_(texturePath);
        }

        public void SetParamsCardWithoutName(CardData cardData)
        {
            Color = cardData.Color;
            Type = cardData.Type;
            SetTexture_(cardData.TexturePath);
        }
        
        public void SetParamsCard(CardData cardData)
        {
            SetParamsCardWithoutName(cardData);
            SetName_(cardData.Name);
        }

        public static Texture GetTexture(string texturePath)
        {
            return GD.Load<Texture>(texturePath);
        }
        
        public override void _Input(InputEvent @event)
        {
            if (!IsClicked) return;

            if (!(@event is InputEventMouseButton mouseButton) || !mouseButton.Pressed ||
                mouseButton.ButtonIndex != (int)ButtonList.Left) return;
            if (!GetRect().HasPoint(ToLocal(mouseButton.Position))) return;

            EmitSignal(nameof(Clicked), this);
        }

        public static Card Create()
        {
            return GD.Load<PackedScene>("res://scenes/Card.tscn").Instance() as Card;
        }

        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, _data);
                return ms.ToArray();
            }
        }

        public static CardData Deserialize(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(ms) as CardData;
            }
        }
    }
}