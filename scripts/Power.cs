using System;

public class Power
    {
        private int MinValue { get; set; }
        private int MaxValue { get; set; }

        public Power(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        private float GetRandomNumber(double minimum, double maximum)
        { 
            Random random = new Random();
            return (float)(random.NextDouble() * (maximum - minimum) + minimum);
        }
        
        public Damage GetDamage()
        {
            return new Damage(GetRandomNumber(MinValue, MaxValue));
        }
    }