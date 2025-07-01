using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Coreblock
{
    public class Particle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }
        public float Scale { get; set; }
        public float ScaleDelta { get; set; }
        public float Lifetime { get; set; }
        public float Age { get; private set; }
        public Color Color { get; set; }
        public Color EndColor { get; set; }
        public Texture2D Texture { get; set; }
        public float FadeInTime { get; set; }
        public float FadeOutTime { get; set; }
        public bool EmitsLight { get; private set; }
        public Color LightColor { get; private set; }
        public float LightIntensity { get; private set; }
        public float LightIntensityDelta { get; private set; }
        public float LightRadius { get; private set; }
        public string LightUID { get; } = Guid.NewGuid().ToString("N");
        public bool IsAlive => Age < Lifetime;

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, Vector2 acceleration,
                      float rotation, float rotationSpeed, float scale, float scaleDelta,
                      float lifetime, Color startColor, Color endColor,
                      float fadeInTime = 0.1f, float fadeOutTime = 0.3f,
                      bool emitsLight = false, Color? lightColor = null,
                      float lightIntensity = 0.5f, float lightIntensityDelta = -0.3f,
                      float lightRadius = 1.0f)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            Rotation = rotation;
            RotationSpeed = rotationSpeed;
            Scale = scale;
            ScaleDelta = scaleDelta;
            Lifetime = lifetime;
            Color = startColor;
            EndColor = endColor;
            FadeInTime = fadeInTime;
            FadeOutTime = fadeOutTime;
            Age = 0f;
            EmitsLight = emitsLight;
            LightColor = lightColor ?? startColor;
            LightIntensity = lightIntensity;
            LightIntensityDelta = lightIntensityDelta;
            LightRadius = lightRadius;
        }

        public void Update(float deltaTime)
        {
            Age += deltaTime;
            if (!IsAlive) return;

            Velocity += Acceleration * deltaTime;
            Position += Velocity * deltaTime;
            Rotation += RotationSpeed * deltaTime;
            Scale = Math.Max(Scale + ScaleDelta * deltaTime, 0);

            if (EmitsLight)
            {
                LightIntensity = MathHelper.Clamp(LightIntensity + LightIntensityDelta * deltaTime, 0f, 1f);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsAlive) return;

            float alpha = CalculateAlpha();
            Color currentColor = Color.Lerp(Color, EndColor, Age / Lifetime) * alpha;
            
            spriteBatch.Draw(
                Texture,
                Position,
                null,
                currentColor,
                Rotation,
                new Vector2(Texture.Width / 2f, Texture.Height / 2f),
                Scale,
                SpriteEffects.None,
                0f);
        }

        private float CalculateAlpha()
        {
            float alpha = 1f;
            if (Age < FadeInTime && FadeInTime > 0)
                alpha = Age / FadeInTime;
            else if (Age > Lifetime - FadeOutTime && FadeOutTime > 0)
                alpha = (Lifetime - Age) / FadeOutTime;
            return alpha;
        }

        public (string uid, Point position, Color color, float intensity) GetLightData()
        {
            if (!EmitsLight || !IsAlive)
                return (null, Point.Zero, Color.Transparent, 0f);

            Point tilePosition = new Point(
                (int)(Position.X / 8f),
                (int)(Position.Y / 8f));

            float currentIntensity = LightIntensity * (1f - (Age / Lifetime));
            return (LightUID, tilePosition, LightColor, currentIntensity * LightRadius);
        }
    }
}